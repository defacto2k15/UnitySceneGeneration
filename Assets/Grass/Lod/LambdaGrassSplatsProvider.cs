using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Container;
using UnityEngine;

namespace Assets.Grass.Lod
{
    class LambdaGrassSplatsProvider : IGrassSplatsProvider
    {
        private readonly Func<Vector3, Vector2, int, IGrassSplat> _func;

        public LambdaGrassSplatsProvider(Func<Vector3, Vector2, int, IGrassSplat> func)
        {
            this._func = func;
        }

        public IGrassSplat GenerateGrassSplat(Vector3 position, Vector2 size, int lodLevel)
        {
            return _func(position, size, lodLevel);
        }
    }
}
