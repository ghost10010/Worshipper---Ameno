using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class PatrolState : VillagerState
{
    public List<GameObject> wayPoints = new List<GameObject>();

    public int currentWayPoint = 0;

    public float distance = 0.0f;

    void Start()
    {
        GetComponentInParent<VillagerController>().AddState(this);
    }

    public override void InitState()
    {
        GetPatrolPoints();

        if (!isInit)
        {
            villager.moveTarget = wayPoints[currentWayPoint].transform.position;
        }
        
        villager.minDistance = 2.0f;

        isInit = true;
        isActive = true;
    }

    public override void Action()
    {
        distance = Vector3.Distance(this.transform.position, villager.moveTarget);

        if (Mathf.RoundToInt(Vector3.Distance(this.transform.position, villager.moveTarget)) > villager.minDistance)
        {
            villager.SetBool("OnMove", true);
            villager.ResetActiveState();
        }
    }
    public override bool Condition()
    {
        if (Mathf.RoundToInt(Vector3.Distance(this.transform.position, villager.moveTarget)) <= villager.minDistance) //reached waypoint
        {
            return true;
        }

        return false;
    }

    public override void StateUpdate()
    {
        if (Condition()) // reached target
        {
            SetNextPointID();
            villager.moveTarget = wayPoints[currentWayPoint].transform.position;
        }
        else
        {
            Action();
        }
    }

    public void SetNextPointID()
    {
        currentWayPoint++;

        if (currentWayPoint >= wayPoints.Count)
        {
            currentWayPoint = 0;
        }
    }

    private void GetPatrolPoints()
    {
        if (!IsVillageNull())
        {
            wayPoints = villager.GetVillage().GetPatrolPoints();
        }
        else
        {
            wayPoints = villager.GetNonNetworkedVillage().GetPatrolPoints();
        }
    }
}