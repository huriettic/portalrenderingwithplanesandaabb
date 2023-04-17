/******************************************************************************
 * The MIT License (MIT)
 * 
 * Copyright (c) 2016 Bunny83
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to
 * deal in the Software without restriction, including without limitation the
 * rights to use, copy, modify, merge, publish, distribute, sublicense, and/or
 * sell copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
 * FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
 * DEALINGS IN THE SOFTWARE.
 *****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manager : MonoBehaviour
{
    public Vector3 CamPoint;

    public List<GameObject> VisitedSector = new List<GameObject>();

    public List<GameObject> AllSector = new List<GameObject>();

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < AllSector.Count; ++i)
        {
            AllSector[i].GetComponent<Collider>().enabled = false;
        }

        for (int i = 0; i < Camera.main.GetComponent<Cam>().CurrentSector.GetComponent<Sector>().CheckSectors.Count; ++i)
        {
            GameObject Check = Camera.main.GetComponent<Cam>().CurrentSector.GetComponent<Sector>().CheckSectors[i];

            Check.GetComponent<Sector>().CheckSector();

            Check.GetComponent<Collider>().enabled = true;
        }

        VisitedSector.Clear();

        GetSector(Camera.main.GetComponent<Cam>().planes, Camera.main.GetComponent<Cam>().CurrentSector);
    }

    public void GetSector(Plane[] APlanes, GameObject BSector)
    {
        CamPoint = Camera.main.transform.position;

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

            if (d < -0.2f)
            {
                continue;
            }

            if (VisitedSector.Contains(p.GetComponent<Portal>().TargetSector) && d < 0f)
            {
                continue;
            }

            if (d < 1f)
            {
                GetSector(Camera.main.GetComponent<Cam>().planes, p.GetComponent<Portal>().TargetSector);
                continue;
            }

            if (GeometryUtility.TestPlanesAABB(APlanes, p.GetComponent<Renderer>().bounds))
            {
                p.GetComponent<Portal>().SetPlanes();

                GetSector(p.GetComponent<Portal>().planes, p.GetComponent<Portal>().TargetSector);
            }
        }
    }
}
