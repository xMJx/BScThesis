using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class SteeringBehaviors : MonoBehaviour
    {
        public enum Deceleration { light, moderate, strong};
        private Vector2 steeringForce;
        private Boid boid;

        public Vector2 SeekTargetPosition;
        public bool SeekOn;
        public float SeekWeight;

        public Vector2 FleeThreatPosition;
        public bool FleeOn;
        public float FleeWeight;
        public float PanicRange;

        public Vector2 ArriveTargetPosition;
        public bool ArriveOn;
        public Deceleration DecelerationRate;

        
        
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
            steeringForce = new Vector2(0.0f, 0.0f);
            steeringForce = CalculatePrioritized();

            return steeringForce;
        }

        private Vector2 CalculatePrioritized()
        {
            if (SeekOn)
            {
                steeringForce += SeekWeight * Seek(SeekTargetPosition);
            }
            if (FleeOn)
            {
                steeringForce += FleeWeight * Flee(FleeThreatPosition);
            }
            if (ArriveOn)
            {
                steeringForce += Arrive(ArriveTargetPosition, DecelerationRate);
            }
            return steeringForce;
        }

        private Vector2 Seek (Vector2 target)
        {
            Vector2 desiredVelocity = (target - (Vector2)transform.position).normalized * boid.MaxSpeed;
            return desiredVelocity - boid.Velocity;
        }

        private Vector2 Flee (Vector2 threat)
        {
            if ((threat-(Vector2)transform.position).magnitude < PanicRange)
            {
                Vector2 desiredVelocity = ((Vector2)transform.position - threat).normalized * boid.MaxSpeed;
                return desiredVelocity - boid.Velocity;
            }
            else
            {
                return new Vector2(0,0);
            }
        }

        private Vector2 Arrive(Vector2 target, Deceleration decelerationRate)
        {
            if ((target - (Vector2)transform.position).magnitude > 0)
            {
                float speed = (target - (Vector2)transform.position).magnitude / ((int)decelerationRate * 0.3f);

                Vector2 desiredVelocity = (target - (Vector2)transform.position).normalized * speed;
                return desiredVelocity - boid.Velocity;
            }
            else
            {
                return new Vector2(0, 0);
            }
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

        //public void SeekOn()
        //{
        //    throw new NotImplementedException();
        //}

        //public void FleeOn()
        //{
        //    throw new NotImplementedException();

        //}

        //public void ArriveOn()
        //{
        //    throw new NotImplementedException();

        //}

        //public void SeekOff()
        //{
        //    throw new NotImplementedException();

        //}

        //public void FleeOff()
        //{
        //    throw new NotImplementedException();

        //}

        //public void ArriveOff()
        //{
        //    throw new NotImplementedException();

        //}
    }
}