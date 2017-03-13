using Assets.Grass.Container;
using UnityEngine;

namespace Assets.Grass.Instancing
{
    interface IGrassInstanceGenerator
    {
        IGrassInstanceContainer Generate(Material material);
    }
}