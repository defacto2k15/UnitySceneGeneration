using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Grass
{
    class GrassEntitiesSet
    {
        private readonly List<GrassEntity> _entities;

        public GrassEntitiesSet(List<GrassEntity> entities)
        {
            _entities = entities;
        }

        public List<GrassEntity> Entities
        {
            get { return _entities; }
        }

        public void TranslateBy(Vector3 vector3)
        {
            _entities.ForEach( e => e.Position = e.Position + vector3);
        }
    }
}
