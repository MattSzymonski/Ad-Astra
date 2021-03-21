using UnityEngine;
using System.Collections;

public class BackgroundGradient : MonoBehaviour {

    Mesh mesh;
    Vector3[] vertices;
    Color[] cols;

    public Color top;
    public Color bottom;

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        vertices = mesh.vertices;
        cols = new Color[vertices.Length];     
    }

    void Update()
    {
        for (var i = 0; i < vertices.Length; i++)
        {
            cols[i] = Color.Lerp(top, bottom, vertices[i].y + 0.5f);         
        }
        mesh.colors = cols;
    }

}
