using Assets.Utils;

namespace Assets.Grass.Lod
{
    internal class LodEntitySplat
    {
        private readonly MapAreaPosition _position;
        private readonly IEntityLodResolver _entityLodResolver;
        private readonly IEntitySplatGenerator _splatGenerator;
        private IGrassSplat _splat;
        private int _entityLodLevel;

        public LodEntitySplat(MapAreaPosition position, IEntityLodResolver entityLodResolver, IEntitySplatGenerator splatGenerator, int entityLodLevel)
        {
            this._position = position;
            this._entityLodResolver = entityLodResolver;
            this._splatGenerator = splatGenerator;
            this._entityLodLevel = entityLodLevel;

            _splat = splatGenerator.GenerateSplat(position, entityLodLevel);
        }

        public void UpdateLod(int newLod)
        {
            int entityLod = _entityLodResolver.GetEntityLod(newLod);
            if (_entityLodLevel != entityLod)
            {
                _entityLodLevel = entityLod;
                _splat.Remove();
                _splat = _splatGenerator.GenerateSplat(_position, _entityLodLevel);
            }
        }
    }
}