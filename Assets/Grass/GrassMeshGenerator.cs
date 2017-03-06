using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.MeshGeneration;
using UnityEngine;

namespace Assets.Grass
{
    class GrassMeshGenerator
    {
        public static void GenerateGrassBladeMesh(MeshFilter filter, int turfCount)
        {
            Mesh mesh = filter.mesh;
            mesh.Clear();

            var sw= 1.0f; //standard width
            var sh= 1.0f; //standard height
            var df= 0.5f; //decrease factor

            Vector3[] vertices = new Vector3[]
            {
                new Vector3(0, 0),
                new Vector3(sw, 0, 0),
                new Vector3(sw*(1 - df)/2, sh),
                new Vector3(sw*(1 + df)/2, sh),
                new Vector3(sw*(1 - df*df)/2, sh*2),
                new Vector3(sw*(1 + df*df)/2, sh*2),
                new Vector3(sw/2, sh*3),
            };


            Vector3[] normales = new Vector3[vertices.Length];

            Vector2[] uvs = new Vector2[vertices.Length];

            int[] triangles = new int[]
            {
                0,1,3,
                0,3,2,
                2,3,5,
                2,5,4,
                4,5,6
            };
            triangles = MeshGenerationUtils.makeTrianglesDoubleSided(triangles);

            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.Optimize();
        }
    }
}
