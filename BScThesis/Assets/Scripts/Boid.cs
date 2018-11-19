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

        public GameObject TempTarget;

        public float Mass { get; set; }
        public float MaxSpeed { get; set; }
        public float MaxForce { get; set; }
        public float MaxTurnRate { get; set; }

        public SteeringBehaviors SteeringBehaviors = new SteeringBehaviors();

        // Use this for initialization
        void Start()
        {
            MaxTurnRate = 1.0f;
            Heading = new Vector2(0.0f, 1.0f);
        }

        // Update is called once per frame
        void Update()
        {
            //Vector2 SteeringForce = SteeringBehaviors.Calculate();

            RotateHeadingToFacePosition(TempTarget.transform.position);
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

            //Heading = toTarget;
            Heading.Normalize();
            
            Heading = Quaternion.Euler(0, 0, angle) * Heading;


            return false;
        }

        bool RotateBoidToMatchHeading()
        {
            if (transform.rotation.)
                return true;

            transform.rotation = Quaternion.EulerRotation(0,0,-Mathf.Atan2(Heading.x, Heading.y));

            return false;
        }

    }
}