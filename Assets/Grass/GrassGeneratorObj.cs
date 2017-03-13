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
        private GameObject testObject ;
        private List<GrassInstance> _grassList;
        private Material _material;

        public bool shouldUseInstancing = true;
        private IGrassInstanceContainer _grassInstanceContainer;
        private IGrassInstanceGenerator _grassInstanceGenerator;

        private void Start()
        {
            string shaderName;
            if (shouldUseInstancing)
            {
                _grassInstanceGenerator = new GpuInstancingGrassInstanceGenerator();
                shaderName = "Custom/testSurfaceShaderInstanced";
            }
            else
            {
                _grassInstanceGenerator = new GameObjectGrassInstanceGenerator();
                shaderName = "Custom/testSurfaceShader23" ;
            }

            var material = new Material(Shader.Find(shaderName));

            _grassInstanceContainer = _grassInstanceGenerator.Generate( material);

        }

        private void Update()
        {
            _grassInstanceContainer.Draw();
        }
    }
}
