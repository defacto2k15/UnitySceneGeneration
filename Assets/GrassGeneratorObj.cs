using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Assets.Grass.Billboard;
using Assets.Grass.Container;
using Assets.Grass.Generating;
using Assets.Grass.Instancing;
using Assets.Grass.Lod;
using Assets.Grass.Placing;
using Assets.Utils;
using UnityEngine;
using UnityEngine.Rendering;
using Random = UnityEngine.Random;

namespace Assets.Grass
{
    internal class GrassGeneratorObj : MonoBehaviour
    {
        public bool shouldUseInstancing = true;
        private IGrassInstanceContainer _grassInstanceContainer;
        public Material BillboardMaterial;
        public Material ShellMaterial;
        public Material GrassBillboardGeneratorMaterial;
        public RenderTexture RenderTexture;
        public Texture RealTexture;
        public Material fragMaterial;

        public GameObject RenderTextureGameObject;
        public GameObject CameraGameObject;

        private void Start()
        {
            Random.InitState(DateTime.Now.Second);
            _rootInstanceContainer = new RootInstanceContainer();
            Material grassMaterial = new Material( Shader.Find("Custom/testSurfaceShader23.Instanced"));

            var meshGenerator = new GrassMeshGenerator();

            var grassTuftLodEntitySplatGenerator = new GrassTuftLodEntitySplatGenerator(
                new GrassTuftGenerator(), 
                new SingleGrassUniformPositionProvider(),
                new GrassTuftAbstractSettingGenerator(),
                _rootInstanceContainer,
                meshGenerator,
                grassMaterial);

            var singleGrassLodEntitySplatGenerator = new SingleGrassLodEntitySplatGenerator(
                new GrassSingleGenerator(),
                new SingleGrassUniformPositionProvider(),
                new GrassSingleAbstractSettingGenerator(),
                _rootInstanceContainer,
                meshGenerator,
                grassMaterial);

            var lodGroupProvider = new LodGroupsProvider(new List<ILodEntitySplatGenerator> { singleGrassLodEntitySplatGenerator, grassTuftLodEntitySplatGenerator });

            var mapSize = new Vector2(100, 100);
            var splatSize = mapSize/10;
            var singleLevelDistance = (int)splatSize.x*2;
            var singleLevelMargin = 0.1f;
            var lodMarginPowCoef = 0.8f;
            _grassLodManager = new GrassLodManager(new LodLevelResolver(7, singleLevelDistance, singleLevelMargin, lodMarginPowCoef), lodGroupProvider, mapSize, splatSize);
            _grassLodManager.UpdateLod(new Vector3(0,0,0));
        }

        private void generateShells()
        {
            var layersCount = 10;
            var grassHeight = 0.2f;
            GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
            for (int i = 0; i < layersCount; i++)
            {
                var shellObj = new GameObject("grassShell " + i);
                shellObj.transform.localPosition = new Vector3(10, (grassHeight/layersCount)* i, 10);
                var rend = shellObj.AddComponent<MeshRenderer>();
                rend.transform.localScale = new Vector3(1,1,1);
                rend.material = ShellMaterial;
                rend.material.SetFloat("_LayerHeight", i *(1 / (float)layersCount));

                rend.material.SetFloat("_BendingStrength", 0.2f);
                rend.material.SetVector("_WindDirection", new Vector4(1,1,1,1));
                rend.material.SetFloat("__Scale", 30.0f);
                shellObj.AddComponent<MeshFilter>().mesh = plane.GetComponent<MeshFilter>().mesh;
            }

        }

        private void generateBillboardTexture()
        {
            Graphics.Blit(RealTexture, RenderTexture, fragMaterial);
        }

        private GrassEntitiesWithMaterials generateTurf(GrassBillboardGenerator billboardGenerator)
        {
            var meshGenerator = new GrassMeshGenerator();
            var mesh = meshGenerator.GetGrassBillboardMesh(0, 1);
            var xgenerateTriangleTurf = billboardGenerator.GenerateTriangleTurf(); //todo : use grass entities set and rotate
            //xgenerateTriangleTurf.Rotation = (MyMathUtils.DegToRad(new Vector3(0, 90, 0)));
            //xgenerateTriangleTurf.Position = new Vector3(2,2,2);
            return  new GrassEntitiesWithMaterials(xgenerateTriangleTurf.EntitiesAfterTransform, BillboardMaterial, mesh, ContainerType.GameObject);
        }

        static float a = 0;
        static float angl = 0;
        private static float windStrength = 0;
        private IGrassSplat grassSplat;
        private GrassLodManager _manager;
        private RootInstanceContainer _rootInstanceContainer;
        private GrassLodManager _grassLodManager;

        private void Update()
        {
            if (Input.GetKey(KeyCode.UpArrow))
            {
                _rootInstanceContainer.SetGlobalColor("_Color", new Color(a % 1.0f, (a + 0.5f) % 1.0f, (a + 0.3f) % 1.0f));
                a += 0.1f;
            }

            if (Input.GetKey(KeyCode.Q))
            {
                _rootInstanceContainer.SetGlobalUniform(GrassShaderUniformName._BendingStrength, windStrength);
                windStrength += 0.1f;
                windStrength = Mathf.Clamp01(windStrength);
            }
            if (Input.GetKey(KeyCode.W))
            {
                _rootInstanceContainer.SetGlobalUniform(GrassShaderUniformName._BendingStrength, windStrength);
                windStrength -= 0.1f;
                windStrength = Mathf.Clamp01(windStrength);
            }
            if (Input.GetKey(KeyCode.T))
            {
                grassSplat.Remove();
            }

            //if (Time.frameCount%100 == 0)
            //{
             //   _manager.UpdateLod(new Vector3(0, 0, 0));
                
            //}
            if (windStrength < 1)
            {
                //_manager.UpdateLod(new Vector3(0,0,0));
                //windStrength = 2;
            }

            if (Time.frameCount%100 == 0)
            {
                _grassLodManager.UpdateLod(CameraGameObject.transform.position);
            }
            _rootInstanceContainer.Draw();
        }

        Vector4 GetWindVector(float angle)
        {
            return new Vector4((float)Math.Sin(angle), 0, (float)Math.Cos(angle));
        }
    }

    internal class GrassTuftAbstractSettingGenerator : AbstractSettingGenerator
    {
        public override void SetSettings(GrassEntitiesSet aGrass)
        {
            var tuftHue = RandomGrassGenerator.GetHue();
            var tuftValue = RandomGrassGenerator.GetValue();
            var randomSaturation = RandomGrassGenerator.GetSaturation(); //todo use it
            MyRange basePlantBendingStiffness = RandomTuftGenerator.GetBasePlantBendingStiffness();
            MyRange basePlantBendingValue = RandomTuftGenerator.GetBasePlantBendingValue();
            ForeachEntity(aGrass, c => c.AddUniform(GrassShaderUniformName._PlantBendingStiffness, RandomTuftGenerator.GetPlantBendingStiffness(basePlantBendingStiffness)));
            ForeachEntity(aGrass, c => c.AddUniform(GrassShaderUniformName._InitialBendingValue, RandomTuftGenerator.GetPlantBendingValue(basePlantBendingValue)));
            ForeachEntity(aGrass, c => c.AddUniform(GrassShaderUniformName._Color, RandomGrassGenerator.GetGrassColor(tuftHue)));
            ForeachEntity(aGrass, c => c.AddUniform(GrassShaderUniformName._RandSeed, UnityEngine.Random.value));            
        }
    }

    internal class LodGroupsProvider : ILodGroupsProvider //todo przenies
    {
        private readonly List<ILodEntitySplatGenerator> _lodEntitySplatGenerators;

        public LodGroupsProvider(List<ILodEntitySplatGenerator> lodEntitySplatGenerators)
        {
            _lodEntitySplatGenerators = lodEntitySplatGenerators;
        }

        public LodGroup GenerateLodGroup(MapAreaPosition position, int newLodLevel)
        {
            return new LodGroup(_lodEntitySplatGenerators.Select(c => c.Generate(position, newLodLevel)).ToList(), newLodLevel, position);
        }
    }

    internal class SingleGrassLodResolver : IEntityLodResolver
    {
        // globalLod 0 - 10
        public int GetEntityLod(int globalLod)
        {
            var t = Mathf.InverseLerp(Constants.MIN_GLOBAL_LOD, Constants.MAX_GLOBAL_LOD, globalLod);
            return (int)Mathf.Round(Mathf.Lerp(Constants.MIN_SINGLE_GRASS_LOD, Constants.MAX_SINGLE_GRASS_LOD, t));
        }
    }

    internal class SingleGrassLodEntitySplatGenerator : ILodEntitySplatGenerator
    {
        private readonly GrassSingleGenerator _grassSingleGenerator;
        private readonly IEntityPositionProvider _singleGrassUniformPositionProvider;
        private readonly GrassSingleAbstractSettingGenerator _grassSingleAbstractSettingGenerator;
        private readonly IGrassInstanceContainer _grassInstanceContainer;
        private readonly GrassMeshGenerator _meshGenerator;
        private readonly Material _grassMaterial;

        public SingleGrassLodEntitySplatGenerator(GrassSingleGenerator grassSingleGenerator, IEntityPositionProvider singleGrassUniformPositionProvider, GrassSingleAbstractSettingGenerator grassSingleAbstractSettingGenerator, IGrassInstanceContainer grassInstanceContainer, GrassMeshGenerator meshGenerator, Material grassMaterial)
        {
            _grassSingleGenerator = grassSingleGenerator;
            _singleGrassUniformPositionProvider = singleGrassUniformPositionProvider;
            _grassSingleAbstractSettingGenerator = grassSingleAbstractSettingGenerator;
            _grassInstanceContainer = grassInstanceContainer;
            _meshGenerator = meshGenerator;
            _grassMaterial = grassMaterial;
        }

        public LodEntitySplat Generate(MapAreaPosition position, int newLodLevel)
        {
            return new LodEntitySplat(position,
                new SingleGrassLodResolver(),
                new SingleGrassEntitySplatGenerator(_grassSingleGenerator, _singleGrassUniformPositionProvider,
                    _grassSingleAbstractSettingGenerator, _grassInstanceContainer, _meshGenerator, _grassMaterial),
                newLodLevel);
        }
    }

    internal class GrassTuftLodEntitySplatGenerator : ILodEntitySplatGenerator
    {
        private readonly GrassTuftGenerator _grassTuftGenerator;
        private readonly IEntityPositionProvider _grassTuftPositionProvider;
        private readonly GrassTuftAbstractSettingGenerator _grassTuftAbstractSettingGenerator;
        private readonly IGrassInstanceContainer _grassInstanceContainer;
        private readonly GrassMeshGenerator _meshGenerator;
        private readonly Material _grassTuftMaterial;

        public GrassTuftLodEntitySplatGenerator(GrassTuftGenerator grassTuftGenerator, IEntityPositionProvider grassTuftPositionProvider, GrassTuftAbstractSettingGenerator grassTuftAbstractSettingGenerator, IGrassInstanceContainer grassInstanceContainer, GrassMeshGenerator meshGenerator, Material grassTuftMaterial)
        {
            this._grassTuftGenerator = grassTuftGenerator;
            _grassTuftPositionProvider = grassTuftPositionProvider;
            _grassTuftAbstractSettingGenerator = grassTuftAbstractSettingGenerator;
            _grassInstanceContainer = grassInstanceContainer;
            _meshGenerator = meshGenerator;
            _grassTuftMaterial = grassTuftMaterial;
        }


        public LodEntitySplat Generate(MapAreaPosition position, int newLodLevel)
        {
            return new LodEntitySplat(position,
                new SingleGrassLodResolver(), //its good for tuft
                new GrassTuftEntitySplatGenerator(_grassTuftGenerator, _grassTuftPositionProvider, _grassTuftAbstractSettingGenerator, _grassInstanceContainer, _meshGenerator, _grassTuftMaterial),
                newLodLevel);
        }
    }

    internal class GrassTuftEntitySplatGenerator : IEntitySplatGenerator
    {
        private readonly GrassTuftGenerator _grassTuftGenerator;
        private readonly IEntityPositionProvider _grassTuftPositionProvider;
        private readonly GrassTuftAbstractSettingGenerator _grassTuftAbstractSettingGenerator;
        private readonly IGrassInstanceContainer _grassInstanceContainer;
        private readonly GrassMeshGenerator _meshGenerator;
        private readonly Material _grassTuftMaterial;

        public GrassTuftEntitySplatGenerator(GrassTuftGenerator grassTuftGenerator, IEntityPositionProvider grassTuftPositionProvider, GrassTuftAbstractSettingGenerator grassTuftAbstractSettingGenerator, IGrassInstanceContainer grassInstanceContainer, GrassMeshGenerator meshGenerator, Material grassTuftMaterial)
        {
            _grassTuftGenerator = grassTuftGenerator;
            _grassTuftPositionProvider = grassTuftPositionProvider;
            _grassTuftAbstractSettingGenerator = grassTuftAbstractSettingGenerator;
            _grassInstanceContainer = grassInstanceContainer;
            _meshGenerator = meshGenerator;
            _grassTuftMaterial = grassTuftMaterial;
        }

        public IGrassSplat GenerateSplat(MapAreaPosition position, int entityLodLevel)
        {
            List<GrassEntitiesSet> singleEntities = new List<GrassEntitiesSet>();
            for (int i = 0; i < 20; i++)
            {
                var aTuft = _grassTuftGenerator.CreateGrassTuft();
                _grassTuftPositionProvider.SetPosition(aTuft, position);
                _grassTuftAbstractSettingGenerator.SetSettings(aTuft);
                if (entityLodLevel == 0)
                {
                    aTuft.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._Color, new Vector4(99.0f, 0f, 0.0f, 1.0f)));
                }
                if (entityLodLevel == 1)
                {
                    aTuft.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._Color, new Vector4(99.0f, 99.0f, 0.0f, 1.0f)));
                }
                if (entityLodLevel == 2)
                {
                    aTuft.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._Color, new Vector4(99.0f, 99.0f, 99.0f, 1.0f)));
                }
                if (entityLodLevel == 3)
                {
                    aTuft.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._Color, new Vector4(0.0f, 99.0f, 0.0f, 1.0f)));
                }
                if (entityLodLevel == 4)
                {
                    aTuft.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._Color, new Vector4(0.0f, 99.0f, 99.0f, 1.0f)));
                }
                if (entityLodLevel == 5)
                {
                    aTuft.EntitiesBeforeTransform.ForEach(c => c.AddUniform(GrassShaderUniformName._Color, new Vector4(0.0f, 0.0f, 99.0f, 1.0f)));
                }

                singleEntities.Add(aTuft);
            }
            var bladeTriangleCount = (int)Mathf.Max(Constants.MAX_GRASS_MESH_LEVEL - entityLodLevel, Constants.MIN_GRASS_MESH_LEVEL);
            Mesh mesh = _meshGenerator.GetGrassBladeMesh(bladeTriangleCount);
            return _grassInstanceContainer.AddGrassEntities(
                new GrassEntitiesWithMaterials(singleEntities.SelectMany(c => c.EntitiesAfterTransform).ToList(), _grassTuftMaterial, mesh,
                    ContainerType.Instancing));
        }
    }
}
