using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackVillageState : VillagerState
{
    public float distance = 0.0f;

    public GameObject enemyVillageCenter;
    public Village enemyVillage;

    public List<VillagerController> enemyVillagers = new List<VillagerController>();

	void Start ()
    {
        GetComponentInParent<VillagerController>().AddState(this);
    }

    public override void InitState()
    {
        villager.minDistance = 10.0f;

        if (!isInit && !villager.GetTrigger("IsAttackingVillager"))
        {
            Village foundTarget = GetTarget();
            if (foundTarget != null)
            {
                enemyVillage = foundTarget.GetComponent<Village>();
            }

            if (enemyVillage != null)
            {
                enemyVillageCenter = enemyVillage.villageCenter;
                enemyVillagers = enemyVillage.GetVillagerControllers();

                villager.moveTarget = enemyVillageCenter.transform.position;

                if (!Condition())
                {
                    villager.SetBool("OnMove", true);
                    villager.ResetActiveState();
                }
            }

            isInit = true;
        }

        isActive = true;
    }

    public override void Action()
    {
        distance = Vector3.Distance(this.transform.position, villager.moveTarget);
    }
    public override bool Condition()
    {
        if (Mathf.RoundToInt(Vector3.Distance(this.transform.position, villager.moveTarget)) <= 11) //Enemy Village
        {
            return true;
        }

        return false;
    }

    public override void StateUpdate()
    {
        if(Condition())
        {
            //Debug.Log("AttackVillage Condition");
            if (enemyVillagers.Count > 0)
            {
                //Debug.Log("AttackVillage Enemies");
                

                if (villager.enemys.Count > 0)
                {
                    villager.enemys[0] = enemyVillagers[0].gameObject;
                }
                else
                {
                    villager.enemys.Add(enemyVillagers[0].gameObject);
                }

                //Debug.Log("Add to villager");
                enemyVillagers.RemoveAt(0);
                villager.SetBool("IsAttackingVillager", true);

                isActive = false;
                isInit = false;
            }
            else
            {
                //Debug.Log("AttackVillage No Enemies");
                villager.SetBool("IsAttackingVillage", false);
                villager.SetBool("IsFinished", true);

                isActive = false;
                isInit = false;
            }
        }
        else
        {
            Action();
        }
    }

    private Village GetTarget()
    {
        Village foundTarget = null;

        if (!IsVillageNull())
        {
            foundTarget = villager.GetVillage().target;
        }
        else
        {
            foundTarget = villager.GetNonNetworkedVillage().target;
        }

        return foundTarget;
    }
}
