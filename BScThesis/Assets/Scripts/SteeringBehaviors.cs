using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class SteeringBehaviors : MonoBehaviour
    {
        public enum Behaviors { Seek, Flee, Arrive }

        private Vector2 steeringForce;
        public Vector2 targetPosition;
        private Boid boid;
        private float weight;
        public Behaviors behaviors;
        
        public SteeringBehaviors()
        {
            weight = 1.0f;
            behaviors = Behaviors.Seek;
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
            steeringForce = new Vector2(0.0f, 0.0f);
            steeringForce = CalculatePrioritized();

            return steeringForce;
        }

        private Vector2 CalculatePrioritized()
        {
            switch (behaviors)
            {
                case Behaviors.Seek:
                    {
                        steeringForce += weight * Seek(targetPosition);
                        break;
                    }
                case Behaviors.Flee:
                    {
                        steeringForce += weight * Flee(targetPosition);
                        break;
                    }
                case Behaviors.Arrive:
                    {
                        steeringForce += weight * Arrive(targetPosition);
                        break;
                    }
                default:
                    break;
            }
            return steeringForce;
        }

        private Vector2 Seek (Vector2 target)
        {
            Vector2 ret = (target - (Vector2)transform.position).normalized * boid.MaxSpeed;
            return ret;
        }

        private Vector2 Flee (Vector2 threat)
        {
            Vector2 ret = ((Vector2)transform.position - threat).normalized * boid.MaxSpeed;
            return (Vector2)transform.position - threat;
        }

        private Vector2 Arrive (Vector2 target)
        {
            throw new NotImplementedException();
        }


        public Vector2 ForwardComponent()
        {
            throw new NotImplementedException();
        }

        public Vector2 SideComponent()
        {
            throw new NotImplementedException();
        }

        public void SetPath()
        {
            throw new NotImplementedException();
        }

        public void SetTarget(Vector2 target)
        {
            throw new NotImplementedException();
        }

        public void SetTargetAgent1(Boid agent1)
        {
            throw new NotImplementedException();
        }

        public void SetTargetAgent2(Boid agent2)
        {
            throw new NotImplementedException();
        }

        public void SeekOn()
        {
            throw new NotImplementedException();
        }

        public void FleeOn()
        {
            throw new NotImplementedException();

        }

        public void ArriveOn()
        {
            throw new NotImplementedException();

        }

        public void SeekOff()
        {
            throw new NotImplementedException();

        }

        public void FleeOff()
        {
            throw new NotImplementedException();

        }

        public void ArriveOff()
        {
            throw new NotImplementedException();

        }
    }
}