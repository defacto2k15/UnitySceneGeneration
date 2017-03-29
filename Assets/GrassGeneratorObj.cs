using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Billboard;
using Assets.Grass.Container;
using Assets.Grass.Instancing;
using Assets.Grass.Lod;
using Assets.Grass.Placing;
using Assets.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Assets.Grass
{
    internal class GrassGeneratorObj : MonoBehaviour
    {
        public bool shouldUseInstancing = true;
        private IGrassInstanceContainer _grassInstanceContainer;
        public Material BillboardMaterial;
        public Material ShellMaterial;

        private void Start()
        {
            Random.InitState(DateTime.Now.Second);

            string shaderName;
            if (shouldUseInstancing)
            {
                _grassInstanceContainer = new GpuInstancingGrassInstanceContainer();
                shaderName = "Custom/testSurfaceShader23.Instanced";
                BillboardMaterial.shader = Shader.Find("Custom/BillboardTransparent.Instanced");
               // BillboardMaterial.shader = Shader.Find("Custom/testSurfaceShader23.Instanced");
            }
            else
            {
                _grassInstanceContainer = new GameObjectGrassInstanceContainer();
                shaderName = "Custom/testSurfaceShader23";
            }

            var material = new Material(Shader.Find(shaderName));
            var entitiesGenerator = new GrassEntityGenerator();

            var terrainSize = new Vector2(100, 100);
            var splatSize = new Vector2(10, 10);
            var maxLodLevel = 5;
            var singleLevelDistance = 20;
            var noChangeMargin = 4;
            //_manager = new GrassLodManager(new LodLevelResolver(maxLodLevel, singleLevelDistance, noChangeMargin),
            //    new LambdaGrassSplatsProvider((downLeftPointArg, splatSizeArg, lodLevel) =>
            //        _grassInstanceContainer.AddGrassEntities(entitiesGenerator.GenerateUniformRectangeBillboardGrass(BillboardMaterial,
            //            new UniformRectangleGrassPlacer(
            //                new Vector2(downLeftPointArg.x, downLeftPointArg.z),
            //                new Vector2(downLeftPointArg.x, downLeftPointArg.z) + splatSizeArg), lodLevel))),
            //    terrainSize, splatSize);  


            //grassSplat = _grassInstanceContainer.AddGrassEntities(entitiesGenerator.GenerateUniformRectangeSingleGrass(material,
            //    new UniformRectangleGrassPlacer(Vector2.zero, new Vector2(10, 20)), 0));


            //GrassBillboardGenerator billboardGenerator = new GrassBillboardGenerator();
            //GrassEntitiesWithMaterials bilboardTurf = generateTurf(billboardGenerator);
            //var splat = _grassInstanceContainer.AddGrassEntities(bilboardTurf);

            //_grassInstanceContainer.SetGlobalUniform(GrassShaderUniformName._WindDirection, new Vector4(1,0,0,0).normalized);
            //_grassInstanceContainer.SetGlobalUniform(GrassShaderUniformName._BendingStrength, 0.0f);

            //_manager.UpdateLod(Vector3.zero);

            generateShells();

        }

        private void generateShells()
        {
            var layersCount = 10;
            var grassHeight = 0.2f;
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            for (int i = 0; i < layersCount; i++)
            {
                var shellObj = new GameObject("grassShell " + i);
                shellObj.transform.localPosition = new Vector3(10, (grassHeight/layersCount)* i, 10);
                var rend = shellObj.AddComponent<MeshRenderer>();
                rend.transform.localScale = new Vector3(1,1,1);
                rend.material = ShellMaterial;
                rend.material.SetFloat("_LayerHeight", i *(1 / (float)layersCount));

                rend.material.SetFloat("_BendingStrength", 0.8f);
                rend.material.SetVector("_WindDirection", new Vector4(1,1,1,1));
                shellObj.AddComponent<MeshFilter>().mesh = plane.GetComponent<MeshFilter>().mesh;
            }

        }

        private GrassEntitiesWithMaterials generateTurf(GrassBillboardGenerator billboardGenerator)
        {
            var meshGenerator = new GrassMeshGenerator();
            var mesh = meshGenerator.GetGrassBillboardMesh(0, 1);
            var xgenerateTriangleTurf = billboardGenerator.GenerateTriangleTurf(); //todo : use grass entities set and rotate
            //xgenerateTriangleTurf.Rotation = (MyMathUtils.DegToRad(new Vector3(0, 90, 0)));
            //xgenerateTriangleTurf.Position = new Vector3(2,2,2);
            return  new GrassEntitiesWithMaterials(xgenerateTriangleTurf.Entities, BillboardMaterial, mesh);
        }

        static float a = 0;
        static float angl = 0;
        private static float windStrength = 0;
        private IGrassSplat grassSplat;
        private GrassLodManager _manager;

        private void Update()
        {

            if (Input.GetKey(KeyCode.UpArrow))
            {
                _grassInstanceContainer.SetGlobalColor("_Color", new Color( a%1.0f, (a +0.5f )%1.0f, (a+0.3f)%1.0f));
                a += 0.1f;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                _grassInstanceContainer.SetGlobalUniform(GrassShaderUniformName._BendingStrength, windStrength);
                windStrength += 0.1f;
                windStrength = Mathf.Clamp01(windStrength);
            }
            if (Input.GetKey(KeyCode.W))
            {
                _grassInstanceContainer.SetGlobalUniform(GrassShaderUniformName._BendingStrength, windStrength);
                windStrength -= 0.1f;
                windStrength = Mathf.Clamp01(windStrength);
            }
            if (Input.GetKey(KeyCode.T))
            {
                grassSplat.Remove();
            }

            //if (Time.frameCount%100 == 0)
            //{
             //   _manager.UpdateLod(new Vector3(0, 0, 0));
                
            //}
            if (windStrength < 1)
            {
                //_manager.UpdateLod(new Vector3(0,0,0));
                //windStrength = 2;
            }
            //_grassInstanceContainer.Draw();
        }

        Vector4 GetWindVector(float angle)
        {
            return new Vector4((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
        }
    }
}
