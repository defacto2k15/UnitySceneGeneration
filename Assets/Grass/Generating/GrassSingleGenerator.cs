using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Grass.Generating
{
    class GrassSingleGenerator
    {
        public GrassEntitiesSet CreateSingleGrass()
        {
            var angle = RandomGrassGenerator.GetAngle();
            float plantBendingSiffness = RandomGrassGenerator.GetBendingStiffness();
            float initialBendingValue = RandomGrassGenerator.GetInitialBendingValue();

            var grassEntity = new GrassEntity
            {
                Position = new Vector3(0,0,0),
                Rotation = new Vector3(0, angle, 0),
                Scale = RandomGrassGenerator.GetScale(),
                Color = RandomGrassGenerator.GetGrassColor(),
                PlantBendingStiffness = plantBendingSiffness ,
                InitialBendingValue = initialBendingValue ,
            };
            return new GrassEntitiesSet(new List<GrassEntity>{grassEntity});
        }
    }
}

