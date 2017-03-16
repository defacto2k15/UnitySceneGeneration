
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Generating;
using Assets.Utils;
using UnityEngine;

namespace Assets.Grass
{
    class GrassEntityGenerator
    {
        private readonly GrassMeshGenerator _generator = new GrassMeshGenerator();
        private readonly GrassTuftGenerator _tuftGenerator = new GrassTuftGenerator();
        private readonly GrassSingleGenerator _singleGenerator = new GrassSingleGenerator();

        public GrassEntitiesWithMaterials Generate(Material material)
        {
            var mesh = _generator.GetGrassBladeMesh(3);
            var outList = new List<GrassEntity>();
            outList.AddRange(_tuftGenerator.CreateGrassTuft().Entities);
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 30; j++)
                {
                    var entitiesSet = _tuftGenerator.CreateGrassTuft();
                    entitiesSet.TranslateBy(new Vector3(i * 2, 0, j * 2));
                    outList.AddRange(entitiesSet.Entities);
                }
            }

            return new GrassEntitiesWithMaterials(outList, material, mesh);
        }

        public GrassEntitiesWithMaterials GenerateUniformRectangleTufts(Material material, Vector2 rectangeSize)
        {
            var mesh = _generator.GetGrassBladeMesh(3);
            var outList = new List<GrassEntity>();
            var tuftCount = 50; //todo do sth with it
            for (var i = 0; i < tuftCount; i++)
            {
                var entitiesSet = _tuftGenerator.CreateGrassTuft();
                Vector3 randomNormalizedPos = RandomGrassDistributionGenerator.GenerateRandomPosition();
                var randomScaledPos = new Vector3(randomNormalizedPos.x*rectangeSize.x, 0,
                    randomNormalizedPos.z*rectangeSize.y);
                entitiesSet.TranslateBy(randomScaledPos);
                outList.AddRange(entitiesSet.Entities);
            }

            return new GrassEntitiesWithMaterials(outList, material, mesh);
        }

        public GrassEntitiesWithMaterials GenerateUniformRectangeSingleGrass(Material material, Vector2 rectangeSize)
        {
            var mesh = _generator.GetGrassBladeMesh(3); // todo do sth
            var outList = new List<GrassEntity>();
            var singleCount = 50;
            for (var i = 0; i < singleCount; i++)
            {
                var entitiesSet = _singleGenerator.CreateSingleGrass();
                Vector3 randomNormalizedPos = RandomGrassDistributionGenerator.GenerateRandomPosition();
                var randomScaledPos = new Vector3(randomNormalizedPos.x*rectangeSize.x, 0,
                    randomNormalizedPos.z*rectangeSize.y);
                entitiesSet.TranslateBy(randomScaledPos);
                outList.AddRange(entitiesSet.Entities);
            }
            return new GrassEntitiesWithMaterials(outList, material, mesh);
        }
    }

    internal class RandomGrassDistributionGenerator //todo to own file
    {
        public static Vector3 GenerateRandomPosition()
        {
            return new Vector3( Random.value, 0, Random.value);
        }
    }
}
