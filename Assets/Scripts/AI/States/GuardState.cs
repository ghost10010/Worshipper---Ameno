using UnityEngine;
using System.Collections;

public class GuardState : VillagerState
{
    public Vector3 guardPoint = Vector3.zero;
    public GameObject guardObject;

	void Start ()
    {
        GetComponentInParent<VillagerController>().AddState(this);
    }

    public override void InitState()
    {
        if (!isInit && !villager.GetTrigger("IsFinished"))
        {
            if (!villager.isGuarding)
            {
                guardObject = GetGuardPoint();
                
                if (guardObject != null)
                {
                    guardPoint = guardObject.transform.position;
                    villager.isGuarding = true;
                }
            }
        }

        if (guardObject == null)
        {
            villager.SetBool("IsGuarding", false);
            villager.ResetActiveState();
        }
        else
        {
            villager.moveTarget = guardPoint;
            villager.minDistance = 2;

            isActive = true;
            isInit = true;
        }
    }
    public override void Action()
    {
        if (guardObject != null)
        {
            if (Mathf.RoundToInt(Vector3.Distance(this.transform.position, villager.moveTarget)) > villager.minDistance)
            {
                isActive = false;

                villager.SetBool("OnMove", true);
                villager.ResetActiveState();
            }
            else
            {
                Debug.Log(transform.position + (guardObject.transform.forward * 10));
                transform.LookAt(transform.position + (guardObject.transform.forward * 10));
            }
        }
    }
    public override bool Condition()
    {
        if (Mathf.RoundToInt(Vector3.Distance(this.transform.position, villager.moveTarget)) <= villager.minDistance)
        {
            return true;
        }
        return false;
    }
    public override void StateUpdate()
    {
        if (Condition())
        {
            Debug.Log("ConditionReached");

            //Debug.Log("GuardPosition: " + );
            //villager.isGuarding = true;
        }
        else
        {
            Action();
        }
    }

    private GameObject GetGuardPoint()
    {
        if (!IsVillageNull())
        {
            return villager.GetVillage().GetGuardPoint();
        }
        else
        {
            return villager.GetNonNetworkedVillage().GetGuardPoint();
        }
    }
}
