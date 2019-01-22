using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class Boid : MonoBehaviour
    {
        public List<GameObject> OtherBoids { get; set; }
        public List<Wall> Walls { get; set; }

        public Vector2 Velocity;
        public Vector2 Heading;
        public Vector2 Side;

        public float Mass;
        public float MaxSpeed;
        public float MaxForce;
        public float MaxTurnRate;

        public SteeringBehaviors SteeringBehaviors { get; set; }

        // Use this for initialization
        void Start()
        {
            Heading = new Vector2(0.0f, 1.0f);

            SteeringBehaviors = GetComponent<SteeringBehaviors>();

            Walls = new List<Wall>();
            foreach (GameObject o in GameObject.FindGameObjectsWithTag("SceneWall"))
            {
                Walls.Add(o.GetComponent<Wall>());
            }

            OtherBoids = new List<GameObject>();
            OtherBoids.AddRange(GameObject.FindGameObjectsWithTag("SceneBoid"));
            OtherBoids.Remove(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            // get F
            Vector2 steeringForce = SteeringBehaviors.Calculate();

            // a = F/m
            Vector2 acceleration = steeringForce / Mass;
            
            // v = v + at
            Velocity += acceleration * Time.deltaTime;

            // v <= vmax
            Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);

            // s = s + vt
            //Move();
            
            // wizualizacja
            RotateHeadingToFacePosition((Vector2)transform.position + Velocity);
            RotateBoidToMatchHeading();
        }

        void Move()
        {
            Vector2 newPosition = (Vector2)transform.position + Velocity * Time.deltaTime;
            transform.position = newPosition;
        }

        bool RotateHeadingToFacePosition(Vector2 target)
        {
            // Normalizowany wektor od boidu do celu
            Vector2 toTarget = (target - (Vector2)transform.position).normalized;

            float angle = Vector2.SignedAngle(Heading, toTarget);

            if (angle > 0)
                angle = Mathf.Min(angle, MaxTurnRate);
            else
                angle = -Mathf.Min(-angle, MaxTurnRate);
            
            Heading.Normalize();
            
            Heading = Quaternion.Euler(0, 0, angle) * Heading;
            
            return false;
        }

        bool RotateBoidToMatchHeading()
        {
            transform.rotation = Quaternion.Euler(0, 0, Mathf.Rad2Deg * -Mathf.Atan2(Heading.x, Heading.y));

            return false;
        }
    }
}