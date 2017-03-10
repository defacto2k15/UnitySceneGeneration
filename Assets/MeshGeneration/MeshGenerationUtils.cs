using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.MeshGeneration
{
    class MeshGenerationUtils
    {
        public static int[] makeTrianglesDoubleSided(int []oldTriangles)
        {
            Preconditions.Assert(oldTriangles.Length%3==0, string.Format("Triangle array has length {0}, which is not divisible by 3 ",oldTriangles.Length));
            var newTriangles = new int[oldTriangles.Length*2];
            Array.Copy(oldTriangles, newTriangles, oldTriangles.Length);
            for (var i = 0; i < oldTriangles.Length/3; i++)
            {
                newTriangles[oldTriangles.Length + i*3] = oldTriangles[i*3];
                newTriangles[oldTriangles.Length + i*3+1] = oldTriangles[i*3+2];
                newTriangles[oldTriangles.Length + i*3+2] = oldTriangles[i*3+1];
            }
            return newTriangles;
        }
    }
}
