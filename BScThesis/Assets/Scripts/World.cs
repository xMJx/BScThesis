using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class World : MonoBehaviour
    {
        struct BoidData
        {
            public Vector2 pos;
            public Vector2 vel;

        }

        private readonly int structSize = 4 * sizeof(float);

        private List<Boid> boids;
        private ComputeBuffer boidDataBuffer;
        private BoidData[] boidsData;

        public ComputeShader SteeringBehaviorsShader;
        
        // Use this for initialization
        void Start()
        {
            boids = FindObjectsOfType<Boid>().ToList();
            boidDataBuffer = new ComputeBuffer(boids.Count, structSize);
        }

        // Update is called once per frame
        void Update()
        {
            // TODO: move to start for performance purposes
            boidsData = new BoidData[boids.Count];

            for (int i=0; i<boids.Count; i++)
            {
                BoidData bd = new BoidData
                {
                    pos = boids[i].transform.position,
                    vel = boids[i].Velocity
                };

                boidsData[i] = bd;
            }

            DispatchAndUpdateData();
            MoveBoids();
        }


        private void DispatchAndUpdateData()
        {
            boidDataBuffer.SetData(boidsData); // TODO: don't do this for performance purposes

            int kernelIndex = SteeringBehaviorsShader.FindKernel("CSMain");
            SteeringBehaviorsShader.SetBuffer(kernelIndex, "BoidDataBuffer", boidDataBuffer);
            SteeringBehaviorsShader.SetFloat("DeltaTime", Time.deltaTime);

            SteeringBehaviorsShader.Dispatch(kernelIndex, 1, 1, 1);

            boidDataBuffer.GetData(boidsData);
        }

        private void MoveBoids()
        {
            for (int i = 0; i < boids.Count; i++)
            {
                boids[i].transform.position = boidsData[i].pos;
            }
        }

        private void OnDestroy()
        {
            boidDataBuffer.Release();
        }
    }
}