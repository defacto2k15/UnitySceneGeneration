﻿using Assets.Utils;
using UnityEngine;

namespace Assets.Grass.Generating
{
    class SingleGrassUniformPositionProvider : IEntityPositionProvider
    {
        public void SetPosition(GrassEntitiesSet aGrass, MapAreaPosition globalPosition)
        {
            var randomNormalized = new Vector3(Random.value, 0, Random.value);
            aGrass.Position = MyMathUtils.MultiplyMembers(randomNormalized,new Vector3(globalPosition.Size.x, 1, globalPosition.Size.y)) + globalPosition.DownLeft;
        }
    }
}