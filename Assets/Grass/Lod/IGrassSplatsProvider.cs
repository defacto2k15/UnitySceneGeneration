using UnityEngine;

namespace Assets.Grass.Lod
{
    interface IGrassSplatsProvider
    {
        IGrassSplat GenerateGrassSplat(Vector3 position, Vector2 size, int lodLevel);
    }
}
