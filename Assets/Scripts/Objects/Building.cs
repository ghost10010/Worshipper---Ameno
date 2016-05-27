using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Building : MonoBehaviour
{
    public int buildCostFood = 0;
    public int buildCostWood = 0;
    public int buildCostIron = 0;

    public int upkeepFood = 0;
    public int upkeepWood = 0;
    public int upkeepIron = 0;

    public int increaseFood = 0;
    public int increaseWood = 0;
    public int increaseIron = 0;

    public int score = 0;

    public int increaseAmount = 0;
    public IncreaseType increaseType;

    public string buildingKey = "";
    public string descriptionKey = "";

    public bool canMove = false;

    public BuildingType buildingType;
    private Village village;

    private GameObject particleSpawn;

    public ParticleSystem selectedParticle;

    [SerializeField] private List<GameObject> guardPositions;
    [SerializeField] private List<GameObject> patrolPositions;

    private Renderer[] objectRenderers;

	// Use this for initialization
	void Start ()
    {
        village = GetComponentInParent<Village>();

        objectRenderers = GetComponentsInChildren<Renderer>();

        for (int index = 0; index < this.gameObject.transform.childCount; index++)
        {
            if (this.gameObject.transform.GetChild(index).name == "ParticleSpawn")
            {
                particleSpawn = this.gameObject.transform.GetChild(index).gameObject;
                return;
            }
        }
	}

    void OnMouseDown()
    {
        if (!GameManger.instance.isLevelEditor && !GameManger.instance.isMenu)
        {
            if (!UIManager.instance.IsMouseOverUI() && !canMove)
            {
                if (!GameManger.instance.gameEnded)
                {
                    selectedParticle.Play();

                    UIManager.instance.UnselectVillager("Click");
                    UIManager.instance.SetBuildingInfo(this);
                    village.SelectVillage();
                }
            }
        }
    }
    void OnMouseOver()
    {
        if (!GameManger.instance.isLevelEditor && !GameManger.instance.isMenu)
        {
            if (!UIManager.instance.IsMouseOverUI())
            {
                if (village != null)
                {
                    if (village.owner == GameManger.instance.GetLocalPlayer().playerID)
                    {
                        if (!GameManger.instance.gameEnded)
                        {
                            //UIManager.instance.SetBuildingInfo(this);
                        }
                    }
                }
            }
        }
    }
    void OnMouseExit()
    {
        if (!GameManger.instance.isLevelEditor && !GameManger.instance.isMenu)
        {
            //UIManager.instance.TogglePanelState("BuildingInfo", false);
        }
    }

    public void AddResourcesToWarehouse(string type, int amount)
    {
        switch(type)
        {
            case "Tree":
                village.wood += amount;
                break;
            case "Rock":
                village.iron += amount;
                break;
        }

        UIManager.instance.SetVillageInfo(village);
    }

    public GameObject GetParticleSpawnPoint()
    {
        return particleSpawn;
    }

    public Village GetVillage()
    {
        return village;
    }
    public void SetVillage(Village village)
    {
        this.village = village;
    }


    public List<GameObject> GetPatrolPoints()
    {
        return patrolPositions;
    }
    public List<GameObject> GetGuardPositions()
    {
        return guardPositions;
    }

    public void SetAlpha(float alpha)
    {
        for (int index = 0; index < objectRenderers.Length; index++)
        {
            Color rendereColor = objectRenderers[index].material.color;
            rendereColor.a = alpha;
            objectRenderers[index].material.color = rendereColor;
        }
    }
}