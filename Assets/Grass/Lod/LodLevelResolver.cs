using UnityEngine;

namespace Assets.Grass.Lod
{
    class LodLevelResolver
    {
        private readonly int _maxLodLevel;
        private readonly int _singleLevelDistance;
        private readonly int _noChangeMargin;

        public LodLevelResolver(int maxLodLevel, int singleLevelDistance, int noChangeMargin)
        {
            this._maxLodLevel = maxLodLevel;
            this._singleLevelDistance = singleLevelDistance;
            this._noChangeMargin = noChangeMargin;
        }

        public int Resolve(Vector3 cameraPosition, Vector3 splatPosition, int oldLodLevel = -1)
        {
            var distance = Vector3.Distance(cameraPosition, splatPosition);
            var newLod = (int)Mathf.Floor(distance/_singleLevelDistance);
            if (oldLodLevel == -1)
            {
                return (int)Mathf.Min(newLod, _maxLodLevel);
            } else if (newLod == oldLodLevel)
            {
                return (int)Mathf.Min(newLod, _maxLodLevel); ;
            } else if (Mathf.Abs(oldLodLevel*_singleLevelDistance - distance) < _noChangeMargin)
            {
                return oldLodLevel;
            }
            else
            {
                return (int)Mathf.Min(newLod, _maxLodLevel); ;
            }
        }
    }
}