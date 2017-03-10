using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Grass
{
    internal class GrassGeneratorObj : MonoBehaviour
    {
        private void Start()
        {
            var grassList = new List<GrassTuft>();
            string shaderName = "Custom/testSurfaceShaderInstanced";
            Material material = new Material(Shader.Find(shaderName));

            GrassTuftGenerator generator = new GrassTuftGenerator();
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    var newObject = generator.CreateGrassTuft(material);
                    newObject.Position = new Vector3(i / 4.0f, 0, j / 4.0f);
                    grassList.Add(newObject);
                }
            }

            MaterialPropertyBlock props = new MaterialPropertyBlock();
            MeshRenderer renderer;

            foreach (var grassObj in grassList)
            {
                float r = Random.Range(0.0f, 1.0f);
                float g = Random.Range(0.0f, 1.0f);
                float b = Random.Range(0.0f, 1.0f);
                props.SetColor("_Color", new Color(r, g, b));

                renderer = grassObj.gameObj.GetComponent<MeshRenderer>();
                renderer.SetPropertyBlock(props);
            }

            
            //GrassTuftGenerator.CreateGrassTuft().Position = new Vector3(0, 0, 0);
        }
    }
}
