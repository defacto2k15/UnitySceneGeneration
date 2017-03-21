using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.Grass.Container
{
    class GpuInstancingGrassSplat : IGrassSplat
    {
        private readonly SplatInfo _splatInfo;
        private readonly int _packId;
        private readonly GpuInstancingGrassInstanceContainer _gpuInstancingGrassInstanceContainer;

        public GpuInstancingGrassSplat(SplatInfo splatInfo, int packId, GpuInstancingGrassInstanceContainer gpuInstancingGrassInstanceContainer)
        {
            _splatInfo = splatInfo;
            _packId = packId;
            _gpuInstancingGrassInstanceContainer = gpuInstancingGrassInstanceContainer;
        }

        public void Remove()
        {
            _gpuInstancingGrassInstanceContainer.RemoveSplat(_splatInfo, _packId);
        }
    }
}
