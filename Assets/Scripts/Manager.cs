using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public Camera Cam;

    public Plane[] planes;

    public Collider Player;

    public GameObject CurrentSector;

    public List<GameObject> VisitedSector = new List<GameObject>();

    public List<GameObject> AllSector = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        SetPlanes();
    }

    // Update is called once per frame
    void Update()
    {
        CheckSector();

        planes = GeometryUtility.CalculateFrustumPlanes(Cam);

        VisitedSector.Clear();

        GetSector(planes, CurrentSector);
    }

    public void SetPlanes()
    {
        for (int i = 0; i < AllSector.Count; i++)
        {
            for (int e = 0; e < AllSector[i].GetComponent<Sector>().triangles.Count; e += 3)
            {
                Vector3 p1 = AllSector[i].GetComponent<Sector>().verticestp[AllSector[i].GetComponent<Sector>().triangles[e + 0]];
                Vector3 p2 = AllSector[i].GetComponent<Sector>().verticestp[AllSector[i].GetComponent<Sector>().triangles[e + 1]];
                Vector3 p3 = AllSector[i].GetComponent<Sector>().verticestp[AllSector[i].GetComponent<Sector>().triangles[e + 2]];

                AllSector[i].GetComponent<Sector>().Planes.Add(new Plane(p1, p2, p3));
            }
            for (int e = 0; e < AllSector[i].GetComponent<Sector>().OutPortals.Count; e++)
            {
                for (int r = 0; r < AllSector[i].GetComponent<Sector>().OutPortals[e].GetComponent<Portal>().triangles.Count; r += 3)
                {
                    Vector3 p1 = AllSector[i].GetComponent<Sector>().OutPortals[e].GetComponent<Portal>().cornertp[AllSector[i].GetComponent<Sector>().OutPortals[e].GetComponent<Portal>().triangles[r + 0]];
                    Vector3 p2 = AllSector[i].GetComponent<Sector>().OutPortals[e].GetComponent<Portal>().cornertp[AllSector[i].GetComponent<Sector>().OutPortals[e].GetComponent<Portal>().triangles[r + 1]];
                    Vector3 p3 = AllSector[i].GetComponent<Sector>().OutPortals[e].GetComponent<Portal>().cornertp[AllSector[i].GetComponent<Sector>().OutPortals[e].GetComponent<Portal>().triangles[r + 2]];

                    AllSector[i].GetComponent<Sector>().Planes.Add(new Plane(p1, p2, p3));
                }
            }
        }
    }

    public void CheckSector()
    {
        Vector3 CamPoint = Cam.transform.position;

        for (int i = 0; i < CurrentSector.GetComponent<Sector>().CheckSectors.Count; i++)
        {
            bool PointIn = true;

            for (int e = 0; e < CurrentSector.GetComponent<Sector>().CheckSectors[i].GetComponent<Sector>().Planes.Count; e++)
            {
                if (CurrentSector.GetComponent<Sector>().CheckSectors[i].GetComponent<Sector>().Planes[e].GetDistanceToPoint(CamPoint) < 0)
                {
                    PointIn = false;
                    break;
                }
            }

            if (PointIn == true)
            {
                CurrentSector = CurrentSector.GetComponent<Sector>().CheckSectors[i];
            }
        }

        IEnumerable<GameObject> except = AllSector.Except(CurrentSector.GetComponent<Sector>().CheckSectors);

        foreach (GameObject sector in except)
        {
            Physics.IgnoreCollision(Player, sector.GetComponent<Collider>(), true);
        }

        for (int i = 0; i < CurrentSector.GetComponent<Sector>().CheckSectors.Count; ++i)
        {
            GameObject Check = CurrentSector.GetComponent<Sector>().CheckSectors[i];

            Physics.IgnoreCollision(Player, Check.GetComponent<Collider>(), false);
        }
    }

    public void GetSector(Plane[] APlanes, GameObject BSector)
    {
        Vector3 CamPoint = Cam.transform.position;

        Matrix4x4 matrix = Matrix4x4.TRS(BSector.transform.position, BSector.transform.rotation, BSector.transform.lossyScale);

        Graphics.DrawMesh(BSector.GetComponent<MeshFilter>().mesh, matrix, BSector.GetComponent<Renderer>().sharedMaterial, 0, Camera.main, 0, null, false, false);

        VisitedSector.Add(BSector);

        for (int i = 0; i < BSector.GetComponent<Sector>().OutPortals.Count; ++i)
        {
            GameObject p = BSector.GetComponent<Sector>().OutPortals[i];

            float d = p.GetComponent<Portal>().portalPlane.GetDistanceToPoint(CamPoint);

            if (p.GetComponent<Portal>().RenderPortal == true)
            {
                Matrix4x4 matrix2 = Matrix4x4.TRS(p.transform.position, p.transform.rotation, p.transform.lossyScale);

                Graphics.DrawMesh(p.GetComponent<MeshFilter>().mesh, matrix2, p.GetComponent<Renderer>().sharedMaterial, 0, Camera.main, 0, null, false, false);
            }

            if (d < -0.1f)
            {
                continue;
            }

            if (VisitedSector.Contains(p.GetComponent<Portal>().TargetSector) && d < 0.1f)
            {
                continue;
            }

            if (d < 1f)
            {
                GetSector(planes, p.GetComponent<Portal>().TargetSector);
                continue;
            }

            if (GeometryUtility.TestPlanesAABB(APlanes, p.GetComponent<Renderer>().bounds))
            {
                p.GetComponent<Portal>().SetPortal();

                GetSector(p.GetComponent<Portal>().planes, p.GetComponent<Portal>().TargetSector);
            }
        }
    }
}
