﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class Boid : MonoBehaviour
    {
        public bool IsThreat;
        public float ThreatRange;

        public SteeringBehaviorsCPU SteeringBehaviorsCPU { get; set; }
        public SteeringBehaviorsGPU SteeringBehaviorsGPU { get; set; }
        public Vector2 Velocity { get; set; }
        public Vector2 Heading { get; set; }

        public float Mass;
        public float MaxSpeed;
        public float MaxForce;
        public float MaxTurnRate;
        

        private int frameCount;

        // Use this for initialization
        void Start()
        {
            Heading = new Vector2(0.0f, 1.0f);
            SteeringBehaviorsCPU = GetComponent<SteeringBehaviorsCPU>();
            SteeringBehaviorsGPU = FindObjectOfType<SteeringBehaviorsGPU>();
            
            frameCount = 0;
        }

        // Update is called once per frame
        void Update()
        {
            if (frameCount == 0 && IsThreat)
            {
                Debug.Log("Start: " + Time.realtimeSinceStartup);
            }

            if (IsThreat || SteeringBehaviorsGPU == null)
            {
                Velocity = SteeringBehaviorsCPU.UpdateVelocity();
                Move();
            }
            
            // Rotate the boid
            RotateHeadingToFacePosition((Vector2)transform.position + Velocity);
            RotateBoidToMatchHeading();
            
            if (frameCount == 100 && IsThreat)
            {
                Debug.Log("100 frames: " + Time.realtimeSinceStartup);
            }
            if (frameCount == 500 && IsThreat)
            {
                Debug.Log("500 frames: " + Time.realtimeSinceStartup);
            }
            if (frameCount == 1000 && IsThreat)
            {
                Debug.Log("1000 frames: " + Time.realtimeSinceStartup);
            }
            frameCount++;
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