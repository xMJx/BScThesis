using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class SteeringBehaviors : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
        
        private Vector2 Seek (Vector2 target)
        {
            return new Vector2();
        }

        private Vector2 Flee (Vector2 threat)
        {
            return new Vector2();
        }

        private Vector2 Arrive (Vector2 target)
        {
            return new Vector2();
        }

        public Vector2 Calculate()
        {
            return new Vector2();
        }

        public Vector2 ForwardComponent()
        {
            return new Vector2();
        }

        public Vector2 SideComponent()
        {
            return new Vector2();
        }

        public void SetPath()
        {

        }

        public void SetTarget(Vector2 target)
        {

        }

        public void SetTargetAgent1(Boid agent1)
        {

        }

        public void SetTargetAgent2(Boid agent2)
        {

        }

        public void SeekOn()
        {

        }

        public void FleeOn()
        {

        }

        public void ArriveOn()
        {

        }

        public void SeekOff()
        {

        }

        public void FleeOff()
        {

        }

        public void ArriveOff()
        {

        }
    }
}