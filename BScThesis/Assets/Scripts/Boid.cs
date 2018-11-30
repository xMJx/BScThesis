using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class Boid : MonoBehaviour
    {
        public Vector2 Velocity;
        public Vector2 Heading;
        public Vector2 Side;

        public float Mass { get; set; }
        public float MaxSpeed { get; set; }
        public float MaxForce { get; set; }
        public float MaxTurnRate { get; set; }

        private SteeringBehaviors SteeringBehaviors;

        // Use this for initialization
        void Start()
        {
            MaxTurnRate = 20.0f;
            Heading = new Vector2(0.0f, 1.0f);
            SteeringBehaviors = GetComponent<SteeringBehaviors>();
            Mass = 1.0f;
            MaxSpeed = 4.0f;
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 steeringForce = SteeringBehaviors.Calculate();
            Vector2 acceleration = steeringForce / Mass;
            Velocity += acceleration * Time.fixedDeltaTime;
            Velocity = Vector2.ClampMagnitude(Velocity, MaxSpeed);
            Move();
            
            RotateHeadingToFacePosition((Vector2)transform.position + Velocity);
            RotateBoidToMatchHeading();
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
            transform.rotation = Quaternion.EulerRotation(0,0,-Mathf.Atan2(Heading.x, Heading.y));

            return false;
        }

        void Move()
        {
            Vector2 newPosition = (Vector2)transform.position + Velocity * Time.fixedDeltaTime;
            transform.position = newPosition;
        }
    }
}