using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Container;
using Assets.Grass.Instancing;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Assets.Grass
{
    internal class GrassGeneratorObj : MonoBehaviour
    {
        public bool shouldUseInstancing = true;
        private IGrassInstanceContainer _grassInstanceContainer;

        private void Start()
        {
            Random.InitState(DateTime.Now.Second);

            string shaderName;
            if (shouldUseInstancing)
            {
                _grassInstanceContainer = new GpuInstancingGrassInstanceContainer();
                shaderName = "Custom/testSurfaceShader23.Instanced";
            }
            else
            {
                _grassInstanceContainer = new GameObjectGrassInstanceContainer();
                shaderName = "Custom/testSurfaceShader23" ;
            }

            var material = new Material(Shader.Find(shaderName));
            var entitiesGenerator = new GrassEntityGenerator();

            var grassSplat = _grassInstanceContainer.AddGrassEntities(entitiesGenerator.GenerateUniformRectangeSingleGrass(material, new Vector2(100, 200)));

            //_grassInstanceContainer.SetGlobalColor("_Color", new Color(0.4f, 0.4f, 0.4f));
        }

        static float a = 0;
        static float angl = 0;
        private static float windStrength = 0;

        private void Update()
        {

            if (Input.GetKey(KeyCode.UpArrow))
            {
                _grassInstanceContainer.SetGlobalColor("_Color", new Color( a%1.0f, (a +0.5f )%1.0f, (a+0.3f)%1.0f));
                a += 0.1f;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                _grassInstanceContainer.SetGlobalUniform("_BendingStrength", windStrength);
                windStrength += 0.1f;
                windStrength = Mathf.Clamp01(windStrength);
            }
            if (Input.GetKey(KeyCode.W))
            {
                _grassInstanceContainer.SetGlobalUniform("_BendingStrength", windStrength);
                windStrength -= 0.1f;
                windStrength = Mathf.Clamp01(windStrength);
            }

            _grassInstanceContainer.Draw();
        }

        Vector4 GetWindVector(float angle)
        {
            return new Vector4((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
        }
    }
}
