using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ObjectManager : MonoBehaviour
{
    public static ObjectManager instance = null;

    public List<GameObject> terrains;

    public GameObject villagePrefab;
    public GameObject emptyVillagePrefab;
    public GameObject villagerPrefab;
    public GameObject soldierPrefab;
    public List<GameObject> terrainPrefabs;
    public GameObject emptyTerrainPrefab;

    public List<GameObject> buildingPrefabs;
    public List<GameObject> buildingTransPrefab;
    public List<GameObject> crates = new List<GameObject>();

    public GameObject spawnParticle;
    public GameObject woodParticle;
    public GameObject ironParticle;
    public GameObject manaParticle;
    public GameObject foodParticle;

    public List<GameObject> treePrefabs;
    public List<GameObject> rockPrefabs;
    public List<GameObject> bushPrefabs;

    public Color selectedColor;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
    }

    public GameObject GetCratePrefab()
    {
        return crates[Random.Range(0, crates.Count)];
    }

    public GameObject GetVillagePrefab()
    {
        if (Settings.instance.LoadFromSave)
        {
            return emptyVillagePrefab;
        }

        return villagePrefab;
    }

    public GameObject GetTerrainPrefab()
    {
        if (Settings.instance.LoadFromSave)
        {
            return emptyTerrainPrefab;
        }

        return terrainPrefabs[Random.Range(0, ObjectManager.instance.terrainPrefabs.Count)];
    }

    public GameObject GetObjectOfType(ResourceType type, int Id)
    {
        List<GameObject> prefabs = new List<GameObject>();

        switch(type)
        {
            case ResourceType.Food:
                prefabs = bushPrefabs;
                break;
            case ResourceType.Iron:
                prefabs = rockPrefabs;
                break;
            case ResourceType.Wood:
                prefabs = treePrefabs;
                break;
        }

        for (int index = 0; index < prefabs.Count; index++)
        {
            string name = prefabs[index].name;
            string idString = name.Substring(name.Length - 1);
            int id = -1;

            if (int.TryParse(idString, out id))
            {
                if (id == Id)
                {
                    return prefabs[index];
                }
            }
        }

        return null;
    }
}