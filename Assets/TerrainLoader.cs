using UnityEngine;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using Assets;
using Assets.MeshGeneration;

public class TerrainLoader : MonoBehaviour {
    GameObject terrainGameObject;

	// Use this for initialization
    void Start()
    {

        string bilFilePath = @"C:\n49_e019_1arc_v3.bil";
        HeightmapFile heightmapFile = new HeightmapFile();
        heightmapFile.loadFile(bilFilePath, 3601);
        heightmapFile.MirrorReflectHeightDataInXAxis();

        MyTotalHeightmap totalHeightmap = new MyTotalHeightmap();
        totalHeightmap.LoadHeightmap(heightmapFile);
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