using UnityEngine;
using System.Collections;

public class StoreState : VillagerState
{
    public GameObject pickup;
    public GameObject warehouse;

    public bool reachedPickup = false;
    public bool reachedWarehouse = false;

    public Crate crate;

    public Vector3 currentDestination;
    public int conditionDistance = 0;
    public float distance = 0.0f;

	void Start ()
    {
        GetComponentInParent<VillagerController>().AddState(this);
    }

    public override void InitState()
    {
        if (!isInit && !villager.GetTrigger("IsFinished"))
        {
            isInit = true;

            if (!villager.isStoring)
            {
                if (pickup == null)
                {
                    villager.isStoring = true;
                    pickup = GetResourceCrate();
                }
            }
            
            if (warehouse == null)
            {
                warehouse = GetWarehouse();
            }

            if (!reachedPickup)
            {
                if (pickup != null)
                {
                    villager.moveTarget = pickup.transform.position;
                    currentDestination = pickup.transform.position;
                    villager.minDistance = 10.0f;
                    conditionDistance = 10;
                }
                else
                {
                    villager.SetIdle();
                }
            }
            else
            {
                villager.moveTarget = warehouse.transform.position;
                currentDestination = warehouse.transform.position;
                villager.minDistance = 10.0f;
                conditionDistance = 10;
            }

            isActive = true;
        }
    }

    public override void Action()
    {
        distance = Vector3.Distance(this.transform.position, villager.moveTarget);

        if (Mathf.RoundToInt(Vector3.Distance(this.transform.position, villager.moveTarget)) > conditionDistance)
        {
            isActive = false;
            isInit = false;

            villager.SetBool("OnMove", true);
            villager.ResetActiveState();
        }
    }
    public override bool Condition()
    {
        if (Vector3.Distance(this.transform.position, currentDestination) <= conditionDistance)
        {
            return true;
        }

        return false;
    }

    public override void StateUpdate()
    {
        if (Condition() && !villager.GetTrigger("IsFinished"))
        {
            if (!reachedPickup)
            {
                reachedPickup = true;

                crate = pickup.GetComponent<Crate>();
                Destroy(pickup);

                currentDestination = warehouse.transform.position;
                villager.moveTarget = warehouse.transform.position;
                villager.minDistance = 10.0f;
            }
            else if (!reachedWarehouse)
            {
                Vector3 particlePosition = warehouse.GetComponent<Building>().GetParticleSpawnPoint().transform.position;

                if (crate != null)
                {
                    AddResourcesNetworked(crate.resourceType, crate.amount, villager.GetVillage().owner, particlePosition);
                }

                reachedWarehouse = true;

                villager.isStoring = false;
                villager.SetBool("Store", false);
                villager.SetBool("IsFinished", true);
                villager.ResetActiveState();
                villager.SetActivityTimer();

                crate = null;

                isActive = false;
                isInit = false;

                reachedPickup = false;
                reachedWarehouse = false;
            }
        }
        else
        {
            Action();
        }
    }

    private GameObject GetResourceCrate()
    {
        if (!IsVillageNull())
        {
            return villager.GetVillage().GetResourceCrate();
        }
        else
        {
            return villager.GetNonNetworkedVillage().GetResourceCrate();
        }
    }
    private GameObject GetWarehouse()
    {
        if(!IsVillageNull())
        {
            return villager.GetVillage().GetWarehouse();
        }
        else
        {
            return villager.GetNonNetworkedVillage().GetWarehouse();
        }
    }
    private void AddResourcesNetworked(ResourceType type, int amount, int playerId, Vector3 particlePosition)
    {
        if (!IsVillageNull())
        {
            villager.GetVillage().AddResourcesNetworked(type, amount, playerId, particlePosition);
        }
        else
        {
            villager.GetNonNetworkedVillage().AddResource(type, amount);
        }
    }
}
