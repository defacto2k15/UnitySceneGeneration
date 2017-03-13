using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Grass.Container
{
    class GameObjectGrassInstanceContainer : IGrassInstanceContainer
    {
        private List<GameObject> gameObjects;

        public GameObjectGrassInstanceContainer(List<GameObject> gameObjects)
        {
            this.gameObjects = gameObjects;
        }

        public void Draw()
        {
        }
    }
}
