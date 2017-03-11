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
        private readonly Dictionary<int, Mesh> meshCache = new Dictionary<int, Mesh>(); 

        public static Mesh GenerateGrassBladeMesh( int turfCount)
        {
            var levelsCount = 6;
            Func<float, float> leftOffsetGenerator = (percent) =>
            {
                return 1.0f - (float)Math.Pow(-percent+1.0f, 0.5f);
            };
            Mesh mesh = new Mesh();
            mesh.Clear();

            var sw= 0.3f; //standard width
            var sh= 1.0f; //standard height
            var df= 0.5f; //decrease factor

            int vertexCount = 2*levelsCount + 1;
            Vector3[] vertices = new Vector3[vertexCount];
            Vector2[] uvs = new Vector2[vertices.Length];

            for (int currLev = 0; currLev < levelsCount; currLev++)
            {
                float height = sh*((float) currLev/levelsCount);
                float offsetFromLeft = leftOffsetGenerator((float)currLev / levelsCount)*(sw/2);
                var pos1 = new Vector3(offsetFromLeft, height);
                var pos2 =  new Vector3(sw - offsetFromLeft, height);
                vertices[currLev*2] = pos1;
                vertices[currLev*2 + 1] = pos2;

                uvs[currLev*2] = new Vector2(pos1.x, pos1.y);
                uvs[currLev*2+1] = new Vector2(pos2.x, pos2.y);
            }
            vertices[vertexCount-1] = new Vector3(sw/2, sh);
            uvs[vertexCount-1] = new Vector2(sw/2, sh);


            Vector3[] normales = new Vector3[vertices.Length];


            var trianglesCount = 2*levelsCount + 1;
            int[] triangles = new int[3*trianglesCount];
            for (int currLev = 0; currLev < levelsCount-1; currLev++)
            {
                triangles[currLev*6] = currLev*2;
                triangles[currLev*6+1] = currLev*2+1;
                triangles[currLev*6+2] = currLev*2+3;
                triangles[currLev*6+3] = currLev*2;
                triangles[currLev*6+4] = currLev*2+3;
                triangles[currLev*6+5] = currLev*2+2;
            }
            triangles[3*trianglesCount - 3] = vertexCount-3;
            triangles[3*trianglesCount - 2] = vertexCount-2;
            triangles[3*trianglesCount - 1] = vertexCount-1;


            triangles = MeshGenerationUtils.makeTrianglesDoubleSided(triangles);

            mesh.vertices = vertices;
            mesh.normals = normales;
            mesh.uv = uvs;
            mesh.triangles = triangles;

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            ;
            return mesh;
        }

        public Mesh GetGrassBladeMesh(int turfCount)
        {
            if (!meshCache.ContainsKey(turfCount))
            {
                meshCache[turfCount] = GenerateGrassBladeMesh(turfCount);
            }
            return meshCache[turfCount];
        }
    }
}
