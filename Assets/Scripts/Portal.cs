using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public Vector3 CamPoint;

    public bool RenderPortal;

    public Plane[] planes;

    public Plane portalPlane;

    public GameObject TargetSector;

    // Start is called before the first frame update
    void Start()
    {
        planes = new Plane[4];

        Mesh portalMesh = GetComponent<MeshFilter>().sharedMesh;

        int[] triangles = portalMesh.triangles;

        Vector3[] vertices = portalMesh.vertices;

        portalPlane = new Plane(transform.TransformPoint(vertices[0]), transform.TransformPoint(vertices[1]), transform.TransformPoint(vertices[2]));

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 p1 = transform.TransformPoint(vertices[triangles[i + 0]]);
            Vector3 p2 = transform.TransformPoint(vertices[triangles[i + 1]]);
            Vector3 p3 = transform.TransformPoint(vertices[triangles[i + 2]]);

            transform.parent.gameObject.GetComponent<Sector>().Planes.Add(new Plane(p1, p2, p3));
        }
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    public void SetPlanes()
    {
        Mesh portalMesh = GetComponent<MeshFilter>().sharedMesh;

        Vector3[] corners = portalMesh.vertices;

        CamPoint = Camera.main.transform.position;

        planes[0].Set3Points(CamPoint, transform.TransformPoint(corners[3]), transform.TransformPoint(corners[2]));
        planes[1].Set3Points(CamPoint, transform.TransformPoint(corners[2]), transform.TransformPoint(corners[1]));
        planes[2].Set3Points(CamPoint, transform.TransformPoint(corners[1]), transform.TransformPoint(corners[0]));
        planes[3].Set3Points(CamPoint, transform.TransformPoint(corners[0]), transform.TransformPoint(corners[3]));

    }
}
