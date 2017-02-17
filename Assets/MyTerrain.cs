using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class MyTerrain
    {
        private HeightmapArray heightmapArray;
        private GameObject terrainObject;
        private int terrainXIndex;
        private int terrainYIndex;
        private MyTotalHeightmap totalHeightmap;

        public MyTerrain(HeightmapArray inHeightmapArray, MyTotalHeightmap totalHeightmap, int terrainXIndex, int terrainYIndex, int terrainObjectWidth, int? lodFactor = null)
        {
            this.terrainXIndex = terrainXIndex;
            this.terrainYIndex = terrainYIndex;
            this.totalHeightmap = totalHeightmap;
            if (lodFactor != null)
            {
                this.heightmapArray = inHeightmapArray.simplyfy(lodFactor.GetValueOrDefault(0)); ;
            }
            else
            {
                this.heightmapArray = inHeightmapArray;
            }

            TerrainData data = new TerrainData();
            data.size = new Vector3(terrainObjectWidth / 2 * heightmapArray.UnityBaseHeightmapWidth, terrainObjectWidth *5, terrainObjectWidth / 2 * heightmapArray.UnityBaseHeightmapWidth);
            data.heightmapResolution = heightmapArray.Width;

            setHeightmapMargins();

            data.SetHeights(0, 0, heightmapArray.HeightmapAsArray);

            var terrainGameObject = Terrain.CreateTerrainGameObject(data);
            terrainGameObject.transform.position = new Vector3(0, 0, 0);
            var terrainComponent = terrainGameObject.GetComponent<Terrain>();

            terrainComponent.heightmapPixelError = 10;

            terrainComponent.heightmapMaximumLOD = 0;
            terrainComponent.Flush();
            terrainObject =  terrainGameObject;
        }

        public void SetNeighbours()
        {
            Terrain topNeighbour = null;
            Terrain bottomNeighbour = null;
            Terrain leftNeighbour = null;
            Terrain rightNeighbour = null;

            if (terrainXIndex < totalHeightmap.heightArrays.GetLength(0) - 1)
            {
                rightNeighbour = totalHeightmap.heightArrays[terrainXIndex + 1, terrainYIndex].Terrain;
            }

            if (terrainXIndex > 0)
            {
                leftNeighbour = totalHeightmap.heightArrays[terrainXIndex - 1, terrainYIndex].Terrain;
            }

            if (terrainYIndex < totalHeightmap.heightArrays.GetLength(1) - 1)
            {
                topNeighbour = totalHeightmap.heightArrays[terrainXIndex , terrainYIndex+1].Terrain;
            }

            if (terrainYIndex > 0)
            {
                bottomNeighbour = totalHeightmap.heightArrays[terrainXIndex, terrainYIndex-1].Terrain;
            }
            Debug.Log("Setting neighbours " + terrainXIndex + "  " + terrainYIndex + " Right " + rightNeighbour + " Left " + leftNeighbour + " Top " + topNeighbour + " Bot " + bottomNeighbour);

            //Terrain.SetNeighbors( leftNeighbour, topNeighbour, rightNeighbour, bottomNeighbour);
            
        }

        public float GetHeight( HeightmapPosition position ){
            var positionOnOurHeightmap = position.GetPositionOnOtherHeightmap( heightmapArray.WorkingWidth );  
            return heightmapArray.GetHeight(positionOnOurHeightmap.X, positionOnOurHeightmap.Y);
        }

        public int HeightmapWidth { get { return heightmapArray.Width; } }

        public Vector3 Position
        {
            set{ terrainObject.transform.position = value;}
        }

        public string Name
        {
            set { terrainObject.transform.name = value;}
        }

        public Terrain Terrain
        {
            get { return terrainObject.GetComponent<Terrain>(); }
        }

        private void setHeightmapMargins(){
            if( terrainXIndex < totalHeightmap.heightArrays.GetLength(0) - 1 ) {
                for( int i = 0; i < heightmapArray.WorkingWidth; i++){
                    heightmapArray.SetHeight(i, heightmapArray.Width - 1,
                        totalHeightmap.GetHeight( terrainXIndex+1, terrainYIndex, new HeightmapPosition(   i, 0, heightmapArray.WorkingWidth))); 
                }
            }
            if (terrainYIndex < totalHeightmap.heightArrays.GetLength(1) - 1)
            {
                for (int i = 0; i < heightmapArray.WorkingWidth; i++)
                {
                    heightmapArray.SetHeight( heightmapArray.Width - 1,  i,
                        totalHeightmap.GetHeight(terrainXIndex , terrainYIndex+1, new HeightmapPosition(  0, i, heightmapArray.WorkingWidth)));
                }
            }

            // ustawienie wartości na rogu
            if ((terrainXIndex < totalHeightmap.heightArrays.GetLength(0) - 1)
                && (terrainYIndex < totalHeightmap.heightArrays.GetLength(1) - 1))
            {
                heightmapArray.SetHeight(heightmapArray.Width-1, heightmapArray.Width-1,
                    totalHeightmap.GetHeight(terrainXIndex + 1, terrainYIndex + 1, new HeightmapPosition(0, 0, heightmapArray.WorkingWidth)));
            }
        }
    }
}
