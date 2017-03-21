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
        private readonly Dictionary<int, List<GameObject>> _gameObjectSplats = new Dictionary<int, List<GameObject>>();
        private int _lastSplatId = 0;

        public void Draw()
        {
        }

        public IGrassSplat AddGrassEntities(GrassEntitiesWithMaterials grassEntitiesWithMaterials)
        {
            _lastSplatId++;
            _gameObjectSplats[_lastSplatId] = _generator.Generate(grassEntitiesWithMaterials);
            return new GameObjectGrassSplat(_lastSplatId, this);
        }

        public void SetGlobalColor(string name, Color value)
        {
            ForeachObject((aObject) => aObject.GetComponent<Renderer>().material.SetColor(name, value));
        }

        public void SetGlobalUniform(string name, float value)
        {
            ForeachObject((aObject) => aObject.GetComponent<Renderer>().material.SetFloat(name, value));
        }

        private void ForeachObject(Action<GameObject> action)
        {
            foreach (var pair in _gameObjectSplats)
            {
                foreach (var obj in pair.Value)
                {
                    action(obj);
                }
            }
        }

        public void RemoveSplat(int splatId)
        {
            _gameObjectSplats.Remove(splatId);
        }
    }
}
