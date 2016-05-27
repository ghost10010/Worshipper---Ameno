using UnityEngine;
using System.Collections;

public class IdleState : VillagerState
{
    public float distance = 0.0f;

    // Use this for initialization
    void Start()
    {
        GetComponentInParent<VillagerController>().AddState(this);
    }

    public override void InitState()
    {
        isActive = true;
        isInit = true;
        /*if (!villager.hasActivity)
        {
            villager.moveTarget = villager.GetVillage().villageCenter.transform.position;
            villager.minDistance = 10.0f;
        }

        isActive = true;*/
        //base.InitState();
    }

    public override void Action()
    {

        /*distance = Vector3.Distance(this.transform.position, villager.moveTarget);

        if (Mathf.RoundToInt(Vector3.Distance(this.transform.position, villager.moveTarget)) > villager.minDistance)
        {
            isActive = false;

            villager.SetBool("OnMove", true);
            villager.ResetActiveState();
        }*/
        //base.Action();
    }
    public override bool Condition()
    {
        /*if (Vector3.Distance(this.transform.position, villager.moveTarget) <= villager.minDistance || villager.hasActivity == true)
        {
            return true;
        }*/

        return villager.canSetNextActivity;
    }

    public override void StateUpdate()
    {
        if (Condition())
        {
            villager.SetBool("IsFinished", false);

            if (villager.health > 0)
            {
                SetActivity();
            }
        }
        else
        {
            Action();
        }
    }

    private void SetActivity()
    {
        if (villager.GetVillagerType() == VillagerType.Villager)
        {
            if (HasResourceCrate() || villager.isStoring)
            {
                villager.SetBool("Store", true);
            }
            else
            {
                villager.SetBool("Gather", true);
            }
        }
        else
        {
            if (IsAttackingVillage())
            {
                villager.SetAttackVillage();
            }
            else if (HasGuardPoint() || villager.isGuarding)
            {
                villager.SetBool("IsGuarding", true);
            }
            else
            {
                villager.SetBool("IsPatroling", true);
            }
        }

        isActive = false;
        isInit = false;
    }

    private bool HasResourceCrate()
    {
        if (!IsVillageNull())
        {
            return villager.GetVillage().HasResourceCrate();
        }
        else
        {
            return villager.GetNonNetworkedVillage().HasResourceCrate();
        }
    }
    private bool HasGuardPoint()
    {
        if (!IsVillageNull())
        {
            return villager.GetVillage().HasGuardPoint();
        }
        else
        {
            return villager.GetNonNetworkedVillage().HasGuardPoint();
        }
    }
    private bool IsAttackingVillage()
    {
        if (!IsVillageNull())
        {
            return villager.GetVillage().isAttackingVillage();
        }
        else
        {
            return villager.GetNonNetworkedVillage().isAttackingVillage();
        }
    }
}