using UnityEngine;
using System.Collections;

public class AttackState : VillagerState
{
    public GameObject enemyTarget;
    public VillagerController enemy;

    public NavMeshAgent navAgent;

    public float attackTimer = 1.0f;
    public float attackTime = 1.0f;

    void Start()
    {
        GetComponentInParent<VillagerController>().AddState(this);
    }

    public override void InitState()
    {
        if (!isInit && !villager.GetTrigger("IsFinished"))
        {
            if (villager.enemys.Count > 0)
            {
                navAgent.Resume();
                //Debug.Log("Has Enemys ");
                enemyTarget = villager.enemys[0];

                if (enemyTarget != null)
                {
                    enemy = enemyTarget.GetComponent<VillagerController>();
                }

                villager.minDistance = 10.0f;

                isActive = true;
                isInit = true;
            }
            else
            {

            }
        }
    }

    public override void Action()
    {
        if (enemyTarget != null)
        {
            float distance = Vector3.Distance(this.transform.position, enemyTarget.transform.position);

            attackTimer -= Time.deltaTime;

            if (distance <= villager.minDistance)
            {
                if (attackTimer <= 0.0f)
                {
                    attackTimer = attackTime;

                    enemy.DoDamage(villager.damage);
                }
            }
            else
            {
                navAgent.SetDestination(enemyTarget.transform.position);
            }
        }
    }
    public override bool Condition()
    {
        if (enemy != null)
        {
            if (enemy.health <= 0)
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
            navAgent.Stop();

            villager.RemoveEnemy(enemyTarget);

            //enemy.Kill();
            enemyTarget = null;
            enemy = null;

            NetworkController netController = GameManger.instance.GetLocalNetworkController();
            netController.AddScoreToPlayer(GameManger.instance.GetLocalPlayer().gameObject, 15);

            if (villager.GetTrigger("IsAttackingVillage"))
            {
                villager.SetBool("IsAttackingVillager", false);
            }
            else
            {
                villager.SetBool("UnderAttack", false);
            }
            
            villager.SetBool("IsFinished", true);

            isActive = false;
            isInit = false;
        }
        else
        {
            Action();
        }
    }
}
