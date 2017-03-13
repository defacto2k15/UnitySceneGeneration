using System.Collections.Generic;
using Assets.Grass.Container;
using UnityEngine;

namespace Assets.Grass.Instancing
{
    class GameObjectGrassInstanceGenerator : IGrassInstanceGenerator
    {
        GrassMeshGenerator generator = new GrassMeshGenerator();

        public IGrassInstanceContainer Generate(Material material)
        {
            var mesh = generator.GetGrassBladeMesh(2);
            List<GameObject> gameObjects = new List<GameObject>();

            var grassInstance = new GameObject("grassInstance");
            grassInstance.AddComponent<MeshFilter>().mesh =  generator.GetGrassBladeMesh(2);;
            var rend = grassInstance.AddComponent<MeshRenderer>();
            rend.material = material;
            gameObjects.Add(grassInstance);

            return new GameObjectGrassInstanceContainer(gameObjects);
        }
    }
}