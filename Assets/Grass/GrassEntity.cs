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
        private Matrix4x4 _localToWorldMatrix;
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;
        private readonly List<GrassShaderUniform<float>> _floatUniforms = new List<GrassShaderUniform<float>>();
        private readonly List<GrassShaderUniform<Vector4>> _vector4Uniforms = new List<GrassShaderUniform<Vector4>>();

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
            _floatUniforms.Add(new GrassShaderUniform<float>(name, value));
        }

        public void AddUniform(GrassShaderUniformName name, Vector4 value)
        {
            _vector4Uniforms.Add(new GrassShaderUniform<Vector4>(name, value));
        }

        public void AddUniform(GrassShaderUniformName name, Color value)
        {
            _vector4Uniforms.Add(new GrassShaderUniform<Vector4>(name, value));
        }

        public List<GrassShaderUniform<float>> GetFloatUniforms()
        {
            return _floatUniforms;
        } 

        public List<GrassShaderUniform<Vector4>> GetVector4Uniforms()
        {
            return _vector4Uniforms
                .Union( new List<GrassShaderUniform<Vector4>>(){ new GrassShaderUniform<Vector4>(GrassShaderUniformName._PlantDirection, PlantDirection)})
                .ToList();
        }  
    }
}