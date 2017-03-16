
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Utils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Assets.Grass
{
    class RandomGrassGenerator
    {
        private const float NOT_SET = -1;
        private const float EPSYLON = 0.001f;

        static public Color GetGrassColor(float baseHue = NOT_SET, float baseSaturation = NOT_SET, float baseValue = NOT_SET)
        {
            const float minHue = 67.0f/255.0f;
            const float maxHue = 125.0f/255.0f;
            if (Math.Abs(baseHue - NOT_SET) < EPSYLON)
            {
                baseHue = GetHue();
            }

            var rand1 = Random.Range(-0.2f, 0.2f);
            float hue = Mathf.Clamp(minHue + (maxHue - minHue) * (baseHue + rand1), minHue, maxHue);

            const float saturationMin = 0.8f;
            const float saturationMax = 1.0f;
            if (Math.Abs(baseSaturation - NOT_SET) < EPSYLON)
            {
                baseSaturation = GetSaturation();
            }
            var rand2 = Random.Range(-0.2f, 0.2f);
            float saturation = Mathf.Clamp(saturationMin + (saturationMax - saturationMin) * (baseSaturation + rand2), saturationMin, saturationMax);

            const float minValue = 0.25f;
            const float maxValue = 0.6f;
            if (Math.Abs(baseValue - NOT_SET) < EPSYLON)
            {
                baseValue = GetValue();
            }
            var rand3 = Random.Range(-0.2f, 0.2f);
            float value = Mathf.Clamp(minValue + (maxValue - minValue) * (baseValue + rand3), minValue, maxValue);
            
            return new Color(hue, saturation, value);
        }

        public static float GetHue()
        {
            return Random.value;
        }

        public static float GetSaturation()
        {
            return Random.value;
        }

        public static float GetValue()
        {
            return Random.value;
        }

        public static Vector3 GetScale()
        {
            return new Vector3(0.05f*Random.Range(0.75f, 1.25f), 1*Random.Range(0.75f, 1.25f),
                1*Random.Range(0.75f, 1.25f));
        }

        public static float GetAngle()
        {
            return (float)(2*Math.PI*Random.value);
        }

        public static float GetBendingStiffness()
        {
            return Random.value;
        }

        public static float GetInitialBendingValue()
        {
            return (Random.value -0.5f)*2 ;
        }
    }
}
