using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Grass.Instancing;
using UnityEngine;
using UnityEngine.Rendering;

namespace Assets.Grass.Container
{
    class GpuInstancingGrassInstanceContainer : IGrassInstanceContainer
    {
        private Mesh _mesh; //todo what with multimesh and multimaterial??
        private Material _material;
        private readonly List<GrassPack> _grassPacks = new List<GrassPack>();
        private readonly GpuInstancingGrassInstanceGenerator _generator = new GpuInstancingGrassInstanceGenerator();

        public GrassSplat AddGrassEntities(GrassEntitiesWithMaterials grassEntitiesWithMaterials)
        {
            var grassInstancesTemplate = _generator.Generate(grassEntitiesWithMaterials);

            var maticesArray = grassInstancesTemplate.TransformMatices;
            var uniformArrays = grassInstancesTemplate.UniformArrays;

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
                _grassPacks.Add(new GrassPack(packMaticesArray, block));
            }

            _mesh = grassEntitiesWithMaterials.Mesh;
            _material = grassEntitiesWithMaterials.Material;
            return new GrassSplat();
        }

        public void Draw()
        {
            foreach (var aGrassPack in _grassPacks)
            {
                Graphics.DrawMeshInstanced(_mesh, 0, _material, aGrassPack.MaticesArray, aGrassPack.InstancesCount, aGrassPack.MyBlock.Block, 
                    aGrassPack.CastShadows, false,0, null );
            }
        }


        public void SetGlobalColor(string name, Color value)
        {
            foreach (var aGrassPack in _grassPacks)
            {
                aGrassPack.MyBlock.AddSingleVectorArray(name, value);
            }
        }

        public void SetGlobalUniform(string name, float value)
        {
            foreach (var aGrassPack in _grassPacks)
            {
                aGrassPack.MyBlock.AddGlobalUniform(name, value);
            }
        }
    }

    class MyMaterialPropertyBlock
    {
        private int _arraySize;
        private readonly MaterialPropertyBlock block = new MaterialPropertyBlock();

        public MyMaterialPropertyBlock(int arraySize)
        {
            _arraySize = arraySize;
        }

        public void AddVectorArrayToBlock(IUniformArray uniformArray, int elementsToSkipCount, int elementsToTakeCount)
        {
            Preconditions.Assert(uniformArray.Count == _arraySize,
                string.Format("Cant add vector of length {0} to block, as block length is {1}", uniformArray.Count, _arraySize));
            uniformArray.AddToBlock(block, elementsToSkipCount, elementsToTakeCount);
        }

        public void AddSingleVectorArray(string name, Vector4 value)
        {
            var newValueArray = Enumerable.Repeat(value, _arraySize).ToArray();
            block.SetVectorArray(name, newValueArray);
        }

        public MaterialPropertyBlock Block
        {
            get { return block; }
        }

        public void AddGlobalUniform(string name, float value)
        {
            var newValueArray = Enumerable.Repeat(value, _arraySize).ToArray();
            block.SetFloatArray(name, newValueArray);    
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
}
