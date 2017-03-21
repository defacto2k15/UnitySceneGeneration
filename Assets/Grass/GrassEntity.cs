using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Utils;
using JetBrains.Annotations;
using UnityEngine;

namespace Assets.Grass
{
    internal class GrassEntity
    {
        private readonly List<IGrassShaderUniform> _uniforms = new List<IGrassShaderUniform>(); 
        private Matrix4x4 _localToWorldMatrix;
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;

        public Vector3 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                RegenerateLocalToWorldMatrix();
            }
        }

        public Vector3 Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Vector3 Scale
        {
            get { return _scale; }
            set { _scale = value; }
        }

        private void RegenerateLocalToWorldMatrix()
        {
            _localToWorldMatrix = TransformUtils.GetLocalToWorldMatrix(_position,_rotation, _scale);
        }

        public Matrix4x4 LocalToWorldMatrix
        {
            get
            {
                RegenerateLocalToWorldMatrix();
                return _localToWorldMatrix;
            }
        }

        private Vector4 PlantDirection
        {
            get
            {
                var angle = _rotation.y;
                return new Vector4((float)Math.Sin(angle), 0, (float)Math.Cos(angle), 0).normalized;
            }
        }

        public void AddUniform(GrassShaderUniformName name, float value)
        {
            _uniforms.Add( new GrassShaderUniform<float>(name, value, (mat, aName, aValue) => mat.SetFloat(aName.ToString(),aValue)));
        }

        public void AddUniform(GrassShaderUniformName name, Color value)
        {
            _uniforms.Add( new GrassShaderUniform<Color>(name, value, (mat, aName, aValue) => mat.SetColor(aName.ToString(),aValue)));
        }

        public void AddUniform(GrassShaderUniformName name, Vector4 value)
        {
            _uniforms.Add( new GrassShaderUniform<Color>(name, value, (mat, aName, aValue) => mat.SetVector(aName.ToString(),aValue)));
        }

        public List<IGrassShaderUniform> GetUniforms()
        {
            AddUniform(GrassShaderUniformName._PlantDirection, PlantDirection);
            return _uniforms.ToList();
        } 
    }

    interface IGrassShaderUniform
    {
        void Set(Material material);
    }

    class GrassShaderUniform<T> : IGrassShaderUniform
    {
        private readonly GrassShaderUniformName _name;
        private readonly T _value;

        private readonly Action<Material, GrassShaderUniformName, T> _materialSetter;

        public GrassShaderUniform(GrassShaderUniformName name, T value, Action<Material, GrassShaderUniformName, T> materialSetter)
        {
            _name = name;
            _value = value;
            _materialSetter = materialSetter;
        }

        public void Set(Material material)
        {
            _materialSetter(material, _name, _value);
        }
    }
}