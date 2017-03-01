using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Assets;
using Assets.MeshGeneration;

public class TerrainLoader : MonoBehaviour {
    GameObject terrainGameObject;

	// Use this for initialization
    void Start()
    {

        string bilFilePath = @"C:\n49_e019_1arc_v3.bil";
        HeightmapFile heightmapFile = new HeightmapFile();
        const int filePixelWidth = 3601;
        heightmapFile.loadFile(bilFilePath, 3601);
        //heightmapFile.MirrorReflectHeightDataInXAxis();

        MyTotalHeightmap totalHeightmap = new MyTotalHeightmap();

        const int subTerrainCount = 14;
        int minSubmapWidth = (int)Math.Floor((double)filePixelWidth/subTerrainCount) ;
        List<SubmapInfo> submapInfos = new List<SubmapInfo>
        {
    //        newSubmapDimension(2,2,2,2,minSubmapWidth, 4),


            newSubmapDimension(0,0,4,4,minSubmapWidth, 6),
            newSubmapDimension(0,4,4,6,minSubmapWidth, 7),
            newSubmapDimension(4,0,6,4,minSubmapWidth, 5), //todo delete
            newSubmapDimension(0,10,4,4,minSubmapWidth, 4),
            newSubmapDimension(4,4,2,2,minSubmapWidth, 2+4),
            newSubmapDimension(4,6,2,2,minSubmapWidth, 2+4),
            newSubmapDimension(4,8,2,2,minSubmapWidth, 2+4),
            newSubmapDimension(6,4,2,2,minSubmapWidth, 2+4),
            newSubmapDimension(6,8,2,2,minSubmapWidth, 2+4),
            newSubmapDimension(6,8,2,2,minSubmapWidth, 2+4),
            newSubmapDimension(8,4,2,2,minSubmapWidth, 2+4),
            newSubmapDimension(8,6,2,2,minSubmapWidth, 2+4),
            newSubmapDimension(8,8,2,2,minSubmapWidth, 2+4),
            newSubmapDimension(6,6,2,2,minSubmapWidth, 2+4),
            newSubmapDimension(10,4,4,6,minSubmapWidth, 3+2),
            newSubmapDimension(10,10,4,4,minSubmapWidth, 4+1),
            newSubmapDimension(4,10,6,4,minSubmapWidth, 3+4),
            newSubmapDimension(10,0,4,4,minSubmapWidth, 3+4),


            //newSubmapDimension(0,0,1,1,minSubmapWidth, 1),
            //newSubmapDimension(0,1,1,1,minSubmapWidth, 1),
            //newSubmapDimension(0,2,1,1,minSubmapWidth, 1),
            //newSubmapDimension(1,0,1,1,minSubmapWidth, 1),
            //newSubmapDimension(1,1,1,1,minSubmapWidth, 1),
            //newSubmapDimension(1,2,1,1,minSubmapWidth, 1),
            //newSubmapDimension(2,0,1,1,minSubmapWidth, 1),
            //newSubmapDimension(2,1,1,1,minSubmapWidth, 1),
            //newSubmapDimension(2,2,1,1,minSubmapWidth, 1),


        };
        float[,] heightFloats = new float[6, 4];
        //SubmapPlane.CreatePlaneObject(heightFloats);
        Func<int, int, int> lodLevelEvaluator = (x, y) => Math.Min( Math.Abs(subTerrainCount/2 - x), Math.Abs(subTerrainCount/2 - y) ) + 1;
        totalHeightmap.LoadHeightmap(heightmapFile, submapInfos, minSubmapWidth, subTerrainCount );
    }

    private SubmapInfo newSubmapDimension(int x, int y, int width, int height, int distanceBase, int lodFactor)
    {
        return new SubmapInfo(x*distanceBase, y*distanceBase, width * distanceBase, height * distanceBase, lodFactor);
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            terrainGameObject.transform.position += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.D))
        {
            terrainGameObject.transform.position += Vector3.right;
        }
        if (Input.GetKey(KeyCode.A))
        {
            terrainGameObject.transform.position += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            terrainGameObject.transform.position += Vector3.down;
        }
	}
}

public class HeightmapWidth
{
    private int smallerWidth;
    public HeightmapWidth( int smallerWidth){
        this.smallerWidth = smallerWidth;
        // todo check if smallerWidth to potega 2
    }

    public int StandardWidth{ get {return smallerWidth;} }
    public int UnityWidth{ get { return smallerWidth + 1; }}
}