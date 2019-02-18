using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class SteeringBehaviorsGPU : MonoBehaviour
    {
        public ComputeShader SteeringBehaviorsShader;
        public Transform Threat;

        public float BoidMass;
        public float MaxBoidSpeed;
        public float ThreatRange;
        public float NeighborhoodRange;
        
        public float FleeWeight;
        public float CohesionWeight;
        public float AlignmentWeight;
        public float SeparationWeight;

        struct BoidData
        {
            public Vector2 pos;
            public Vector2 vel;
        }
        
        private List<Boid> boids;
        private BoidData[] boidsData;
        private ComputeBuffer boidDataBuffer;

        // Use this for initialization
        void Start()
        {
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
            boids = FindObjectsOfType<Boid>().ToList();
            boids.Remove(Threat.GetComponent<Boid>());
            boidDataBuffer = new ComputeBuffer(boids.Count, 4 * sizeof(float));

            boidsData = new BoidData[boids.Count];

            for (int i = 0; i < boids.Count; i++)
            {
                BoidData bd = new BoidData
                {
                    pos = boids[i].transform.position,
                    vel = boids[i].Velocity
                };
                boidsData[i] = bd;
            }

            boidDataBuffer.SetData(boidsData);

            int kernelIndex = SteeringBehaviorsShader.FindKernel("CSMain");
            SteeringBehaviorsShader.SetBuffer(kernelIndex, "BoidDataBuffer", boidDataBuffer);

            SteeringBehaviorsShader.SetInt("BoidCount", boidDataBuffer.count);
            SteeringBehaviorsShader.SetFloat("MaxBoidSpeed", MaxBoidSpeed);
            SteeringBehaviorsShader.SetFloat("BoidMass", BoidMass);
            SteeringBehaviorsShader.SetFloat("ThreatRange", ThreatRange);
            SteeringBehaviorsShader.SetFloat("NeighborhoodRange", NeighborhoodRange);

            SteeringBehaviorsShader.SetFloat("FleeWeight", FleeWeight);
            SteeringBehaviorsShader.SetFloat("CohesionWeight", CohesionWeight);
            SteeringBehaviorsShader.SetFloat("AlignmentWeight", AlignmentWeight);
            SteeringBehaviorsShader.SetFloat("SeparationWeight", SeparationWeight);
        }

        private void DispatchAndUpdateData()
        {
            SteeringBehaviorsShader.SetFloat("DeltaTime", Time.deltaTime);
            SteeringBehaviorsShader.SetVector("ThreatPosition", (Vector2)Threat.position);

            int kernelIndex = SteeringBehaviorsShader.FindKernel("CSMain");
            SteeringBehaviorsShader.Dispatch(kernelIndex, boids.Count/1, 1, 1);

            boidDataBuffer.GetData(boidsData);
        }

        private void MoveBoids()
        {
            for (int i = 0; i < boids.Count; i++)
            {
                boids[i].transform.position = boidsData[i].pos;
                boids[i].Velocity = boidsData[i].vel;
            }
        }

        private void OnDestroy()
        {
            boidDataBuffer.Dispose();
        }
    }
}