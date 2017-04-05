using Assets.Utils;

namespace Assets.Grass.Lod
{
    internal interface IEntitySplatGenerator
    {
        IGrassSplat GenerateSplat(MapAreaPosition position, int entityLodLevel);
    }
}