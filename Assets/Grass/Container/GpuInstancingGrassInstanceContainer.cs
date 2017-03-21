using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Instancing;
using UnityEngine;
using UnityEngine.Rendering;

using PackId = System.Int32;

namespace Assets.Grass.Container
{
    class GpuInstancingGrassInstanceContainer : IGrassInstanceContainer
    {
        private readonly Dictionary<SplatInfo, Dictionary<PackId, List<GrassPack>>> _grassPacks = new Dictionary<SplatInfo, Dictionary<PackId, List<GrassPack>>>();
        private PackId lastPackId = 0;

        private readonly GpuInstancingGrassInstanceGenerator _generator = new GpuInstancingGrassInstanceGenerator();

        public IGrassSplat AddGrassEntities(GrassEntitiesWithMaterials grassEntitiesWithMaterials)
        {
            var grassInstancesTemplate = _generator.Generate(grassEntitiesWithMaterials);

            var maticesArray = grassInstancesTemplate.TransformMatices;
            var uniformArrays = grassInstancesTemplate.UniformArrays;

            var newGrassPacks = new List<GrassPack>();

            for (var i = 0; i < Math.Ceiling((float) maticesArray.Length/ Constants.MaxInstancesPerPack); i++)
            {
                var elementsToSkipCount = i*Constants.MaxInstancesPerPack;
                var elementsToTakeCount = Math.Min(Constants.MaxInstancesPerPack,
                    maticesArray.Length - i*Constants.MaxInstancesPerPack);
                var packMaticesArray = maticesArray.Skip(elementsToSkipCount).Take(elementsToTakeCount).ToArray();
                MyMaterialPropertyBlock block = new MyMaterialPropertyBlock(elementsToTakeCount);
                foreach (var aUniformArray in uniformArrays)
                {
                    aUniformArray.AddToBlock(block.Block, elementsToSkipCount, elementsToTakeCount);
                }
                newGrassPacks.Add(new GrassPack(packMaticesArray, block));
            }

            var splatInfo = new SplatInfo(grassEntitiesWithMaterials.Mesh, grassEntitiesWithMaterials.Material);
            if (!_grassPacks.ContainsKey(splatInfo))
            {
                _grassPacks.Add(splatInfo, new Dictionary<PackId, List<GrassPack>>());
            }
            lastPackId++;
            _grassPacks[splatInfo][lastPackId] = newGrassPacks;

            return new GpuInstancingGrassSplat(splatInfo, lastPackId, this);
        }

        public void Draw()
        {
            foreach (var aGrassPack in _grassPacks)
            {
                foreach (var materialMeshPair in _grassPacks)
                {
                    foreach (var packList in materialMeshPair.Value.Values)
                    {
                        foreach (var pack in packList)
                        {
                            Graphics.DrawMeshInstanced(aGrassPack.Key.Mesh, 0, aGrassPack.Key.Material, pack.MaticesArray, pack.InstancesCount, pack.MyBlock.Block,
                                pack.CastShadows, false, 0, null);
                        }
                    }
                }
            }
        }


        public void SetGlobalColor(string name, Color value)
        {
            ForeachObject(aGrassPack => aGrassPack.MyBlock.AddSingleVectorArray(name, value));
        }

        public void SetGlobalUniform(string name, float value)
        {
            ForeachObject(aGrassPack => aGrassPack.MyBlock.AddGlobalUniform(name, value));
        }

        private void ForeachObject(Action<GrassPack> action)
        {
            foreach (var materialMeshPair in _grassPacks)
            {
                foreach (var packList in materialMeshPair.Value.Values)
                {
                    foreach (var pack in packList)
                    {
                        action(pack);
                    }
                }
            }
        }

        public void RemoveSplat(SplatInfo splatInfo, int packId)
        {
            _grassPacks[splatInfo].Remove(packId);
        }
    }

    class MyMaterialPropertyBlock
    {
        private int _arraySize;
        private readonly MaterialPropertyBlock _block = new MaterialPropertyBlock();

        public MyMaterialPropertyBlock(int arraySize)
        {
            _arraySize = arraySize;
        }

        public void AddVectorArrayToBlock(IUniformArray uniformArray, int elementsToSkipCount, int elementsToTakeCount)
        {
            Preconditions.Assert(uniformArray.Count == _arraySize,
                string.Format("Cant add vector of length {0} to block, as block length is {1}", uniformArray.Count, _arraySize));
            uniformArray.AddToBlock(_block, elementsToSkipCount, elementsToTakeCount);
        }

        public void AddSingleVectorArray(string name, Vector4 value)
        {
            var newValueArray = Enumerable.Repeat(value, _arraySize).ToArray();
            _block.SetVectorArray(name, newValueArray);
        }

        public MaterialPropertyBlock Block
        {
            get { return _block; }
        }

        public void AddGlobalUniform(string name, float value)
        {
            var newValueArray = Enumerable.Repeat(value, _arraySize).ToArray();
            _block.SetFloatArray(name, newValueArray);    
        }
    }

    class UniformArray<T> : IUniformArray
    {
        private string _name;
        private T[] _valuesArray;
        private readonly Action<MaterialPropertyBlock, string, T[]> _settingFunc;

        private UniformArray(string name, T[] valuesArray, Action< MaterialPropertyBlock, string, T[] > settingFunc  )
        {
            this._name = name;
            _valuesArray = valuesArray;
            _settingFunc = settingFunc;
        }

        public void AddToBlock(MaterialPropertyBlock block, int elementsToSkipCount, int elementsToTakeCount)
        {
            _settingFunc(block, _name, _valuesArray.Skip(elementsToSkipCount).Take(elementsToTakeCount).ToArray());
        }

        public int Count
        {
            get { return _valuesArray.Length; }
        }

        public string Name
        {
            get { return _name; }
        }

        public T[] ValuesArray
        {
            get { return _valuesArray; }
        }

        public static UniformArray<float> Of(string name, float[] values)
        {
            return new UniformArray<float>(name, values, (block, aName, aValues) =>
            {
                block.SetFloatArray(aName, aValues);
            });
        }

        public static UniformArray<Vector4> Of(string name, Vector4[] values)
        {
            return new UniformArray<Vector4>(name, values, (block, aName, aValues) =>
            {
                block.SetVectorArray(aName, aValues);
            });
        } 
    }

    interface IUniformArray
    {
        void AddToBlock(MaterialPropertyBlock block, int elementsToSkipCount, int elementsToTakeCount);
        int Count { get; }
    }

    class GrassPack
    {
        public GrassPack( Matrix4x4[] maticesArray, MyMaterialPropertyBlock propertiesBlock)
        {
            Preconditions.Assert( maticesArray.Length <= Constants.MaxInstancesPerPack,
                String.Format("In grass pack there can be at max {0} elements, but is {1}", Constants.MaxInstancesPerPack, maticesArray.Length));

            CastShadows = ShadowCastingMode.Off;
            MaticesArray = maticesArray;
            InstancesCount = maticesArray.Length;
            MyBlock = propertiesBlock;
        }

        public ShadowCastingMode CastShadows { get; private set; }
        public Matrix4x4[] MaticesArray { get; private set; }
        public int InstancesCount { get; private set; }

        public MyMaterialPropertyBlock MyBlock { get; private set; }
    }

    internal class SplatInfo
    {
        private Mesh _mesh;
        private Material _material;

        public SplatInfo(Mesh mesh, Material material)
        {
            this._mesh = mesh;
            this._material = material;
        }

        public Mesh Mesh
        {
            get { return _mesh; }
        }

        public Material Material
        {
            get { return _material; }
        }

        protected bool Equals(SplatInfo other)
        {
            return Equals(_mesh, other._mesh) && Equals(_material, other._material);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SplatInfo) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((_mesh != null ? _mesh.GetHashCode() : 0)*397) ^ (_material != null ? _material.GetHashCode() : 0);
            }
        }

        private sealed class MeshMaterialEqualityComparer : IEqualityComparer<SplatInfo>
        {
            public bool Equals(SplatInfo x, SplatInfo y)
            {
                if (ReferenceEquals(x, y)) return true;
                if (ReferenceEquals(x, null)) return false;
                if (ReferenceEquals(y, null)) return false;
                if (x.GetType() != y.GetType()) return false;
                return Equals(x._mesh, y._mesh) && Equals(x._material, y._material);
            }

            public int GetHashCode(SplatInfo obj)
            {
                unchecked
                {
                    return ((obj._mesh != null ? obj._mesh.GetHashCode() : 0)*397) ^ (obj._material != null ? obj._material.GetHashCode() : 0);
                }
            }
        }

        private static readonly IEqualityComparer<SplatInfo> MeshMaterialComparerInstance = new MeshMaterialEqualityComparer();

        public static IEqualityComparer<SplatInfo> MeshMaterialComparer
        {
            get { return MeshMaterialComparerInstance; }
        }
    }
}
