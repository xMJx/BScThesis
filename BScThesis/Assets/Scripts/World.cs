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

        private List<Boid> boids;
        private ComputeBuffer boidDataBuffer;

        public ComputeShader SteeringBehaviorsShader;
        
        // Use this for initialization
        void Start()
        {
            boids = FindObjectsOfType<Boid>().ToList();
            boidDataBuffer = new ComputeBuffer(boids.Count, sizeof(float) * 4);
        }

        // Update is called once per frame
        void Update()
        {
            BoidData[] boidsData = new BoidData[boids.Count];

            for (int i=0; i<boids.Count; i++)
            {
                BoidData bd = new BoidData();
                bd.pos = boids[i].transform.position;
                bd.vel = boids[i].Velocity;

                boidsData[i] = bd;
            }

            boidDataBuffer.SetData(boidsData);

            // compute shader here
            int kernelIndex = SteeringBehaviorsShader.FindKernel("CSMain");
            SteeringBehaviorsShader.SetBuffer(kernelIndex, "BoidDataBuffer", boidDataBuffer);
            SteeringBehaviorsShader.SetFloat("DeltaTime", Time.deltaTime);

            SteeringBehaviorsShader.Dispatch(kernelIndex, 1, 1, 1);

            boidDataBuffer.GetData(boidsData);

            for (int i=0; i<boids.Count; i++)
            {
                boids[i].transform.position = boidsData[i].pos;
            }
        }
    }
}