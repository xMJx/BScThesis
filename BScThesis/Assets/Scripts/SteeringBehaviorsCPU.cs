using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class SteeringBehaviorsCPU : MonoBehaviour
    {
        public Transform Threat;

        public float WanderWeight;
        public float FleeWeight;
        public float SeparationWeight;
        public float CohesionWeight;
        public float AlignmentWeight;
        
        public float NeighborhoodRange;

        private Vector2 wanderTarget;
        private float wanderRadius;
        private float wanderDistance;
        private float wanderJitter;


        private Vector2 steeringForce;
        private Vector2 acceleration;
        private Vector2 velocity;
        private Boid boid;

        private List<Boid> otherBoids;
        private List<int> neighborsTags;


        
        // Use this for initialization
        void Start()
        {
            wanderRadius = 3.0f;
            wanderDistance = 4.0f;
            wanderJitter = 6.0f;
            boid = GetComponent<Boid>();

            otherBoids = new List<Boid>(FindObjectsOfType<Boid>());
            otherBoids.Remove(this.boid);

            neighborsTags = new List<int>();

            velocity = Vector2.zero;

            if (boid.IsThreat)
            {
                //stuff for the wander behavior
                float theta = Random.value * Mathf.PI * 2.0f;

                //create a vector to a target position on the wander circle
                wanderTarget = new Vector2(wanderRadius * Mathf.Cos(theta),
                                            wanderRadius * Mathf.Sin(theta));
            }

        }

        // Update is called once per frame
        void Update()
        {

        }

        public Vector2 UpdateVelocity()
        {
            // get F
            steeringForce = CalculateForce();

            // a = F/m
            acceleration = steeringForce / boid.Mass;

            // v = v + a * t
            velocity += acceleration * Time.deltaTime;

            // v <= vmax
            velocity = Vector2.ClampMagnitude(velocity, boid.MaxSpeed);

            return velocity;
        }

        private Vector2 CalculateForce()
        {
            steeringForce = Vector2.zero;

            if (boid.IsThreat)
                steeringForce += WanderWeight * Wander();
            else
            {
                FindNeighbours();
                steeringForce += SeparationWeight * Separation();
                steeringForce += AlignmentWeight * Alignment();
                steeringForce += CohesionWeight * Cohesion();
                
                if ((boid.transform.position - Threat.position).magnitude < Threat.GetComponent<Boid>().ThreatRange)
                {
                    steeringForce += FleeWeight * Flee(Threat.position);
                }

            }

            return steeringForce;
        }

        private void FindNeighbours()
        {
            neighborsTags.Clear();

            for (int i = 0; i < otherBoids.Count; i++)
            {
                if ((otherBoids[i].transform.position - this.transform.position).magnitude <= NeighborhoodRange)
                {
                    neighborsTags.Add(i);
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

        private Vector2 Wander()
        {
            wanderTarget += new Vector2((Random.value * 2 - 1) * wanderJitter, (Random.value * 2 - 1) * wanderJitter);
            wanderTarget = wanderTarget.normalized * wanderRadius;
            wanderTarget += boid.Heading.normalized * wanderDistance;

            return wanderTarget;
        }

        private Vector2 Separation()
        {
            Vector2 separationForce = new Vector2();

            foreach (int tag in neighborsTags)
            {
                Vector2 ToAgent = this.transform.position - otherBoids[tag].transform.position;
                separationForce += ToAgent.normalized / ToAgent.magnitude;
            }

            return separationForce;
        }

        private Vector2 Alignment()
        {
            Vector2 averageHeading = new Vector2();
            int neighborCount = 0;
            
            foreach (int tag in neighborsTags)
            {
                averageHeading += otherBoids[tag].Velocity.normalized;

                neighborCount++;
            }
            
            if (neighborCount > 0)
            {
                averageHeading /= neighborCount;

                averageHeading -= velocity.normalized;
            }

            return averageHeading;
        }

        private Vector2 Cohesion()
        {
            Vector2 centerOfMass = new Vector2();
            Vector2 cohesionForce = new Vector2();
            int neighborCount = 0;
            
            foreach (int tag in neighborsTags)
            {
                centerOfMass += (Vector2)otherBoids[tag].transform.position;
                neighborCount++;
            }

            if (neighborCount > 0)
            {
                centerOfMass /= neighborCount;
                cohesionForce = Seek(centerOfMass);
            }

            return cohesionForce.normalized;
        }

    }
}