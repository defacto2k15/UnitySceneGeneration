using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Instancing;
using UnityEngine;

namespace Assets.Grass.Container
{
    class GameObjectGrassInstanceContainer : IGrassInstanceContainer
    {
        private readonly GameObjectGrassInstanceGenerator _generator = new GameObjectGrassInstanceGenerator();
        private readonly List<GameObject> _gameObjects = new List<GameObject>();

        public void Draw()
        {
        }

        public GrassSplat AddGrassEntities(GrassEntitiesWithMaterials grassEntitiesWithMaterials)
        {
            _gameObjects.AddRange(_generator.Generate(grassEntitiesWithMaterials));
            return new GrassSplat();
        }

        public void SetGlobalColor(string name, Color value)
        {
            foreach( var aObject in _gameObjects)
            {
                aObject.GetComponent<Renderer>().material.SetColor(name, value);
            }
        }

        public void SetGlobalUniform(string name, float value)
        {
            foreach (var aObject in _gameObjects)
            {
                aObject.GetComponent<Renderer>().material.SetFloat(name, value);
            }
        }
    }
}
