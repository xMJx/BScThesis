using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class SteeringBehaviorsGPU : MonoBehaviour
    {
        struct BoidData
        {
            public Vector2 pos;
            public Vector2 vel;
            public Vector2 heading;

            public Vector2 wanderTarget;

            public float randomSeed;
        }

        private readonly int structSize = 9 * sizeof(float);
        private ComputeBuffer boidDataBuffer;
        private List<Boid> boids;
        private BoidData[] boidsData;
        private Threat threat;

        public ComputeShader SteeringBehaviorsShader;

        public float WanderRadius;
        public float WanderDistance;
        public float WanderJitter;
        public float MaxBoidSpeed;
        public float ThreatRange;

        public float WanderWeight;
        public float FleeWeight;

        public float RandomValue;


        // Use this for initialization
        void Start()
        {
            boids = FindObjectsOfType<Boid>().ToList();
            threat = FindObjectOfType<Threat>();
            boids.Remove(threat.GetComponent<Boid>());
            boidDataBuffer = new ComputeBuffer(boids.Count, structSize);
            InitializeData();
        }

        // Update is called once per frame
        void Update()
        {
            DispatchAndUpdateData();
            MoveBoids();
        }

        private void InitializeData()
        {
            boidsData = new BoidData[boids.Count];

            for (int i = 0; i < boids.Count; i++)
            {
                BoidData bd = new BoidData
                {
                    pos = boids[i].transform.position,
                    vel = boids[i].Velocity,
                    heading = boids[i].Heading,

                    wanderTarget = boids[i].transform.position,
                    randomSeed = 2.0f * UnityEngine.Random.value - 1
                };
                boidsData[i] = bd;
            }

            int kernelIndex = SteeringBehaviorsShader.FindKernel("CSMain");
            SteeringBehaviorsShader.SetBuffer(kernelIndex, "BoidDataBuffer", boidDataBuffer);
            SteeringBehaviorsShader.SetFloat("DeltaTime", Time.deltaTime);

            SteeringBehaviorsShader.SetFloat("WanderRadius", WanderRadius);
            SteeringBehaviorsShader.SetFloat("WanderJitter", WanderJitter);
            SteeringBehaviorsShader.SetFloat("WanderDistance", WanderDistance);
            SteeringBehaviorsShader.SetFloat("MaxBoidSpeed", MaxBoidSpeed);
            SteeringBehaviorsShader.SetFloat("ThreatRange", ThreatRange);

            SteeringBehaviorsShader.SetFloat("WanderWeight", WanderWeight);
            SteeringBehaviorsShader.SetFloat("FleeWeight", FleeWeight);
            
            boidDataBuffer.SetData(boidsData);
        }

        private void DispatchAndUpdateData()
        {
            int kernelIndex = SteeringBehaviorsShader.FindKernel("CSMain");
            SteeringBehaviorsShader.SetFloat("DeltaTime", Time.deltaTime);

            RandomValue = 2.0f * UnityEngine.Random.value - 1;
            SteeringBehaviorsShader.SetFloat("RandomValueX", RandomValue);
            RandomValue = 2.0f * UnityEngine.Random.value - 1;
            SteeringBehaviorsShader.SetFloat("RandomValueY", RandomValue);

            SteeringBehaviorsShader.SetVector("ThreatPosition", (Vector2)threat.transform.position);

            SteeringBehaviorsShader.Dispatch(kernelIndex, 1, 1, 1);

            boidDataBuffer.GetData(boidsData);
        }

        private void MoveBoids()
        {
            for (int i = 0; i < boids.Count; i++)
            {
                boids[i].transform.position = boidsData[i].pos; // for position update
                boids[i].Velocity = boidsData[i].vel;           // for rotation
            }
        }

        private void OnDestroy()
        {
            boidDataBuffer.Dispose();
        }
    }
}