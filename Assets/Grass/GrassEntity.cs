using System;
using Assets.Utils;
using UnityEngine;

namespace Assets.Grass
{
    internal class GrassEntity
    {
        private Matrix4x4 _localToWorldMatrix;
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;
        private Color _color;

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

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

        public float PlantBendingStiffness { get; set; }
        public float InitialBendingValue { get; set; }

        public Vector4 PlantDirection
        {
            get
            {
                var angle = _rotation.y;
                return new Vector4((float)Math.Sin(angle), 0, (float)Math.Cos(angle), 0).normalized;
            }
        }
    }
}