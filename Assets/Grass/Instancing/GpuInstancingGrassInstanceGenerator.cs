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
            List<GrassShaderUniform<float>> floatUniforms = new List<GrassShaderUniform<float>>();
            List<GrassShaderUniform<Vector4>> vector4Uniforms = new List<GrassShaderUniform<Vector4>>();

            foreach (var entity in grassEntitiesWithMaterials.Entities)
            {
                floatUniforms.AddRange(entity.GetFloatUniforms());
                vector4Uniforms.AddRange(entity.GetVector4Uniforms());
            }

            List<IUniformArray> uniformArrays =
                vector4Uniforms.GroupBy(c => c.Name).Select(c => new UniformArray<Vector4>(c.ToArray())).Cast<IUniformArray>().Union(
                floatUniforms.GroupBy(c => c.Name).Select(c => new UniformArray<float>(c.ToArray())).Cast<IUniformArray>()).ToList();

            return new GpuGrassInstancesTemplate(
                grassEntitiesWithMaterials.Entities.Select(c => c.LocalToWorldMatrix).ToArray(), uniformArrays);
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