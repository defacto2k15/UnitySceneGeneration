using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.MeshGeneration;
using UnityEngine;

namespace Assets.Grass
{
    class GrassTuftGenerator
    {
        GrassMeshGenerator generator = new GrassMeshGenerator();

        public  GrassTuft CreateGrassTuft(Material material)
        {
            //var tuftObj = new GameObject {name = "grassTuft"};
            Mesh mesh = generator.GetGrassBladeMesh(2);

            return new GrassTuft(mesh);
        }
    }
}
