using System.Collections.Generic;
using Assets.Grass.Container;
using Assets.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.Grass.Instancing
{
    class GameObjectGrassInstanceGenerator
    {
        public List<GameObject> Generate(GrassEntitiesWithMaterials grassEntitiesWithMaterials)
        {
            var gameObjects = new List<GameObject>();

            foreach (var aGrass in grassEntitiesWithMaterials.Entities)
            {
                var grassInstance = new GameObject("grassInstance");
                grassInstance.AddComponent<MeshFilter>().mesh = grassEntitiesWithMaterials.Mesh;
                grassInstance.transform.localPosition = aGrass.Position;
                grassInstance.transform.localEulerAngles = MyMathUtils.RadToDeg(aGrass.Rotation);
                grassInstance.transform.localScale = aGrass.Scale;
                var rend = grassInstance.AddComponent<MeshRenderer>();
                rend.material = grassEntitiesWithMaterials.Material;

                foreach (var uniform in aGrass.GetUniforms())
                {
                    uniform.Set(rend.material);
                }
                gameObjects.Add(grassInstance);               
            }

            return gameObjects;
        }
    }
}