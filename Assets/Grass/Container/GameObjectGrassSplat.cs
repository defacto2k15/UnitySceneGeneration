using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Grass.Container
{
    class GameObjectGrassSplat : IGrassSplat
    {
        private readonly int _splatId;
        private readonly GameObjectGrassInstanceContainer _container;

        public GameObjectGrassSplat(int splatId, GameObjectGrassInstanceContainer container)
        {
            _splatId = splatId;
            _container = container;
        }

        public void Remove()
        {
            _container.RemoveSplat(_splatId);
        }
    }
}
