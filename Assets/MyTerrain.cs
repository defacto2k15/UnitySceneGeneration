using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets
{
    class MyTerrain
    {
        private readonly HeightmapArray _heightmapArray;
        private readonly GameObject _terrainObject;
        private readonly int _terrainXIndex;
        private readonly int _terrainYIndex;
        private readonly MyTotalHeightmap _totalHeightmap;

        public MyTerrain(HeightmapArray inHeightmapArray, MyTotalHeightmap totalHeightmap, int terrainXIndex, int terrainYIndex)
        {
            _terrainXIndex = terrainXIndex;
            _terrainYIndex = terrainYIndex;
            _totalHeightmap = totalHeightmap;
            _heightmapArray = inHeightmapArray;
            SetHeightmapMargins();
            _terrainObject = SubmapPlane.CreatePlaneObject(_heightmapArray.HeightmapAsArray).GameObject;
        }

        public void SetNeighbours()
        {
            Terrain topNeighbour = null;
            Terrain bottomNeighbour = null;
            Terrain leftNeighbour = null;
            Terrain rightNeighbour = null;

            if (_terrainXIndex < _totalHeightmap.heightArrays.GetLength(0) - 1)
            {
                rightNeighbour = _totalHeightmap.heightArrays[_terrainXIndex + 1, _terrainYIndex].Terrain;
            }

            if (_terrainXIndex > 0)
            {
                leftNeighbour = _totalHeightmap.heightArrays[_terrainXIndex - 1, _terrainYIndex].Terrain;
            }

            if (_terrainYIndex < _totalHeightmap.heightArrays.GetLength(1) - 1)
            {
                topNeighbour = _totalHeightmap.heightArrays[_terrainXIndex , _terrainYIndex+1].Terrain;
            }

            if (_terrainYIndex > 0)
            {
                bottomNeighbour = _totalHeightmap.heightArrays[_terrainXIndex, _terrainYIndex-1].Terrain;
            }
            Debug.Log("Setting neighbours " + _terrainXIndex + "  " + _terrainYIndex + " Right " + rightNeighbour + " Left " + leftNeighbour + " Top " + topNeighbour + " Bot " + bottomNeighbour);

            //Terrain.SetNeighbors( leftNeighbour, topNeighbour, rightNeighbour, bottomNeighbour);
            
        }

        public float GetHeight( HeightmapPosition position ){
            var positionOnOurHeightmap = position.GetPositionOnOtherHeightmap( _heightmapArray.WorkingWidth );  
            return _heightmapArray.GetHeight(positionOnOurHeightmap.X, positionOnOurHeightmap.Y);
        }

        public int HeightmapWidth { get { return _heightmapArray.Width; } }

        public Vector3 Position
        {
            set{ _terrainObject.transform.position = value;}
        }

        public Vector3 Scale
        {
            set { _terrainObject.transform.localScale = value; }
        }

        public string Name
        {
            set { _terrainObject.transform.name = value;}
        }

        public Terrain Terrain
        {
            get { return _terrainObject.GetComponent<Terrain>(); }
        }

        public Vector3 Rotation { set { _terrainObject.transform.localEulerAngles = value; } }

        private void SetHeightmapMargins(){
            if( _terrainXIndex < _totalHeightmap.heightArrays.GetLength(0) - 1 ) {
                for( int i = 0; i < _heightmapArray.WorkingWidth; i++){
                    _heightmapArray.SetHeight(i, _heightmapArray.Width - 1,
                        _totalHeightmap.GetHeight( _terrainXIndex+1, _terrainYIndex, new HeightmapPosition(   i, 0, _heightmapArray.WorkingWidth))); 
                }
            }
            if (_terrainYIndex < _totalHeightmap.heightArrays.GetLength(1) - 1)
            {
                for (int i = 0; i < _heightmapArray.WorkingWidth; i++)
                {
                    _heightmapArray.SetHeight( _heightmapArray.Width - 1,  i,
                        _totalHeightmap.GetHeight(_terrainXIndex , _terrainYIndex+1, new HeightmapPosition(  0, i, _heightmapArray.WorkingWidth)));
                }
            }

            // ustawienie wartości na rogu
            if ((_terrainXIndex < _totalHeightmap.heightArrays.GetLength(0) - 1)
                && (_terrainYIndex < _totalHeightmap.heightArrays.GetLength(1) - 1))
            {
                _heightmapArray.SetHeight(_heightmapArray.Width-1, _heightmapArray.Width-1,
                    _totalHeightmap.GetHeight(_terrainXIndex + 1, _terrainYIndex + 1, new HeightmapPosition(0, 0, _heightmapArray.WorkingWidth)));
            }
        }
    }
}
