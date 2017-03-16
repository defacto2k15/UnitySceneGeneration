using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.MeshGeneration;
using Assets.Utils;
using UnityEngine;

namespace Assets.Grass
{
    class GrassTuftGenerator
    {
        public  GrassEntitiesSet CreateGrassTuft()
        {
            //var tuftObj = new GameObject {name = "grassTuft"};

            var elementsRange = RandomTuftGenerator.GetTuftElementsRange();
            int elementsCount = RandomTuftGenerator.GetTuftCount(elementsRange);
            var anglesList = RandomTuftGenerator.GetRandomAngles(elementsCount, elementsRange.Max);
            float radiusFromCenter = 0.1f;
            List<GrassEntity> entities = new List<GrassEntity>();
            var tuftHue = RandomGrassGenerator.GetHue();
            var tuftValue = RandomGrassGenerator.GetValue();
            var randomSaturation = RandomGrassGenerator.GetSaturation();

            foreach (var angle in anglesList)
            {
                MyRange basePlantBendingStiffness = RandomTuftGenerator.GetBasePlantBendingStiffness();
                MyRange basePlantBendingValue = RandomTuftGenerator.GetBasePlantBendingValue();

                float radiousRandomOffset = RandomTuftGenerator.GetPositionOffset();
                var grassEntity = new GrassEntity
                {
                    Position =
                        new Vector3((radiusFromCenter+radiousRandomOffset)*(float) Math.Sin(angle), 0,
                                    (radiusFromCenter+radiousRandomOffset)*(float) Math.Cos(angle)+90),
                    Rotation = new Vector3(0,  angle, 0),
                    Scale = RandomGrassGenerator.GetScale(),
                    Color = RandomGrassGenerator.GetGrassColor(tuftHue, tuftValue, randomSaturation),
                    PlantBendingStiffness = RandomTuftGenerator.GetPlantBendingStiffness(basePlantBendingStiffness),
                    InitialBendingValue = RandomTuftGenerator.GetPlantBendingValue(basePlantBendingValue),
                };
                entities.Add(grassEntity);
            }

            return new GrassEntitiesSet(entities);
        }
    }
}
