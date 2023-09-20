using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public bool RenderPortal;

    public Plane[] planes;

    public Plane portalPlane;

    public GameObject TargetSector;

    public List<Vector3> corners = new List<Vector3>();

    public List<Vector3> cornertp = new List<Vector3>();

    public List<int> triangles = new List<int>();

    // Start is called before the first frame update
    void Awake()
    {
        SetMesh();

        planes = new Plane[4];

        portalPlane = new Plane(cornertp[0], cornertp[1], cornertp[2]);
    }

    public void SetMesh()
    {
        Mesh portalMesh = GetComponent<MeshFilter>().sharedMesh;

        portalMesh.GetTriangles(triangles, 0);

        portalMesh.GetVertices(corners);

        for (int i = 0; i < corners.Count; i++)
        {
            cornertp.Add(transform.TransformPoint(corners[i]));
        }
    }

    public void SetPortal()
    {
        Vector3 CamPoint = Camera.main.transform.position;

        planes[0].Set3Points(CamPoint, cornertp[3], cornertp[2]);
        planes[1].Set3Points(CamPoint, cornertp[2], cornertp[1]);
        planes[2].Set3Points(CamPoint, cornertp[1], cornertp[0]);
        planes[3].Set3Points(CamPoint, cornertp[0], cornertp[3]);
    }
}
