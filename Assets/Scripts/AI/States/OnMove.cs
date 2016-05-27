using UnityEngine;
using System.Collections;

public class OnMove : VillagerState
{
    public float distance = 0.0f;
    public Vector3 current;
    public Vector3 target;

    public bool destinationReached = false;

    public NavMeshAgent navAgent;

    void Start()
    {
        GetComponentInParent<VillagerController>().AddState(this);
    }

    public override void InitState()
    {
        navAgent.speed = villager.walkSpeed;

        if (!Condition())
        {
            //Debug.Log("InitState Move: " + villager.moveTarget);
            navAgent.SetDestination(villager.moveTarget);
            navAgent.Resume();

            isActive = true;
            isInit = true;
        }
        
    }

    public override void Action()
    {
        //Debug.Log(navAgent.destination + " | " + villager.minDistance + " | " + villager.moveTarget + " | " + Vector3.Distance(this.transform.position, villager.moveTarget));
        distance = Vector3.Distance(this.transform.position, villager.moveTarget);
        current = this.transform.position;
        target = villager.moveTarget;
    }
    public override bool Condition()
    {
        //Debug.Log("ConditionMove");
        if (Vector3.Distance(this.transform.position, villager.moveTarget) <= villager.minDistance)
        {
            return true;
        }

        return false;
    }

    public override void StateUpdate()
    {
        if (Condition()) // reached target
        {
            isActive = false;
            isInit = false;
            //Debug.Log("Reset move init");
            villager.SetTrigger("ReachedPoint");
            villager.SetBool("OnMove", false);
            villager.ResetActiveState();
            navAgent.Stop();
        }
        else
        {
            Action();
        }
    }
}