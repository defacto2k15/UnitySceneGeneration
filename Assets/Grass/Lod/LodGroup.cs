using System.Collections.Generic;
using Assets.Utils;
using UnityEngine;

namespace Assets.Grass.Lod
{
    internal class LodGroup
    {
        private readonly List<LodEntitySplat> _lodEntitySplats;
        private int _lodLevel;
        private readonly MapAreaPosition _position;

        public LodGroup(List<LodEntitySplat> lodEntitySplats, int lodLevel, MapAreaPosition position)
        {
            this._lodEntitySplats = lodEntitySplats;
            _lodLevel = lodLevel;
            _position = position;
        }

        public int LodLevel
        {
            get { return _lodLevel; }
        }

        public MapAreaPosition Position
        {
            get { return _position; }
        }

        public void UpdateLod( int newLod)
        {
            Debug.Log(string.Format("LodGroup of position {0} and lodLevel {1} changing lod to {2}", _position.ToString(), _lodLevel, newLod));
            _lodLevel = newLod;
            foreach (var lodEntitySplat in _lodEntitySplats)
            {
                lodEntitySplat.UpdateLod(newLod);
            }
        }
    }
}