using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Container;
using Assets.Grass.Lod;
using Assets.Utils;
using UnityEngine;

namespace Assets.Grass.Generating
{
    class SingleGrassEntitySplatGenerator : IEntitySplatGenerator
    {
        private readonly GrassSingleGenerator _entityGenerator;
        private readonly IEntityPositionProvider _positionProvider;
        private readonly GrassSingleSettingGenerator _grassSingleSettingGenerator;
        private readonly IGrassInstanceContainer _grassInstanceContainer;
        private readonly GrassMeshGenerator _meshGenerator;
        private readonly Material _material;

        public SingleGrassEntitySplatGenerator(GrassSingleGenerator entityGenerator, IEntityPositionProvider positionProvider, GrassSingleSettingGenerator grassSingleSettingGenerator, IGrassInstanceContainer grassInstanceContainer, GrassMeshGenerator meshGenerator, Material material)
        {
            this._entityGenerator = entityGenerator;
            this._positionProvider = positionProvider;
            this._grassSingleSettingGenerator = grassSingleSettingGenerator;
            this._grassInstanceContainer = grassInstanceContainer;
            _meshGenerator = meshGenerator;
            this._material = material;
        }

        public IGrassSplat GenerateSplat(MapAreaPosition position, int entityLodLevel)
        {
            List<GrassEntitiesSet> singleEntities = new List<GrassEntitiesSet>();
            for (int i = 0; i < 1000; i++)
            {
                var aGrass = _entityGenerator.CreateSingleGrass();
                _positionProvider.SetPosition(aGrass, position);
                _grassSingleSettingGenerator.SetSettings(aGrass);
                if (entityLodLevel == 0)
                {
                    aGrass.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._DbgColor, new Vector4(99.0f, 0f, 0.0f, 1.0f))); 
                }
                if (entityLodLevel == 1)
                {
                    aGrass.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._DbgColor, new Vector4(99.0f, 99.0f, 0.0f, 1.0f)));
                }
                if (entityLodLevel == 2)
                {
                    aGrass.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._DbgColor, new Vector4(99.0f, 99.0f, 99.0f, 1.0f)));
                }
                if (entityLodLevel == 3)
                {
                    aGrass.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._DbgColor, new Vector4(0.0f, 99.0f, 0.0f, 1.0f)));
                }
                if (entityLodLevel == 4)
                {
                    aGrass.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._DbgColor, new Vector4(0.0f, 99.0f, 99.0f, 1.0f)));
                }
                if (entityLodLevel == 5)
                {
                    aGrass.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._DbgColor, new Vector4(0.0f, 0.0f, 99.0f, 1.0f)));
                }
                
                singleEntities.Add(aGrass);
            }
            var bladeTriangleCount = (int)Mathf.Max(Constants.MAX_GRASS_MESH_LEVEL - entityLodLevel, Constants.MIN_GRASS_MESH_LEVEL);
            Mesh mesh = _meshGenerator.GetGrassBladeMesh(bladeTriangleCount);
            return _grassInstanceContainer.AddGrassEntities(
                new GrassEntitiesWithMaterials(singleEntities.SelectMany(c => c.EntitiesAfterTransform).ToList(), _material, mesh,
                    ContainerType.Instancing));
        }
    }
}
