using UnityEngine;

namespace Assets.Grass.Container
{
    interface IGrassInstanceContainer
    {
        void Draw();
        IGrassSplat AddGrassEntities(GrassEntitiesWithMaterials grassEntitiesWithMaterials);
        void SetGlobalColor(string name, Color value);
        void SetGlobalUniform(string name, float strength);
    }
}