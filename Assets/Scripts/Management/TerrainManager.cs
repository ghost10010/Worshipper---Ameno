using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class TerrainManager : MonoBehaviour
{
    private Terrain terrain;

    // Use this for initialization
    void Start ()
    {

    }
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    public void initTerrain(int terrainID)
    {
        GameObject spawnedTerrain = (GameObject)Instantiate(ObjectManager.instance.terrains[terrainID], ObjectManager.instance.terrains[terrainID].transform.position, Quaternion.identity);
        terrain = spawnedTerrain.GetComponent<Terrain>();
    }
}
