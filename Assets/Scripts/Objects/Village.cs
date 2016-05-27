using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using System.Linq;

public class Village : NetworkBehaviour
{
    [SyncVar(hook = "OnFoodChange")] public int food = 100;
    [SyncVar(hook = "OnWoodChange")] public int wood = 100;
    [SyncVar(hook = "OnIronChange")] public int iron = 100;
    [SyncVar] public int maxFood = 250;
    [SyncVar] public int maxWood = 250;
    [SyncVar] public int maxIron = 250;

    public int increaseFood = 1;
    public int increaseWood = 1;
    public int increaseIron = 1;
    public int increaseMana = 2;

    public int decreaseFood = 1;
    public int decreaseWood = 1;
    public int decreaseIron = 1;

    [SerializeField] private int spawnTreshold = 25;

    [SyncVar] public int owner;

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
    public bool isMenu = false;

    public Village target;
    private GameObject terrainObject;
    private NetworkController netController;

    void Update()
    {
        if (isMenu)
        {
            return;
        }

        if (!isInitilized)
        {
            if (terrainObject != null)
            {
                if (netController != null)
                {
                    if (netController.isServer)
                    {
                        InitVillage();
                    }
                }
                else
                {
                    netController = GameManger.instance.GetLocalNetworkController();
                }
            }
            else
            {
                terrainObject = GameObject.FindGameObjectWithTag("Terrain");
            }
        }
        else
        {
            if (netController != null)
            {
                if (netController.isServer)
                {
                    for (int index = 0; index < villagerControllers.Count; index++)
                    {
                        villagerControllers[index].ServerUpdate();
                    }
                }
            }
        }
    }

    public void InitVillage()
    {
        for (int index = 0; index < startVillagers; index++)
        {
            SetNewVillagers(false, false);
        }

        isInitilized = true;
    }

    public void OnFoodChange(int value)
    {
        if (hasAuthority)
        {
            food = value;
        
            UIManager.instance.SetVillageInfo(this);
        }
    }
    public void OnWoodChange(int value)
    {
        if (hasAuthority)
        {
            wood = value;

            UIManager.instance.SetVillageInfo(this);
        }
    }
    public void OnIronChange(int value)
    {
        if (hasAuthority)
        {
            iron = value;

            UIManager.instance.SetVillageInfo(this);
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

        LimitResources();
    }
    public void AddResourcesNetworked(ResourceType resource, int amount, int playerId, Vector3 particlePosition)
    {
        NetworkController netController = GameManger.instance.GetLocalNetworkController();
        netController.AddResourcesToVillage(this.gameObject, amount, resource, playerId, particlePosition);
    }

    public ResourceType GetNeededResource()
    {
        int[] resourceChanges = new int[4];
        resourceChanges[0] = increaseWood - decreaseWood;
        resourceChanges[1] = increaseIron - decreaseIron;
        resourceChanges[2] = increaseFood - decreaseFood;

        int maxIndex = -1;
        int maxValue = 0;
        for (int index = 0; index < resourceChanges.Length; index++)
        {
            if (resourceChanges[index] < 0)
            {
                    if (Mathf.Abs(resourceChanges[index]) > maxValue)
                    {
                        maxValue = resourceChanges[index];
                        maxIndex = index;
                    }   
            }
        }

        if (maxIndex != -1)
        {
            //Debug.Log("Needed: " + (ResourceType)maxIndex);
            return (ResourceType)maxIndex;
        }

        int random = Random.Range(0, 2);

        //Debug.Log("No Needed: " + (ResourceType)random);

        return (ResourceType) random;
    }

    public void UpdateVillage()
    {
        SetNewVillagers(true, true);
        SetResourceIncrease();
        SetResourceDemand();

        food += (increaseFood - decreaseFood);
        wood += (increaseWood - decreaseWood);
        iron += (increaseIron - decreaseIron);

        LimitResources();

        if (isSelected)
        {
            UIManager.instance.SetVillageInfo(this);
            UIManager.instance.UpdateBuildingsButtonsState();
        }
    }
    public void UpdateVillagersLook()
    {
        for (int index = 0; index < villagerControllers.Count; index++)
        {
            villagerControllers[index].SetVillagerLook();
        }
    }
    private void SetNewVillagers(bool spawnParticle, bool score)
    {
        NetworkController netController = GameManger.instance.GetLocalNetworkController();
        Player player = GameManger.instance.GetLocalPlayer();

        if (soldiers < maxSoldiers)
        {
            if (iron >= spawnTreshold)
            {
                netController.SpawnVillager(VillagerType.Soldier, this.gameObject, spawnParticle, owner);

                if (score)
                {
                    netController.AddScoreToPlayer(GameManger.instance.GetPlayerWithId(owner).gameObject, 10);
                }
            }
        }
        else if (villagers < maxVillagers)
        {
            if (food >= spawnTreshold)
            {
                netController.SpawnVillager(VillagerType.Villager, this.gameObject, spawnParticle, owner);

                if (score)
                {
                    netController.AddScoreToPlayer(GameManger.instance.GetPlayerWithId(owner).gameObject, 5);
                }
            }
        }
    }
    private void SetResourceIncrease()
    {
        increaseFood = 0;
        increaseWood = 0;
        increaseIron = 0;

        for (int index = 0; index < buildings.Count; index++)
        {
            Building currentBuilding = buildings[index].GetComponent<Building>();
            increaseFood += currentBuilding.increaseFood;
            increaseWood += currentBuilding.increaseWood;
            increaseIron += currentBuilding.increaseIron;
        }
    }
    private void SetResourceDemand()
    {
        decreaseFood = 0;
        decreaseWood = 0;
        decreaseIron = 0;

        for (int index = 0; index < villagerControllers.Count; index++)
        {
            decreaseFood += villagerControllers[index].upkeepFood;
            decreaseWood += villagerControllers[index].upkeepWood;
            decreaseIron += villagerControllers[index].upkeepIron;
        }

        for (int index = 0; index < buildings.Count; index++)
        {
            Building currentBuilding = buildings[index].GetComponent<Building>();
            decreaseFood += currentBuilding.upkeepFood;
            decreaseWood += currentBuilding.upkeepWood;
            decreaseIron += currentBuilding.upkeepIron;
        }
    }
    private void LimitResources()
    {
        if (food > maxFood)
        {
            food = maxFood;
        }
        if (wood > maxWood)
        {
            wood = maxWood;
        }
        if (iron > maxIron)
        {
            iron = maxIron;
        }

        if (food < 0)
        {
            food = 0;
        }
        if (wood < 0)
        {
            wood = 0;
        }
        if (iron < 0)
        {
            iron = 0;
        }
    }

    private void CheckAttackPossible()
    {
        if (isAttacking)
        {
            if (soldiers == 0)
            {
                isAttacking = false;
            }
        }
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

    public void OnMouseDown()
    {
        if (!UIManager.instance.IsMouseOverUI())
        {
            if (!GameManger.instance.gameEnded)
            {
                UIManager.instance.UnselectVillager("Click");
                UIManager.instance.UnselectBuilding(); // .TogglePanelState("BuildingInfo", false);
                SelectVillage();
            }
        }
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
            GameManger.instance.SelectVillage(this);
        }
        else
        {
            GameManger.instance.DeselectVillage();
        }
    }

    //Attacking
    public void AttackVillage(Village target)
    {
        this.target = target;

        isAttacking = true;

        for (int index = 0; index < villagerControllers.Count; index++)
        {
            
            villagerControllers[index].SetIdle();
            villagerControllers[index].SetBool("IsAttackingVillage", true);
        }
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
        List<ResourcePoint> possiblePoints = new List<ResourcePoint>();

        for (int index = 0; index < resourceObjects.Count; index++)
        {
            if (resourceObjects[index].type == type)
            {
                //if (!resourceObjects[index].isInUse)
                //{
                possiblePoints.Add(resourceObjects[index]);
                    //resourceObjects[index].isInUse = true;
                    //return resourceObjects[index];
                //}
            }
        }

        if (possiblePoints.Count > 0)
        {
            return possiblePoints[Random.Range(0, possiblePoints.Count)];
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

    //Range
    public bool IsInRange(Vector3 position, float objectRange)
    {
        for (int index = 0; index < buildings.Count; index++)
        {
            float distance = Vector3.Distance(buildings[index].transform.position, position);

            if (distance <= objectRange)
            {
                return true;
            }
        }

        return false;
    }
}