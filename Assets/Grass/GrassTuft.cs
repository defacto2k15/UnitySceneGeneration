using UnityEngine;

namespace Assets.Grass
{
    internal class GrassTuft
    {
        private readonly Mesh mesh;

        public GrassTuft(Mesh mesh)
        {
            this.mesh = mesh;
        }

        public Vector3 Position { get; set; }
    }
}