using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Container;
using UnityEngine;

namespace Assets.Grass.Lod
{
    class GrassLodManager
    {
        private readonly List<GrassSplatWithPosition> _grassSplatsList = new List<GrassSplatWithPosition>();
        private readonly LodLevelResolver _lodLevelResolver;
        private readonly IGrassSplatsProvider _grassSplatsProvider;
        private Vector2 _terrainSize;
        private Vector2 _splatSize;

        public GrassLodManager(LodLevelResolver lodLevelResolver, IGrassSplatsProvider grassSplatsProvider, Vector2 terrainSize, Vector2 splatSize)
        {
            _lodLevelResolver = lodLevelResolver;
            _grassSplatsProvider = grassSplatsProvider;
            _terrainSize = terrainSize;
            _splatSize = splatSize;
        }

        public void UpdateLod(Vector3 cameraPosition)
        {
            if (_grassSplatsList.Count == 0)
            {
                InitializeSplats(cameraPosition);
            }
            else
            {
                UpdateLodLevels(cameraPosition);
            }
        }

        private void InitializeSplats(Vector3 cameraPosition)
        {
            int splatXCount = (int) (_terrainSize.x/_splatSize.x);
            int splatYCount = (int) (_terrainSize.y/_splatSize.y);

            for (int x = 0; x < splatXCount; x++)
            {
                for (int y = 0; y < splatYCount; y++)
                {
                    var splatDownLeftPoint = new Vector3(_splatSize.x*(x), 0,
                        _splatSize.y*(y));
                    var splatCenter = new Vector3(_splatSize.x*(x + 0.5f), 0,
                        _splatSize.y*(y + 0.5f)); //todo set y when have heightmap!
                    var lodLevel = _lodLevelResolver.Resolve(cameraPosition, splatCenter);
                    IGrassSplat splat = _grassSplatsProvider.GenerateGrassSplat(splatDownLeftPoint, _splatSize, lodLevel);
                    _grassSplatsList.Add(new GrassSplatWithPosition(splatCenter, splat, lodLevel, _splatSize));
                }
            }
        }

        private void UpdateLodLevels(Vector3 cameraPosition)
        {
            for (int i = 0; i < _grassSplatsList.Count; i++)
            {
                var oldSplat = _grassSplatsList[i];
                var newLodLevel = _lodLevelResolver.Resolve(cameraPosition, oldSplat.SplatPosition, oldSplat.LodLevel);
                if (oldSplat.LodLevel != newLodLevel)
                {
                    IGrassSplat newSplat = _grassSplatsProvider.GenerateGrassSplat(oldSplat.DownLeftPoint, oldSplat.SplatSize, newLodLevel);
                    _grassSplatsList[i] = new GrassSplatWithPosition(oldSplat.SplatPosition, newSplat, newLodLevel, oldSplat.SplatSize);
                }
            }
        }
    }

    internal class GrassSplatWithPosition
    {
        private readonly Vector3 _splatPosition;
        private readonly IGrassSplat _splat;
        private readonly int _lodLevel;
        private readonly Vector2 _splatSize;

        public GrassSplatWithPosition(Vector3 splatPosition, IGrassSplat splat, int lodLevel, Vector2 splatSize)
        {
            _splatPosition = splatPosition;
            _splat = splat;
            _lodLevel = lodLevel;
            _splatSize = splatSize;
        }

        public Vector3 SplatPosition
        {
            get { return _splatPosition; }
        }

        public IGrassSplat Splat
        {
            get { return _splat; }
        }

        public int LodLevel
        {
            get { return _lodLevel; }
        }

        public Vector2 SplatSize
        {
            get { return _splatSize; }
        }

        public Vector3 DownLeftPoint
        {
            get
            {
                return SplatPosition-new Vector3(SplatSize.x, 0, SplatSize.y);
            }
        }
    }
}
