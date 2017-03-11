using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Assets.Grass
{
    internal class GrassGeneratorObj : MonoBehaviour
    {
        private GameObject testObject ;
        private List<GrassTuft> _grassList;
        private Material _material;

        private void Start()
        {
            testObject = new GameObject();
            _grassList = new List<GrassTuft>();
            string shaderName = "Custom/testSurfaceShaderInstanced";
            _material = new Material(Shader.Find(shaderName));

            GrassTuftGenerator generator = new GrassTuftGenerator();
            for (int i = 0; i < 99; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    var newObject = generator.CreateGrassTuft(_material);
                    newObject.Position = new Vector3(i / 4.0f, 0, j / 4.0f);
                    _grassList.Add(newObject);
                }
            }   
            //GrassTuftGenerator.CreateGrassTuft().Position = new Vector3(0, 0, 0);
            maticesArray = new Matrix4x4[_grassList.Count];
            Vector4[] colorVector4 = new Vector4[_grassList.Count];
            mesh = _grassList[0].Mesh;

            for (int i = 0; i < _grassList.Count; i++)
            {
                maticesArray[i] = _grassList[i].LocalToWorldMatrix;
                colorVector4[i] = new Vector4((1.0f + i*0.05f)%1.0f, 0.5f, 0.5f, 0.5f);
            }
            block = new MaterialPropertyBlock();

            block.SetVectorArray("_Color", colorVector4);
            //block.SetColor("_Color", new Color(1.0f, 0.5f, 0.5f, 0.5f));
        }

        private MaterialPropertyBlock block;
        private Matrix4x4[] maticesArray;
        private Mesh mesh;

        private void Update()
        {
            var castShadows = ShadowCastingMode.Off;
            Graphics.DrawMeshInstanced(mesh, 0, _material, maticesArray, maticesArray.Length, block, castShadows, true, 0, null);
            //Graphics.DrawMesh(mesh,maticesArray[0], _material,0,null,0,block);
            //for (int i = 0; i < _grassList.Count; i++)
            //{
            //    var matrix = new[] {_grassList[i].LocalToWorldMatrix};
            //    //Graphics.DrawMeshInstanced(mesh, 0, _material, matrix, matrix.Length, null, castShadows, true, 0, null);
            //    Graphics.DrawMesh(mesh, matrix[0], _material, 0);
            //}
            //Graphics.DrawMeshInstanced(_grassList[0].Mesh, 0, _material, maticesArray, maticesArray.Count());
        }
    }
}
