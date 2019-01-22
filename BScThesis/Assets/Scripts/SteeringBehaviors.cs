
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class SteeringBehaviors : MonoBehaviour
    {
        public float FeelerLength;
        public float WallAvoidanceWeight;
        public float WanderWeight;
        public float FleeWeight;
        public float SeekWeight;
        public float DecelerationRate;

        public Vector2 WanderTarget;
        public float WanderRadius;
        public float WanderDistance;
        public float WanderJitter;

        private Vector2 steeringForce;
        private Boid boid;

        private List<Vector2> feelers;

        LineRenderer feelerRenderer;
        LineRenderer wanderRenderer;

        public enum DebugRenderType { none, feelers, wanderTarget };
        public DebugRenderType DebugRender;


        public SteeringBehaviors()
        {

        }

        // Use this for initialization
        void Start()
        {
            WanderRadius = 3.0f;
            WanderDistance = 4.0f;
            WanderJitter = 6;
            boid = GetComponent<Boid>();

            //stuff for the wander behavior
            float theta = Random.value * Mathf.PI * 2.0f;

            //create a vector to a target position on the wander circle
            WanderTarget = new Vector2(WanderRadius * Mathf.Cos(theta),
                                        WanderRadius * Mathf.Sin(theta));
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
            //React(boid.OtherBoids);
            //steeringForce += WanderWeight * Wander();

            return steeringForce;
        }

        private void React(List<GameObject> others)
        {
            steeringForce += WallAvoidanceWeight * WallAvoidance();
            foreach (var other in others)
            {
                // Threat
                if (other.GetComponent<Threat>() && (this.transform.position - other.transform.position).magnitude < other.GetComponent<Threat>().FearRange)
                {
                    steeringForce += FleeWeight * this.Flee(other.transform.position);
                }

                // Food
                if (other.GetComponent<Food>() && this.GetComponent<Threat>())
                {
                    steeringForce += SeekWeight * this.Seek(other.transform.position);
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
            WanderTarget += new Vector2((Random.value * 2 - 1) * WanderJitter, (Random.value * 2 - 1) * WanderJitter);
            WanderTarget = WanderTarget.normalized * WanderRadius;
            WanderTarget += boid.Heading.normalized * WanderDistance;

            // debug

            if (DebugRender == DebugRenderType.wanderTarget)
            {
                wanderRenderer = GetComponent<LineRenderer>();
                wanderRenderer.positionCount = 2;
                wanderRenderer.SetPositions(new Vector3[2] { boid.transform.position, boid.transform.position + (Vector3)WanderTarget });
            }

            return WanderTarget;
        }
        
        private Vector2 WallAvoidance()
        {
            CreateFeelers();
            if (DebugRender == DebugRenderType.feelers)
            {
                RenderFeelers(); // debug
            }

            if (boid.Walls != null)
            {
                float distToThisIP = 0.0f;
                float distToClosestIP = float.MaxValue;

                int closestWall = -1;
                Vector2 point = Vector2.zero;
                Vector2 closestIP = Vector2.zero;
                Vector2 overshoot = Vector2.zero;

                foreach (Vector2 feeler in feelers)
                {
                    for (int w = 0; w < boid.Walls.Count; w++)
                    {
                        if (LineIntersection2D(boid.transform.position,
                                 feeler,
                                 boid.Walls[w].A,
                                 boid.Walls[w].B,
                                 ref distToThisIP,
                                 ref point))
                        {
                            if (distToThisIP < distToClosestIP)
                            {
                                distToClosestIP = distToThisIP;
                                closestWall = w;
                                closestIP = point;
                            }
                        }
                    }

                    if (closestWall >= 0)
                    {
                        overshoot = feeler - closestIP;
                    }

                }

                if (overshoot != Vector2.zero)
                    return boid.Walls[closestWall].Normal * overshoot.magnitude;
                else
                    return Vector2.zero;
            }
            else
                return Vector2.zero;

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
        
        private void CreateFeelers()
        {
            feelers = new List<Vector2>();

            //feeler pointing straight in front
            feelers.Add((Vector2)boid.transform.position + FeelerLength * boid.Heading);

            //feeler to left
            Vector3 temp = Quaternion.Euler(0, 0, 315) * boid.Heading;
            feelers.Add(boid.transform.position + FeelerLength / 2.0f * temp);

            //feeler to right
            temp = Quaternion.Euler(0, 0, 45) * boid.Heading;
            feelers.Add(boid.transform.position + FeelerLength / 2.0f * temp);
        }

        // debug
        private void RenderFeelers()
        {
            feelerRenderer = GetComponent<LineRenderer>();
            feelerRenderer.positionCount = 6;

            Vector3[] points = new Vector3[6];
            for (int i=0; i<3; i++)
            {
                points.SetValue((Vector3)feelers.ElementAt(i), 2*i);
                points.SetValue(boid.transform.position, 2*i+1);
            }
            feelerRenderer.SetPositions(points);
        }
        
        private bool LineIntersection2D(Vector2 A,
                                       Vector2 B,
                                       Vector2 C,
                                       Vector2 D,
                                       ref float dist,
                                       ref Vector2 point)
        {

            float rTop = (A.y - C.y) * (D.x - C.x) - (A.x - C.x) * (D.y - C.y);
            float rBot = (B.x - A.x) * (D.y - C.y) - (B.y - A.y) * (D.x - C.x);

            float sTop = (A.y - C.y) * (B.x - A.x) - (A.x - C.x) * (B.y - A.y);
            float sBot = (B.x - A.x) * (D.y - C.y) - (B.y - A.y) * (D.x - C.x);

            if ((rBot == 0) || (sBot == 0))
            {
                //lines are parallel
                return false;
            }

            float r = rTop / rBot;
            float s = sTop / sBot;

            if ((r > 0) && (r < 1) && (s > 0) && (s < 1))
            {
                dist = Vector2.Distance(A, B) * r;

                point = A + r * (B - A);

                return true;
            }
            else
            {
                dist = 0;

                return false;
            }
        }
    }
}