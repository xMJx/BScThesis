using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class SteeringBehaviors : MonoBehaviour
    {
        private Vector2 steeringForce;
        private Boid boid;

        public float DecelerationRate;

        public SteeringBehaviors()
        {

        }

        // Use this for initialization
        void Start()
        {
            boid = GetComponent<Boid>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Vector2 Calculate()
        {
            steeringForce = Vector2.zero;
            steeringForce = CalculatePrioritized();

            return steeringForce;
        }
        
        private Vector2 CalculatePrioritized()
        {
            React(boid.Others);
            
            return steeringForce;
        }

        private void React(List<GameObject> others)
        {
            foreach (var other in others)
            {
                // Threat
                if (other.GetComponent<Threat>() && (this.transform.position - other.transform.position).magnitude < other.GetComponent<Threat>().FearRange)
                {
                    steeringForce += this.Flee(other.transform.position);
                }

                // Food
                if (other.GetComponent<Food>() && this.GetComponent<Threat>())
                {
                    steeringForce += this.Seek(other.transform.position);
                }
            }
        }

        private Vector2 Seek(Vector2 target)
        {
            Vector2 desiredVelocity = (target - (Vector2)transform.position).normalized * boid.MaxSpeed;
            return desiredVelocity - boid.Velocity;
        }

        private Vector2 Flee(Vector2 threat)
        {
            Vector2 desiredVelocity = ((Vector2)transform.position - threat).normalized * boid.MaxSpeed;
            return desiredVelocity - boid.Velocity;
        }
        
        // WIP
        private Vector2 Arrive(Vector2 target)
        {
            
            float dist = (target - (Vector2)transform.position).magnitude;

            if (dist > 0.001)
            {
                // ? = s / a = t^2
                // speed to t^2?
                float speed = dist / DecelerationRate;
                speed = Mathf.Min(speed, boid.MaxSpeed);
                Vector2 desiredVelocity;

                // s <= a * t^2 / 2
                desiredVelocity = (target - (Vector2)transform.position) / dist * speed;

                return desiredVelocity - boid.Velocity;
            }
            else
            {
                return Vector2.zero;
            }
        }
    }
}