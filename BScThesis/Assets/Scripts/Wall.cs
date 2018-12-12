using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour {

    public Vector2 A;
    public Vector2 B;
    public Vector2 Normal;

    private LineRenderer lineRenderer;

    public Wall (Vector2 from, Vector2 to)
    {
        A = from;
        B = to;
    }

	// Use this for initialization
	void Start ()
    {
        lineRenderer = GetComponent<LineRenderer>();
        Vector3[] positions = { A, B };
        lineRenderer.SetPositions(positions);
	}
	
	// Update is called once per frame
	void Update ()
    {

    }
}
