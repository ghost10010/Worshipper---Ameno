using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LevelEditorManager : MonoBehaviour
{
	// Use this for initialization
	void Start ()
    {
        //create a new terrain data
        TerrainData terrainData = new TerrainData();

        //set terrain width, height, length
        terrainData.size = new Vector3(2048, 1, 2048);
        terrainData.heightmapResolution = 512;
        terrainData.baseMapResolution = 1024;
        terrainData.SetDetailResolution(1024, 16);

        GameObject terrain = Terrain.CreateTerrainGameObject(terrainData);
        terrain.tag = "Terrain";
        terrain.transform.position = new Vector3(-1024.0f, 0.0f, -1024.0f);
        terrain.AddComponent<TerrainDeformer>();
        terrain.AddComponent<TerrainObject>();

        //GameObject spawnedTerrain = (GameObject)Instantiate(terrain, new Vector3(-1024.0f, 0.0f, -1024.0f), Quaternion.identity);
    }

    private void SaveLevel()
    {

    }
}