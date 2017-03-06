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
        public static GrassTuft CreateGrassTuft()
        {
            var tuftObj = new GameObject {name = "grassTuft"};
            GrassMeshGenerator.GenerateGrassBladeMesh(tuftObj.AddComponent<MeshFilter>());

            var renderer = tuftObj.AddComponent<MeshRenderer>();
            renderer.material.shader = Shader.Find("Particles/Additive");
            Texture2D tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, Color.green);
            tex.Apply();
            renderer.material.mainTexture = tex;
            renderer.material.color = Color.green;

            return new GrassTuft(tuftObj);
        }
    }
}
