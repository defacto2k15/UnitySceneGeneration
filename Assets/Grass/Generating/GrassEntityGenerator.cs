
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Generating;
using Assets.Grass.Placing;
using Assets.Utils;
using UnityEngine;

namespace Assets.Grass
{
    class GrassEntityGenerator
    {
        private readonly GrassMeshGenerator _generator = new GrassMeshGenerator();
        private readonly GrassTuftGenerator _tuftGenerator = new GrassTuftGenerator();
        private readonly GrassSingleGenerator _singleGenerator = new GrassSingleGenerator();

        public GrassEntitiesWithMaterials GenerateUniformRectangleTufts(Material material, IGrassPlacer placer)
        {
            var mesh = _generator.GetGrassBladeMesh(1);
            var outList = new List<GrassEntity>();
            var tuftCount = 50; //todo do sth with it
            for (var i = 0; i < tuftCount; i++)
            {
                var entitiesSet = _tuftGenerator.CreateGrassTuft();
                placer.Set(entitiesSet);
                outList.AddRange(entitiesSet.Entities);
            }

            return new GrassEntitiesWithMaterials(outList, material, mesh);
        }

        public GrassEntitiesWithMaterials GenerateUniformRectangeSingleGrass(Material material, IGrassPlacer placer, int lodLevel)
        {
            var mesh = _generator.GetGrassBladeMesh(Mathf.Max(1, 7 - lodLevel)); // todo do sth
            var outList = new List<GrassEntity>();
            var singleCount = 5;
            for (var i = 0; i < singleCount; i++)
            {
                var entitiesSet = _singleGenerator.CreateSingleGrass();
                placer.Set(entitiesSet);
                outList.AddRange(entitiesSet.Entities);
            }
            return new GrassEntitiesWithMaterials(outList, material, mesh);
        }

        public GrassEntitiesWithMaterials GenerateLineGrass(Material material)
        {
            var mesh = _generator.GetGrassBladeMesh(3); // todo do sth
            var outList = new List<GrassEntity>();
            var singleCount = 20;
            var fullLength = 2f;
            for (var i = 0; i < singleCount; i++)
            {
                var entitiesSet = _singleGenerator.CreateSingleGrass();
                Vector3 randomNormalizedPos = RandomGrassDistributionGenerator.GenerateRandomPosition();
                entitiesSet.TranslateBy(new Vector3(i*fullLength/singleCount, 0, 0));
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
