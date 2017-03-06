using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Grass
{
    internal class GrassGeneratorObj : MonoBehaviour
    {
        private void Start()
        {
            GrassTuftGenerator.CreateGrassTuft();
        }
    }
}
