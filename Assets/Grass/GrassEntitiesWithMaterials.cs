using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Grass
{
    class GrassEntitiesWithMaterials
    {
        private readonly List<GrassEntity> _entities;
        private readonly Material _material;
        private readonly Mesh _mesh;

        public GrassEntitiesWithMaterials(List<GrassEntity> entities, Material material, Mesh mesh)
        {
            this._entities = entities;
            this._material = material;
            this._mesh = mesh;
        }

        public List<GrassEntity> Entities
        {
            get { return _entities; }
        }

        public Material Material
        {
            get { return _material; }
        }

        public Mesh Mesh
        {
            get { return _mesh; }
        }
    }
}
