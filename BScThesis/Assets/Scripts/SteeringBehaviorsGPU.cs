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
        }

        private readonly int structSize = 4 * sizeof(float);
        private ComputeBuffer boidDataBuffer;
        //private ComputeBuffer debugBuffer;
        private List<Boid> boids;
        private BoidData[] boidsData;
        //public Vector2[] debugData;
        private Threat threat;

        public ComputeShader SteeringBehaviorsShader;
        
        public float MaxBoidSpeed;
        public float ThreatRange;
        public float NeighborhoodRange;
        
        public float FleeWeight;
        public float CohesionWeight;
        public float AlignmentWeight;
        public float SeparationWeight;


        // Use this for initialization
        void Start()
        {
            boids = FindObjectsOfType<Boid>().ToList();
            threat = FindObjectOfType<Threat>();
            boids.Remove(threat.GetComponent<Boid>());
            boidDataBuffer = new ComputeBuffer(boids.Count, structSize);
            //debugBuffer = new ComputeBuffer(boids.Count, 2* sizeof(float));
            
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
                    vel = boids[i].Velocity
                };
                boidsData[i] = bd;
            }

            //debugData = new Vector2[boids.Count];

            boidDataBuffer.SetData(boidsData);

            int kernelIndex = SteeringBehaviorsShader.FindKernel("CSMain");
            SteeringBehaviorsShader.SetBuffer(kernelIndex, "BoidDataBuffer", boidDataBuffer);
            //SteeringBehaviorsShader.SetBuffer(kernelIndex, "DebugBuffer", debugBuffer);

            SteeringBehaviorsShader.SetFloat("MaxBoidSpeed", MaxBoidSpeed);
            SteeringBehaviorsShader.SetInt("BoidCount", boidDataBuffer.count);
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
            SteeringBehaviorsShader.SetVector("ThreatPosition", (Vector2)threat.transform.position);

            int kernelIndex = SteeringBehaviorsShader.FindKernel("CSMain");
            SteeringBehaviorsShader.Dispatch(kernelIndex, 1, 1, 1);

            boidDataBuffer.GetData(boidsData);
            //debugBuffer.GetData(debugData);
        }

        private void MoveBoids()
        {
            for (int i = 0; i < boids.Count; i++)
            {
                boids[i].transform.position = boidsData[i].pos; // for position update
                boids[i].Velocity = boidsData[i].vel;           // for rotation
                //boids[i].DebugValueGPU = debugData[i];
            }
        }

        private void OnDestroy()
        {
            boidDataBuffer.Dispose();
        }
    }
}