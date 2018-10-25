using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class Boid : MonoBehaviour
    {

        public Vector2 Velocity { get; set; }
        public Vector2 Heading { get; set; }
        public Vector2 Side { get; set; }

        public double Mass { get; set; }
        public double MaxSpeed { get; set; }
        public double MaxForce { get; set; }
        public double MaxTurnRate { get; set; }

        public SteeringBehaviors SteeringBehaviors { get; set; }

        // Use this for initialization
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            Vector2 SteeringForce = SteeringBehaviors.Calculate();
        }
    }
}