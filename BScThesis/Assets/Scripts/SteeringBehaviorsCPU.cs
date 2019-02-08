using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class SteeringBehaviorsCPU : MonoBehaviour
    {
        public float WanderWeight;
        public float FleeWeight;
        public float SeekWeight;
        
        public Vector2 WanderTarget;
        private float wanderRadius;
        private float wanderDistance;
        private float wanderJitter;


        private Vector2 steeringForce;
        private Vector2 acceleration;
        private Vector2 velocity;
        private Boid boid;
        private Threat threat;
        
        // Use this for initialization
        void Start()
        {
            wanderRadius = 3.0f;
            wanderDistance = 4.0f;
            wanderJitter = 6.0f;
            boid = GetComponent<Boid>();
            threat = FindObjectOfType<Threat>();
            velocity = Vector2.zero;

            //stuff for the wander behavior
            float theta = Random.value * Mathf.PI * 2.0f;

            //create a vector to a target position on the wander circle
            WanderTarget = new Vector2(wanderRadius * Mathf.Cos(theta),
                                        wanderRadius * Mathf.Sin(theta));
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Vector2 UpdateVelocity()
        {
            // get F
            steeringForce = CalculateForce();

            acceleration = steeringForce / boid.Mass;
            
            // v = v + F/m * t
            velocity += acceleration * Time.deltaTime;

            // v <= vmax
            velocity = Vector2.ClampMagnitude(velocity, boid.MaxSpeed);

            return velocity;
        }

        private Vector2 CalculateForce()
        {
            steeringForce = Vector2.zero;
            
            if ((boid.transform.position - threat.transform.position).magnitude < threat.GetComponent<Threat>().FearRange)
            {
                steeringForce += FleeWeight * Flee(threat.transform.position);
            }
            if (GetComponent<Threat>())
                steeringForce += WanderWeight * Wander();

            return steeringForce;
        }

        private Vector2 Flee(Vector2 threat)
        {
            Vector2 desiredVelocity = ((Vector2)transform.position - threat).normalized * boid.MaxSpeed;
            return desiredVelocity - boid.Velocity;
        }

        private Vector2 Wander()
        {
            WanderTarget += new Vector2((Random.value * 2 - 1) * wanderJitter, (Random.value * 2 - 1) * wanderJitter);
            WanderTarget = WanderTarget.normalized * wanderRadius;
            WanderTarget += boid.Heading.normalized * wanderDistance;
            
            return WanderTarget;
        }
        
    }
}