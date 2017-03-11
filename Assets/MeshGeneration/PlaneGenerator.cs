using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.MeshGeneration
{
    class PlaneGenerator
    {
        public static void createPlaneMeshFilter(MeshFilter filter, float length, float width, float[,] heightArray)
        {
            Mesh mesh = filter.mesh;
            mesh.Clear();

            int resX = heightArray.GetLength(0);
            int resZ = heightArray.GetLength(1);

            #region Vertices 
            //todo remove regions to refactor to methods when resharper is present!
            Vector3[] vertices = new Vector3[resX * resZ];
            for (int z = 0; z < resZ; z++)
            {
                // [ -length / 2, length / 2 ]
                float zPos = ((float)z / (resZ - 1) ) * length;
                for (int x = 0; x < resX; x++)
                {
                    // [ -width / 2, width / 2 ]
                    float xPos = ((float)x / (resX - 1) ) * width;
                    vertices[x + z * resX] = new Vector3(xPos, heightArray[x, z], zPos);
                }
            }

            Vector3[] normales = new Vector3[vertices.Length];
            for (int n = 0; n < normales.Length; n++)
                normales[n] = Vector3.up;
            #endregion

            #region UVs
            Vector2[] uvs = new Vector2[vertices.Length];
            for (int v = 0; v < resZ; v++)
            {
                for (int u = 0; u < resX; u++)
                {
                    uvs[u + v * resX] = new Vector2((float)u / (resX - 1), (float)v / (resZ - 1));
                }
            }
            #endregion

            #region Triangles
            int nbFaces = (resX - 1) * (resZ - 1);
            int[] triangles = new int[nbFaces * 6];
            int t = 0;
            for (int face = 0; face < nbFaces; face++)
            {
                // Retrieve lower left corner from face ind
                int i = face % (resX - 1) + (face / (resX - 1) * resX);

                triangles[t++] = i + resX;
                triangles[t++] = i + 1;
                triangles[t++] = i;

                triangles[t++] = i + resX;
                triangles[t++] = i + resX + 1;
                triangles[t++] = i + 1;
            }
            #endregion

            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            ;
        }
    }
}
