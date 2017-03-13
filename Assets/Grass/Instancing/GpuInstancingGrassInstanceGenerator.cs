using System.Collections.Generic;
using Assets.Grass.Container;
using Assets.Utils;
using UnityEngine;

namespace Assets.Grass.Instancing
{
    class GpuInstancingGrassInstanceGenerator : IGrassInstanceGenerator
    {
        GrassMeshGenerator generator = new GrassMeshGenerator();

        public IGrassInstanceContainer Generate(Material material)
        {
            List<Matrix4x4> maticesList = new List<Matrix4x4>();
            List<UniformArray> uniformArrays = new List<UniformArray>();
            List<Vector4> colorUniformList = new List<Vector4>();

            var mesh = generator.GetGrassBladeMesh(2);
            for (int i = 0; i < 999; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    maticesList.Add(TransformUtils.GetLocalToWorldMatrix(new Vector3(i / 4.0f, 0, j / 4.0f)));
                    colorUniformList.Add( new Vector4((1.0f + i*0.15f)%1.0f, 0.5f, 0.5f, 0.5f));
                }
            }

            UniformArray colorUniformArray = new UniformArray("_Color", colorUniformList.ToArray());
            return new GpuInstancingGrassInstanceContainer(mesh, material, maticesList.ToArray(), new List<UniformArray>(){ colorUniformArray});
        }
    }
}