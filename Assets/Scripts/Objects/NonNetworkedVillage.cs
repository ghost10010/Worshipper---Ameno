using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class NonNetworkedVillage : MonoBehaviour
{
    public int food = 100;
    public int wood = 100;
    public int iron = 100;
    public int maxFood = 250;
    public int maxWood = 250;
    public int maxIron = 250;

    public int increaseFood = 1;
    public int increaseWood = 1;
    public int increaseIron = 1;
    public int increaseMana = 2;

    public int decreaseFood = 1;
    public int decreaseWood = 1;
    public int decreaseIron = 1;

    public int owner;

    public int villagers = 0;
    public int startVillagers = 8;
    public int maxVillagers = 15;
    public int soldiers = 0;
    public int maxSoldiers = 3;

    private int currentVillagerSpawn = 0;

    public List<GameObject> buildings = new List<GameObject>();
    public List<GameObject> guardPositions = new List<GameObject>();
    public List<GameObject> patrolPositions = new List<GameObject>();
    public List<GameObject> cratePositions = new List<GameObject>();
    public List<GameObject> spawnPositions = new List<GameObject>();
    public List<VillagerController> villagerControllers = new List<VillagerController>();
    public List<ResourcePoint> resourceObjects = new List<ResourcePoint>();

    public GameObject villageCenter;
    public GameObject resourceParent;

    public bool isSelected = false;
    public bool isInitilized = false;
    private bool isAttacking = false;

    public Village target;

    void Update()
    {
        for (int index = 0; index < villagerControllers.Count; index++)
        {
            villagerControllers[index].ServerUpdate();
        }
    }

    public void AddResource(ResourceType resource, int amount)
    {
        switch (resource)
        {
            case ResourceType.Wood:
                wood += amount;
                break;
            case ResourceType.Food:
                food += amount;
                break;
            case ResourceType.Iron:
                iron += amount;
                break;
        }
    }
    //public void AddResourcesNetworked(ResourceType resource, int amount)
    //{
    //    NetworkController netController = GameManger.instance.GetLocalNetworkController();
    //    netController.AddResourcesToVillage(this.gameObject, amount, resource);
    //}

    public ResourceType GetNeededResource()
    {
        int randomResource = Random.Range(0, (int)ResourceType.None);

        return (ResourceType)randomResource;
    }

    public bool HasStoragePlace(ResourceType type)
    {
        bool hasStorage = false;

        switch (type)
        {
            case ResourceType.Food:
                hasStorage = ResourceStorageFull(maxFood, food);
                break;
            case ResourceType.Iron:
                hasStorage = ResourceStorageFull(maxIron, iron);
                break;
            case ResourceType.Wood:
                hasStorage = ResourceStorageFull(maxWood, wood);
                break;
        }

        return hasStorage;
    }
    private bool ResourceStorageFull(int capacity, int amount)
    {
        if (capacity > amount)
        {
            return true;
        }
        return false;
    }

    public void SetOwner(int ownerID)
    {
        owner = ownerID;
    }
    public int GetOwner()
    {
        return owner;
    }

    public void SelectVillage()
    {
        if (GameManger.instance.GetLocalPlayer().playerID == owner)
        {
            //GameManger.instance.SelectVillage(this);
        }
        else
        {
            GameManger.instance.DeselectVillage();
        }
    }

    //Attacking
    public void AttackVillage(Village target)
    {
        Debug.Log("Target: " + target.gameObject.name);
        this.target = target;

        isAttacking = true;

        Debug.Log("AttackVillage");
        for (int index = 0; index < villagerControllers.Count; index++)
        {
            Debug.Log("Villager: " + index);
            
            villagerControllers[index].SetIdle();
            villagerControllers[index].SetBool("IsAttackingVillage", true);
        }

        //reset soldiers
    }
    public bool isAttackingVillage()
    {
        return isAttacking;
    }

    //Lists
    public void AddBuilding(GameObject building)
    {
        buildings.Add(building);
    }
    public GameObject GetResourceCrate()
    {
        if (cratePositions.Count > 0)
        {
            GameObject crate = cratePositions[0];
            cratePositions.RemoveAt(0);

            return crate;
        }

        return null;
    }
    public bool HasResourceCrate()
    {
        if (cratePositions.Count > 0)
        {
            return true;
        }
        return false;
    }
    public GameObject GetWarehouse()
    {
        foreach (GameObject building in buildings)
        {
            if (building.tag == "Warehouse")
            {
                return building;
            }
        }

        return null;
    }
    public GameObject GetGuardPoint()
    {
        if (guardPositions.Count > 0)
        {
            GameObject nextGuardPoint = guardPositions[0];
            guardPositions.RemoveAt(0);

            return nextGuardPoint;
        }
        
        return null;
    }
    public bool HasGuardPoint()
    {
        if (guardPositions.Count > 0)
        {
            return true;
        }

        return false;
    }
    public List<GameObject> GetPatrolPoints()
    {
        return patrolPositions;
    }
    public Vector3 GetVillagerSpawnPosition()
    {
        Vector3 returnPosition = spawnPositions[currentVillagerSpawn].transform.position;

        currentVillagerSpawn++;

        if (currentVillagerSpawn >= spawnPositions.Count)
        {
            currentVillagerSpawn = 0;
        }

        return returnPosition;
    }

    public void AddCratePosition(GameObject newPoint)
    {
        cratePositions.Add(newPoint);
    }
    public void AddPatrolPoint(GameObject newPoint)
    {
        patrolPositions.Add(newPoint);
    }
    public void AddGuardPoint(GameObject newPoint)
    {
        guardPositions.Add(newPoint);
    }

    public void RemoveObject(ResourcePoint resourceObject)
    {
        resourceObjects.Remove(resourceObject);
    }
    public void AddObject(ResourcePoint resourceObject)
    {
        resourceObjects.Add(resourceObject);
    }
    public ResourcePoint GetResourcePointOfType(ResourceType type)
    {
        for (int index = 0; index < resourceObjects.Count; index++)
        {
            if (resourceObjects[index].type == type)
            {
                return resourceObjects[index];
            }
        }

        return null;
    }

    public void AddVillagerController(VillagerController controller)
    {
        villagerControllers.Add(controller);

        if (controller.GetVillagerType() == VillagerType.Soldier)
        {
            soldiers++;
        }
        else
        {
            villagers++;
        }
    }
    public List<VillagerController> GetVillagerControllers()
    {
        return villagerControllers;
    }
}