using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sector : MonoBehaviour
{
    public Vector3 CamPoint;

    public List<Plane> Planes = new List<Plane>();

    public List<GameObject> CheckSectors = new List<GameObject>();

    public List<GameObject> OutPortals = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

        int[] triangles = mesh.triangles;

        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = transform.TransformPoint(vertices[triangles[i + 0]]);
            Vector3 p2 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 p3 = transform.TransformPoint(vertices[triangles[i + 2]]);

            Planes.Add(new Plane(p1, p2, p3));
        }
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    public void CheckSector()
    {
        CamPoint = Camera.main.transform.position;

        bool PointIn = true;

        foreach (Plane plane in Planes)
        {

            if (plane.GetDistanceToPoint(CamPoint) < 0)
            {
                PointIn = false;
                break;
            }
        }

        if (PointIn == true)
        {
            Camera.main.GetComponent<Cam>().CurrentSector = gameObject;
        }
    }
}
