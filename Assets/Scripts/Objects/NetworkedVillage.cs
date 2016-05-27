using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class NetworkedVillage : MonoBehaviour
{
    /*[SyncVar(hook = "OnFoodChange")] public new int food = 100;
    [SyncVar(hook = "OnWoodChange")] public new int wood = 100;
    [SyncVar(hook = "OnIronChange")] public new int iron = 100;
    [SyncVar] public new int maxFood = 250;
    [SyncVar] public new int maxWood = 250;
    [SyncVar] public new int maxIron = 250;

    [SyncVar] public new int owner;

    void Update()
    {
        NetworkController netController = GameManger.instance.GetLocalNetworkController();
        GameObject terrainObject = GameObject.FindGameObjectWithTag("Terrain");

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
            }
        }
        else
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

    public void InitVillage()
    {
        for (int index = 0; index < startVillagers; index++)
        {
            if (maxSoldiers > soldiers)
            {
                SpawnVillager(VillagerType.Soldier);
            }
            else
            {
                SpawnVillager(VillagerType.Villager);
            }
        }

        isInitilized = true;
    }
    public void SpawnVillager(VillagerType type)
    {
        NetworkController netController = GameManger.instance.GetLocalNetworkController();
        if (netController != null)
        {
            netController.SpawnVillager(type, this.gameObject);
        }
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
    public void AddResourcesNetworked(ResourceType resource, int amount)
    {
        NetworkController netController = GameManger.instance.GetLocalNetworkController();
        netController.AddResourcesToVillage(this.gameObject, amount, resource);
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
            return (ResourceType)maxIndex;
        }

        return ResourceType.Wood;
    }

    private void SetNewVillagers()
    {
        NetworkController netController = GameManger.instance.GetLocalNetworkController();

        if (soldiers < maxSoldiers)
        {
            netController.SpawnVillager(VillagerType.Soldier, this.gameObject);
            netController.AddScoreToPlayer(GameManger.instance.GetPlayerWithId(owner).gameObject, 10);
        }
        else if (villagers < maxVillagers)
        {
            netController.SpawnVillager(VillagerType.Villager, this.gameObject);
            netController.AddScoreToPlayer(GameManger.instance.GetPlayerWithId(owner).gameObject, 5);
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

    private bool ResourceStorageFull(int capacity, int amount)
    {
        if (capacity > amount)
        {
            return true;
        }
        return false;
    }*/
}