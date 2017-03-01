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
        private readonly MyTotalHeightmap _totalHeightmap;
        private readonly SubmapInfo _submapInfo;

        public MyTerrain(HeightmapArray heightmapArray, MyTotalHeightmap totalHeightmap, SubmapInfo submapInfo)
        {
            _heightmapArray = heightmapArray;
            _totalHeightmap = totalHeightmap;
            _submapInfo = submapInfo;
            _terrainObject = SubmapPlane.CreatePlaneObject(_heightmapArray.HeightmapAsArray).GameObject;
        }

        public int HeightmapWidth { get { return _heightmapArray.Width; } }
        public int HeightmapHeight { get { return _heightmapArray.Height; } }

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

        public SubmapInfo SubmapInfo
        {
            get { return _submapInfo; }
        }

    }
}
