using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class MyTotalHeightmap
    {
        public MyTerrain[,] heightArrays; // todo use property

        public void LoadHeightmap(HeightmapFile heightmapFile)
        {
            var wholeTerrainWidth = 100000;
            int subTerrainCount = 14;
            int subterrainWidth = wholeTerrainWidth / subTerrainCount;

            heightArrays = new MyTerrain[subTerrainCount, subTerrainCount];
            HeightmapWidth segmentWidth = new HeightmapWidth(256);

            for (int i = subTerrainCount -1; i >= 0; i--) // reverse counting to take margins into consideration
            {
                for (int j = subTerrainCount - 1; j >= 0; j--)
                {
                    var lodFactor = 4;

                    var segmentHeightmapInFullResolution = heightmapFile.getHeightSubmap(i * segmentWidth.StandardWidth, j * segmentWidth.StandardWidth, segmentWidth.StandardWidth );
                    var simplifiedSegmentHeightmap = segmentHeightmapInFullResolution.simplyfy(lodFactor);

                    var terrain = new MyTerrain(simplifiedSegmentHeightmap, this, i, j);

                    terrain.Position = (new Vector3((i - subTerrainCount/2)*subterrainWidth, 0,
                        (j - subTerrainCount/2)*subterrainWidth));
                    terrain.Scale = new Vector3(subterrainWidth, subterrainWidth, subterrainWidth);
                    terrain.Rotation = new Vector3(0, 90, 0);

                    terrain.Name = ("Terrain " + i + " : " + j + " sRes " + terrain.HeightmapWidth);
                    heightArrays[i, j] = terrain;
                }
            }

            for (int i = subTerrainCount - 1; i >= 0; i--) // reverse counting to take margins into consideration
            {
                for (int j = subTerrainCount - 1; j >= 0; j--)
                {
                    heightArrays[i,j].SetNeighbours();
                }
            }

        }

        public float GetHeight(int terrainXPos, int terrainYPos, HeightmapPosition position)
        {
            return heightArrays[terrainXPos, terrainYPos].GetHeight(position);
        }

        private GameObject createTerrainGameObject( HeightmapArray heightmapArray)
        {
            TerrainData data = new TerrainData();
            data.size = new Vector3(50 * heightmapArray.UnityBaseHeightmapWidth, 1000, 50 * heightmapArray.UnityBaseHeightmapWidth);
            data.heightmapResolution = heightmapArray.Width;


            data.SetHeights(0, 0, heightmapArray.HeightmapAsArray);

            var terrainGameObject = Terrain.CreateTerrainGameObject(data);
            terrainGameObject.transform.position = new Vector3(0, 0, 0);
            terrainGameObject.GetComponent<Terrain>().Flush();
            //terrainGameObject.name = "Terrain " + i + " : " + j + " sRes " + heightmapArray.Width;
            return terrainGameObject;
        }

    }
}
