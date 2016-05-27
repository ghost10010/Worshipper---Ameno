using UnityEngine;
using System.Collections;

public class GatherState : VillagerState
{
    public ResourceType gatherType;
    public ResourcePoint resourcePoint;

    public float gatherTimer = 5.0f;
    public float gatherTime = 5.0f;

    public int gatherAmount = 5;

    public bool gatherd = false;

    void Start()
    {
        base.villager = GetComponentInParent<VillagerController>();
        GetComponentInParent<VillagerController>().AddState(this);
    }

    public override void InitState()
    {
        villager.minDistance = 5.0f;

        if (!isInit && !villager.GetTrigger("IsFinished"))
        {
            GetResourcePoint();

            if (resourcePoint != null)
            {
                villager.moveTarget = resourcePoint.transform.position;
                isInit = true;
                isActive = true;
                
            }
            else
            {
                isActive = false;
                isInit = false;

                villager.SetBool("Gather", false);
                villager.ResetActiveState();
            }
        }
    }
    public override void Action()
    {
        if (Mathf.RoundToInt(Vector3.Distance(this.transform.position, villager.moveTarget)) > villager.minDistance)
        {
            villager.SetBool("OnMove", true);
            villager.ResetActiveState();
        }
    }

    public void Gather()
    {
        if (Mathf.RoundToInt(Vector3.Distance(this.transform.position, villager.moveTarget)) <= villager.minDistance)
        {
            gatherTimer -= Time.deltaTime;
            if (gatherTimer <= 0.0f) //resources gatherd
            {
                GameObject spawnedObject = (GameObject)Instantiate(ObjectManager.instance.GetCratePrefab(), resourcePoint.gameObject.transform.position, Quaternion.identity);
                Crate crateController = spawnedObject.GetComponent<Crate>();
                crateController.resourceType = gatherType;
                crateController.amount = gatherAmount;
                resourcePoint.GatherResources(gatherAmount);

                AddCrateToVillage(spawnedObject);

                gatherd = true;
            }
        }
    }

    public override bool Condition()
    {
        if (Mathf.RoundToInt(Vector3.Distance(this.transform.position, villager.moveTarget)) <= villager.minDistance)
        {
            Gather();
        }

        return gatherd;
    }

    public override void StateUpdate()
    {
        if (Condition())
        {
            isActive = false;
            isInit = false;
            gatherd = false;
            gatherTimer = gatherTime;

            villager.SetBool("Gather", false);
            villager.SetTrigger("IsFinished");
            villager.ResetActiveState();
            villager.SetActivityTimer();
        }
        else
        {
            Action();
        }
    }

    private void GetResourcePoint()
    {
        if (!IsVillageNull())
        {
            gatherType = villager.GetVillage().GetNeededResource();
            resourcePoint = villager.GetVillage().GetResourcePointOfType(gatherType);
        }
        else
        {
            gatherType = villager.GetNonNetworkedVillage().GetNeededResource();
            resourcePoint = villager.GetNonNetworkedVillage().GetResourcePointOfType(gatherType);
        }
    }
    private void AddCrateToVillage(GameObject spawnedObject)
    {
        if (!IsVillageNull())
        {
            villager.GetVillage().AddCratePosition(spawnedObject);
        }
        else
        {
            villager.GetNonNetworkedVillage().AddCratePosition(spawnedObject);
        }
    }
}