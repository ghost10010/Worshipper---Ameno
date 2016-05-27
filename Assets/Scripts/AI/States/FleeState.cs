using UnityEngine;
using System.Collections;

public class FleeState : VillagerState
{
    public float distance;
    public NavMeshAgent navAgent;

    public GameObject enemy;

    void Start()
    {
        GetComponentInParent<VillagerController>().AddState(this);
    }

    public override void InitState()
    {
        //Debug.Log("InitFlee");
        villager.minDistance = 30.0f;
        
        if (!isInit && !villager.GetTrigger("IsFinished"))
        {
            if (villager.enemys.Count > 0)
            {
                navAgent.Resume();
                //Debug.Log("HasEnemy");
                enemy = villager.enemys[0];
                //Debug.Log("InitState: true");
                isActive = true;
                isInit = true;
            }
            else if (enemy == null)
            {
                //Debug.Log("NoEnemy");
                villager.SetBool("IsFinished", true);
                villager.SetBool("UnderAttack", false);
                //Debug.Log("InitState: false");
                isActive = false;
                isInit = false;
            }
        }
    }

    public override void Action()
    {
        //Debug.Log("ActionFlee");
        if (enemy != null)
        {
            distance = Vector3.Distance(this.transform.position, enemy.transform.position);

            Vector3 target = (this.transform.position - enemy.transform.position).normalized;
            //Debug.Log("TargetVector: " + target);
            navAgent.destination = this.transform.position + target * villager.minDistance;
        }
    }
    public override bool Condition()
    {
        //Debug.Log("ConditionFlee");
        if (enemy != null)
        {
            if (Vector3.Distance(this.transform.position, enemy.transform.position) >= villager.minDistance)
            {
                return true;
            }
        }

        return false;
    }

    public override void StateUpdate()
    {
        if (Condition())
        {
            navAgent.speed = villager.walkSpeed;
            navAgent.Stop();

            villager.RemoveEnemy(enemy);

            enemy = null;

            villager.SetBool("IsFinished", true);
            villager.SetBool("UnderAttack", false);

            isActive = false;
            isInit = false;
            //Debug.Log("NoMoreFlee");
        }
        else
        {
            Action();
        }
    }
}
