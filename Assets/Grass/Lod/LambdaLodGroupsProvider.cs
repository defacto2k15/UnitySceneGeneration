using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Container;
using Assets.Utils;
using UnityEngine;

namespace Assets.Grass.Lod
{
    class LambdaLodGroupsProvider : ILodGroupsProvider
    {
        private readonly Func<Vector3, Vector2, int, IGrassSplat> _func;

        public LambdaLodGroupsProvider(Func<Vector3, Vector2, int, IGrassSplat> func)
        {
            this._func = func;
        }

        public IGrassSplat GenerateGrassSplat(Vector3 position, Vector2 size, int lodLevel)
        {
            return _func(position, size, lodLevel);
        }

        public LodGroup GenerateLodGroup(MapAreaPosition position, int newLodLevel)
        {
            throw new NotImplementedException(); //todo
        }
    }
}
