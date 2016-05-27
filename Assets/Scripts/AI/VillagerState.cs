using UnityEngine;
using System.Collections;

public class VillagerState : MonoBehaviour
{
    public VillagerStateID stateID;

    public VillagerController villager;

    public bool isActive = false;
    public bool isInit = false;

    void Start()
    {
        Debug.Log("VillageState Start");
        villager = GetComponentInParent<VillagerController>();
    }

    public VillagerStateID GetStateID()
    {
        return stateID;
    }

    public virtual void StateUpdate() { }

    public virtual void InitState() { }
    public virtual void Action() { }
    public virtual bool Condition() { return false; }

    public bool IsVillageNull()
    {
        if (villager.GetVillage() == null)
        {
            return true;
        }
        return false;
    }
}