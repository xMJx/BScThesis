using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SteeringBehaviorsNS
{

    public class World : MonoBehaviour
    {
        private List<Boid> boids;
        private float[] boidsData;
        private ComputeBuffer computeBuffer;
        
        // Use this for initialization
        void Start()
        {
            boids = FindObjectsOfType<Boid>().ToList();
            computeBuffer = new ComputeBuffer(boids.Count, 16);
            boidsData = new float[boids.Count * 4];
        }

        // Update is called once per frame
        void Update()
        {
            for (int i=0; i<boids.Count; i++)
            {
                boidsData[4 * i] = boids[i].transform.position.x;
                boidsData[4 * i + 1] = boids[i].transform.position.y;
                boidsData[4 * i + 2] = boids[i].Velocity.x;
                boidsData[4 * i + 3] = boids[i].Velocity.y;
            }
            computeBuffer.SetData(boidsData);
        }
    }
}