using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.MeshGeneration;
using Assets.Utils;
using UnityEditor;
using UnityEngine;

namespace Assets.Grass.Billboard
{
    class GrassBillboardGenerator
    {
        private GrassMeshGenerator meshGenerator = new GrassMeshGenerator();

        public void GenerateStarTuft(Material material, int elementsInTuftCount = 3)
        {
            for (int i = 0; i < elementsInTuftCount; i++)
            {
                var obj = generateOnePlaneObject(material, 0, 1);
                obj.transform.localEulerAngles = new Vector3(0, 0, ((float)i)/elementsInTuftCount * 180);
            }
        }

        public void generateTriangleTurf(Material material)
        {
            var scale = 0.75f;
            //var obj1 = generateOnePlantSegment(material, 0, scale, 0,0, 0, 0.33f);
            //var obj2 = generateOnePlantSegment(material, 0, scale, 0.75f, 0, 0.33f, 0.66f);
            //var obj3 = generateOnePlantSegment(material, 0, scale, 1.5f, 0, 0.66f, 1); 

            // instead of 0.5 there was 0.433, but this is better

            var obj1 = generateOnePlantSegment(material, 0, scale, 0, -2.5f * 0.125f, 0.125f, 1f - 0.125f);
            var obj11 = generateOnePlantSegment(material, 0, 0.125f, -(3.5f) * 0.125f, -2.5f * 0.125f, 0f, 0.125f);
            var obj12 = generateOnePlantSegment(material, 0, 0.125f, 3.5f * 0.125f, -2.5f * 0.125f, 1f - 0.125f, 1);

            var obj2 = generateOnePlantSegment(material, 120, scale, -1.5f * 0.125f, 0, 0.125f, 1f - 0.125f);
            var obj21 = generateOnePlantSegment(material, 120, 0.125f, -(3.25f) * 0.125f, (-2.5f - 0.5f) * 0.125f, 1f - 0.125f, 1);
            var obj22 = generateOnePlantSegment(material, 120, 0.125f, (0.25f) * 0.125f, (2.5f + 0.5f) * 0.125f, 0f, 0.125f);

            var obj3 = generateOnePlantSegment(material, 60, scale, 1.5f * 0.125f, 0, 0.125f, 1f - 0.125f);
            var obj31 = generateOnePlantSegment(material, 60, 0.125f, (3.25f) * 0.125f, (-2.5f - 0.5f) * 0.125f, 1f - 0.125f, 1);
            var obj32 = generateOnePlantSegment(material, 60, 0.125f, -(0.25f) * 0.125f, (2.5f + 0.5f) * 0.125f, 0f, 0.125f);

        }

        private GameObject generateOnePlantSegment(Material material, float zEulerAngle, float xScale, float xPos,
            float zPos, float minUv, float maxUv)
        {
            var obj = generateOnePlaneObject(material, minUv, maxUv);
            obj.transform.localEulerAngles = new Vector3(0,  zEulerAngle,0);
            obj.transform.localScale = new Vector3(xScale, 1, 1);
            obj.transform.localPosition = new Vector3(xPos, 0, zPos);
            return obj;
        }

        private GameObject generateOnePlaneObject(Material material, float minUv, float maxUv)
        {
            GameObject obj = new GameObject("GrassBillboard");
            obj.AddComponent<MeshFilter>().mesh = meshGenerator.GetGrassBillboardMesh(minUv, maxUv);
            var rend = obj.AddComponent<MeshRenderer>();
            rend.material = material;
            return obj;
        }



        public void XGenerateStarTuft( int elementsInTuftCount = 3)
        {
            for (int i = 0; i < elementsInTuftCount; i++)
            {
                var obj = new GrassEntity {Rotation = MyMathUtils.DegToRad(new Vector3(0,0, ((float) i)/elementsInTuftCount*180))};
                obj.AddUniform(GrassShaderUniformName._MinUv, 0);
                obj.AddUniform(GrassShaderUniformName._MaxUv, 1);
                obj.AddUniform(GrassShaderUniformName._RandSeed, UnityEngine.Random.value );
            }
        }

        public GrassEntitiesSet XgenerateTriangleTurf()
        {
            var scale = 0.75f;
            //var obj1 = generateOnePlantSegment(material, 0, scale, 0,0, 0, 0.33f);
            //var obj2 = generateOnePlantSegment(material, 0, scale, 0.75f, 0, 0.33f, 0.66f);
            //var obj3 = generateOnePlantSegment(material, 0, scale, 1.5f, 0, 0.66f, 1); 

            // instead of 0.5 there was 0.433, but this is better

            var randSeed1 = UnityEngine.Random.value;
            var obj1 = XgenerateOnePlantSegment( 0, scale, 0, -2.5f * 0.125f, 0.125f, 1f - 0.125f, randSeed1);
            var obj11 = XgenerateOnePlantSegment( 0, 0.125f, -(3.5f) * 0.125f, -2.5f * 0.125f, 0f, 0.125f, randSeed1);
            var obj12 = XgenerateOnePlantSegment( 0, 0.125f, 3.5f * 0.125f, -2.5f * 0.125f, 1f - 0.125f, 1, randSeed1);

            var randSeed2 = UnityEngine.Random.value;
            var obj2 = XgenerateOnePlantSegment(120, scale, -1.5f * 0.125f, 0, 0.125f, 1f - 0.125f, randSeed2);
            var obj21 = XgenerateOnePlantSegment(120, 0.125f, -(3.25f) * 0.125f, (-2.5f - 0.5f) * 0.125f, 1f - 0.125f, 1, randSeed2);
            var obj22 = XgenerateOnePlantSegment(120, 0.125f, (0.25f) * 0.125f, (2.5f + 0.5f) * 0.125f, 0f, 0.125f, randSeed2);

            var randSeed3 = UnityEngine.Random.value;
            var obj3 = XgenerateOnePlantSegment(60, scale, 1.5f * 0.125f, 0, 0.125f, 1f - 0.125f, randSeed3);
            var obj31 = XgenerateOnePlantSegment(60, 0.125f, (3.25f) * 0.125f, (-2.5f - 0.5f) * 0.125f, 1f - 0.125f, 1, randSeed3);
            var obj32 = XgenerateOnePlantSegment(60, 0.125f, -(0.25f) * 0.125f, (2.5f + 0.5f) * 0.125f, 0f, 0.125f, randSeed3);

            return new GrassEntitiesSet(new List<GrassEntity>()
            {
                obj1,
                obj11,
                obj12,
                obj2,
                obj21,
                obj22,
                obj3,
                obj31,
                obj32
            });
        }

        private GrassEntity XgenerateOnePlantSegment(float yEulerAngle, float xScale, float xPos,
            float zPos, float minUv, float maxUv, float randSeed)
        {

            var obj = new GrassEntity
            {
                Rotation = MyMathUtils.DegToRad(new Vector3(0,  yEulerAngle, 0)),
                Scale = new Vector3(xScale, 1, 1),
                Position = new Vector3(xPos, 0, zPos)
            };
            obj.AddUniform(GrassShaderUniformName._MinUv, minUv);
            obj.AddUniform(GrassShaderUniformName._MaxUv, maxUv);
            obj.AddUniform(GrassShaderUniformName._RandSeed, randSeed);
            return obj;
        }
    }
}
