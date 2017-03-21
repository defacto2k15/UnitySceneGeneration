using System.Collections.Generic;
using System.Linq;
using Assets.Grass.Container;
using Assets.Utils;
using UnityEngine;

namespace Assets.Grass.Instancing
{
    class GpuInstancingGrassInstanceGenerator 
    {
        public GpuGrassInstancesTemplate Generate(GrassEntitiesWithMaterials grassEntitiesWithMaterials)
        {
            //var ab = grassEntitiesWithMaterials.Entities.Select(c => c.InitialBendingValue).ToArray(); //todo
            //return new GpuGrassInstancesTemplate(
            //    grassEntitiesWithMaterials.Entities.Select(c => c.LocalToWorldMatrix).ToArray(), 
            //    new List<IUniformArray>()
            //{
            //    UniformArray<Vector4>.Of("_Color", grassEntitiesWithMaterials.Entities.Select( c => (Vector4)c.Color ).ToArray()),
            //    UniformArray<Vector4>.Of("_PlantDirection", grassEntitiesWithMaterials.Entities.Select( c => c.PlantDirection ).ToArray()),
            //    UniformArray<float>.Of("_InitialBendingValue", grassEntitiesWithMaterials.Entities.Select( c => c.InitialBendingValue ).ToArray()),
            //    UniformArray<float>.Of("_PlantBendingStiffness", grassEntitiesWithMaterials.Entities.Select( c => c.PlantBendingStiffness ).ToArray()),
            //});
            return null;
        }
    }


    class GpuGrassInstancesTemplate
    {
        private readonly Matrix4x4[] _transformMatices;
        private readonly List<IUniformArray> _uniformArrays;

        public GpuGrassInstancesTemplate(Matrix4x4[] transformMatices, List<IUniformArray> uniformArrays)
        {
            _transformMatices = transformMatices;
            _uniformArrays = uniformArrays;
        }

        public Matrix4x4[] TransformMatices
        {
            get { return _transformMatices; }
        }

        public List<IUniformArray> UniformArrays
        {
            get { return _uniformArrays; }
        }
    }
}