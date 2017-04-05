using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Container;
using Assets.Utils;
using UnityEngine;

namespace Assets.Grass.Lod
{
    class GrassLodManager
    {
        private readonly List<LodGroup> _groupsList = new List<LodGroup>();
        private readonly LodLevelResolver _lodLevelResolver;
        private readonly ILodGroupsProvider _lodGroupsProvider;
        private Vector2 _terrainSize;
        private Vector2 _groupSize;

        public GrassLodManager(LodLevelResolver lodLevelResolver, ILodGroupsProvider lodGroupsProvider, Vector2 terrainSize, Vector2 groupSize)
        {
            _lodLevelResolver = lodLevelResolver;
            _lodGroupsProvider = lodGroupsProvider;
            _terrainSize = terrainSize;
            _groupSize = groupSize;
        }

        public void UpdateLod(Vector3 cameraPosition)
        {
            if (_groupsList.Count == 0)
            {
                InitializeGroups(cameraPosition);
            }
            else
            {
                UpdateLodLevels(cameraPosition);
            }
        }

        private void InitializeGroups(Vector3 cameraPosition)
        {
            int groupXCount = (int) (_terrainSize.x/_groupSize.x);
            int groupYCount = (int) (_terrainSize.y/_groupSize.y);

            for (int x = 0; x < groupXCount; x++)
            {
                for (int y = 0; y < groupYCount; y++)
                {
                    var groupDownLeftPoint = new Vector3(_groupSize.x*(x), 0,
                        _groupSize.y*(y));
                    var groupCenter = new Vector3(_groupSize.x*(x + 0.5f), 0,
                        _groupSize.y*(y + 0.5f)); //todo set y when have heightmap!
                    var lodLevel = _lodLevelResolver.Resolve(cameraPosition, groupCenter);
                    _groupsList.Add(_lodGroupsProvider.GenerateLodGroup(new MapAreaPosition(_groupSize, groupDownLeftPoint) , lodLevel));
                }
            }
        }

        private void UpdateLodLevels(Vector3 cameraPosition)
        {
            foreach (var group in _groupsList)
            {
                var newLodLevel = _lodLevelResolver.Resolve(cameraPosition, group.Position, group.LodLevel);
                if (newLodLevel != group.LodLevel)
                {
                    group.UpdateLod(newLodLevel);
                }
            }
        }
    }
}
