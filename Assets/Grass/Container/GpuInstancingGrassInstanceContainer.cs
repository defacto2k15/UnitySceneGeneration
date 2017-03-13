using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Grass.Container
{
    class GpuInstancingGrassInstanceContainer : IGrassInstanceContainer
    {
        private readonly Mesh _mesh;
        private readonly Material _material;
        private readonly List<GrassPack> _grassPacks;

        public GpuInstancingGrassInstanceContainer(Mesh mesh, Material material, Matrix4x4[] maticesArray, List<UniformArray> uniformArrays )
        {
            _grassPacks = new List<GrassPack>();
            for (var i = 0; i < Math.Ceiling((float) maticesArray.Length/ Constants.MaxInstancesPerPack); i++)
            {
                var elementsToSkipCount = i*Constants.MaxInstancesPerPack;
                var elementsToTakeCount = Math.Min(Constants.MaxInstancesPerPack,
                    maticesArray.Length - i*Constants.MaxInstancesPerPack);
                var packMaticesArray = maticesArray.Skip(elementsToSkipCount).Take(elementsToTakeCount).ToArray();
                MaterialPropertyBlock block = new MaterialPropertyBlock();
                foreach (var aUniformArray in uniformArrays)
                {
                    aUniformArray.AddToBlock(block, elementsToSkipCount, elementsToTakeCount);
                }
                _grassPacks.Add(new GrassPack(packMaticesArray, block));
            }

            _mesh = mesh;
            _material = material;
        }

        public void Draw()
        {
            foreach (var aGrassPack in _grassPacks)
            {
                Graphics.DrawMeshInstanced(_mesh, 0, _material, aGrassPack.MaticesArray, aGrassPack.InstancesCount, aGrassPack.PropertiesBlock, 
                    aGrassPack.CastShadows, false,0, null );
            }
        }
    }

    class UniformArray
    {
        private string _name;
        private Vector4[] _valuesArray;

        public UniformArray(string name, Vector4[] valuesArray)
        {
            this._name = name;
            _valuesArray = valuesArray;
        }

        public void AddToBlock(MaterialPropertyBlock block, int elementsToSkipCount, int elementsToTakeCount)
        {
            block.SetVectorArray(_name, _valuesArray.Skip(elementsToSkipCount).Take(elementsToTakeCount).ToArray());
        }
    }

    class GrassPack
    {
        public GrassPack( Matrix4x4[] maticesArray, MaterialPropertyBlock propertiesBlock)
        {
            Preconditions.Assert( maticesArray.Length <= Constants.MaxInstancesPerPack,
                String.Format("In grass pack there can be at max {0} elements, but is {1}", Constants.MaxInstancesPerPack, maticesArray.Length));

            CastShadows = ShadowCastingMode.Off;
            MaticesArray = maticesArray;
            InstancesCount = maticesArray.Length;
            PropertiesBlock = propertiesBlock;
        }

        public ShadowCastingMode CastShadows { get; private set; }
        public Matrix4x4[] MaticesArray { get; private set; }
        public int InstancesCount { get; private set; }
        public MaterialPropertyBlock PropertiesBlock { get; private set; }
    }
}
