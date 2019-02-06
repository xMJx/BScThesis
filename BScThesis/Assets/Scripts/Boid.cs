using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class Boid : MonoBehaviour
    {
        public List<GameObject> Neighbours { get; set; }

        public Vector2 Velocity;
        public Vector2 Heading;

        public float Mass;
        public float MaxSpeed;
        public float MaxForce;
        public float MaxTurnRate;

        public SteeringBehaviorsCPU SteeringBehaviorsCPU { get; set; }

        // Use this for initialization
        void Start()
        {
            Heading = new Vector2(0.0f, 1.0f);

            SteeringBehaviorsCPU = GetComponent<SteeringBehaviorsCPU>();

            Neighbours = new List<GameObject>();
            Neighbours.AddRange(GameObject.FindGameObjectsWithTag("SceneBoid"));
            Neighbours.Remove(this.gameObject);
        }

        // Update is called once per frame
        void Update()
        {
            if (GetComponent<Threat>() || !FindObjectOfType<SteeringBehaviorsGPU>())
            {
                Velocity = SteeringBehaviorsCPU.UpdateVelocity();
                Move();
            }
            
            // Rotate the boid
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
            // Normalized vector from boid to its target position
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