using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FSM : MonoBehaviour
{
    public Animator stateMachineAnimator;

    private Dictionary<int, VillagerStateID> villageStateHash = new Dictionary<int, VillagerStateID>();
    public VillagerStateID currentState;

    public bool canSetNextActivity = false;

    private float timer = 1.0f;
    private float time = 1.0f;

    void Awake()
    {
        //Get the Animator state machine, error if none found on this GO
        stateMachineAnimator = (Animator)GetComponent(typeof(Animator));
        if (!stateMachineAnimator || !stateMachineAnimator.runtimeAnimatorController)
        {
            Debug.LogError("State Machine Missing or not configured)");
        }
    }

    protected virtual void Initialize()
    {
        foreach (VillagerStateID state in (VillagerStateID[])System.Enum.GetValues(typeof(VillagerStateID)))
        {
            villageStateHash.Add(Animator.StringToHash("Base Layer." + state.ToString()), state);
        }
    }
    protected virtual void FSMUpdate()
    {
        GetCurrentState();
        
        if (!canSetNextActivity)
        {
            timer -= Time.deltaTime;

            if (timer < 0.0f)
            {
                canSetNextActivity = true;
            }
        }
    }

    public void SetActivityTimer()
    {
        canSetNextActivity = false;

        timer = time;
    }

    public void GetCurrentState()
    {
        if (villageStateHash.ContainsKey(stateMachineAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash))
        {
            currentState = villageStateHash[stateMachineAnimator.GetCurrentAnimatorStateInfo(0).fullPathHash];
        }
    }
}
