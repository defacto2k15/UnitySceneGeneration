using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Grass.Container
{
    class RootInstanceContainer : IGrassInstanceContainer
    {
        public Dictionary<ContainerType, IGrassInstanceContainer> Containers = new Dictionary<ContainerType, IGrassInstanceContainer>();

        public RootInstanceContainer()
        {
            Containers.Add(ContainerType.GameObject,  new GameObjectGrassInstanceContainer());
            Containers.Add(ContainerType.Instancing,  new GpuInstancingGrassInstanceContainer());
        }

        public void Draw()
        {
            foreach( var @container in Containers.Values)
            {
                @container.Draw();
            }
        }

        public IGrassSplat AddGrassEntities(GrassEntitiesWithMaterials grassEntitiesWithMaterials)
        {
            return Containers[grassEntitiesWithMaterials.ContainerType].AddGrassEntities(grassEntitiesWithMaterials);
        }


        public void SetGlobalColor(string name, Color value)
        {
            foreach (var @container in Containers.Values)
            {
                @container.SetGlobalColor(name, value);
            }           
        }

        public void SetGlobalUniform(GrassShaderUniformName name, float value)
        {
            foreach (var @container in Containers.Values)
            {
                @container.SetGlobalUniform(name, value);
            }                   
        }

        public void SetGlobalUniform(GrassShaderUniformName name, Vector4 value)
        {
            foreach (var @container in Containers.Values)
            {
                @container.SetGlobalUniform(name, value);
            }               
        }
    }
}
