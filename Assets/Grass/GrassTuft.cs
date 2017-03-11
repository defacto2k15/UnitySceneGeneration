using Assets.Utils;
using UnityEngine;

namespace Assets.Grass
{
    internal class GrassTuft
    {
        private readonly Mesh mesh;
        private Matrix4x4 _localToWorldMatrix;
        private Vector3 _position;

        public GrassTuft(Mesh mesh)
        {
            this.mesh = mesh;
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

        private void RegenerateLocalToWorldMatrix()
        {
            _localToWorldMatrix = TransformUtils.GetLocalToWorldMatrix(_position);
        }

        public Matrix4x4 LocalToWorldMatrix
        {
            get { return _localToWorldMatrix; }
        }

        public Mesh Mesh
        {
            get { return mesh; }
        }
    }
}