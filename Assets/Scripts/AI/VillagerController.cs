using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class VillagerController : FSM
{
    public List<VillagerState> states;

    public int health = 100;
    public int armor = 0;
    public int damage = 5;

    public int upkeepFood = 1;
    public int upkeepWood = 0;
    public int upkeepIron = 0;

    public bool isGuarding = false;
    public bool isStoring = false;
    public bool hasActivity = false;
    private bool hasSetParticleColor = false;

    public float walkSpeed = 3.5f;

    public VillagerState activeState;

    [SerializeField] private VillagerType villagerType;
    [SerializeField] private Village village;
    [SerializeField] private NonNetworkedVillage noNetworkVillage;

    [SerializeField] private ParticleSystem particleSystem;

    public Vector3 moveTarget;
    public float minDistance = 10.0f;
    public List<GameObject> enemys;

    // Use this for initialization
    void Start ()
    {
        base.Initialize();
	}

    public void ServerUpdate()
    {
        base.FSMUpdate();

        if (!hasSetParticleColor)
        {
            if (!GameManger.instance.isMenu)
            {
                //SetVillagerLook();
            }
        }

        SetActiveState();
    }

    //FSM
    public void SetInt(string name, int value)
    {
        stateMachineAnimator.SetInteger(name, value);
    }
    public void SetBool(string name, bool state)
    {
        stateMachineAnimator.SetBool(name, state);
    }
    public void SetTrigger(string name)
    {
        stateMachineAnimator.SetTrigger(name);
    }
    public bool GetTrigger(string name)
    {
        return stateMachineAnimator.GetBool(name);
    }
    public void SetIdle()
    {
        SetBool("OnMove", false);
        SetBool("Gather", false);
        SetBool("Store", false);
        SetBool("IsGuarding", false);
        SetBool("IsPatroling", false);
        SetBool("IsAttackingVillage", false);

        foreach (VillagerState state in states)
        {
            state.isInit = false;
        }
    }
    public void SetAttackVillage()
    {
        SetIdle();
        SetBool("IsAttackingVillage", true);
    }

    public void AddState(VillagerState villagerState)
    {
        states.Add(villagerState);
    }

    public VillagerState GetActiveState()
    {
        base.GetCurrentState();

        foreach (VillagerState state in states)
        {
            if (state.GetStateID() == base.currentState)
            {
                return state;
            }
        }

        return null;
    }
    public void ResetActiveState()
    {
        activeState = null;
    }
    public void SetActiveState()
    {
        activeState = GetActiveState();

        if (activeState != null)
        {
            if (!activeState.isInit)
            {
                activeState.InitState();
            }
            else
            {
                if (activeState != null)
                {
                    if (activeState.isActive)
                    {
                        activeState.StateUpdate();
                    }
                }
            }
        }
    }

    //Generic Methodes
    private void SetParticleColor(Color color)
    {
        Debug.Log("SetParticleColor: " + color);
        particleSystem.startColor = color;
    }
    public void SetVillagerLook()
    {
        Player localPlayer = GameManger.instance.GetPlayerWithId(village.GetOwner());

        if (localPlayer != null)
        {
            if (particleSystem != null)
            {
                SetParticleColor(localPlayer.GetPlayerColor());
            }
        }
    }
    public void SetVillagerStats()
    {
        switch (villagerType)
        {
            case VillagerType.Soldier:
                SetBool("IsSoldier", true);
                walkSpeed = 3.5f;
                damage = 15;
                armor = 25;
                upkeepIron++;
                break;
            case VillagerType.Villager:
                SetBool("IsSoldier", false);
                walkSpeed = 2.5f;
                damage = 5;
                armor = 10;
                break;
        }
    }

    public void DoDamage(int damage)
    {
        health -= damage;
        SetInt("Health", health);
        UpdateUI();
    }

    public int GetOwner()
    {
        if (village == null)
        {
            return -1;
        }
        return village.owner;
    }

    public void SetVillagerType(VillagerType type)
    {
        villagerType = type;
    }
    public VillagerType GetVillagerType()
    {
        return villagerType;
    }

    public void SetVillage(Village village)
    {
        this.village = village;
        this.village.AddVillagerController(this);
    }
    public Village GetVillage()
    {
        return village;
    }
    public NonNetworkedVillage GetNonNetworkedVillage()
    {
        return noNetworkVillage;
    }

    void OnMouseOver()
    {
        if (!GameManger.instance.isMenu)
        {
            UIManager.instance.SetVillagerInfo(this);
        }
    }
    void OnMouseDown()
    {
        if (!GameManger.instance.isMenu)
        {
            UIManager.instance.SelectVillager(this);
            UIManager.instance.SetVillagerInfo(this);
        }
        Debug.Log("VillagerOnMouseDown");
        NetworkController netController = GameManger.instance.GetLocalNetworkController();

        //if (GetOwner() == (int)netController.netId.Value)
        //{
            SetParticleColor(ObjectManager.instance.selectedColor);
        //}
    }
    void OnMouseExit()
    {
        if (!GameManger.instance.isMenu)
        {
            UIManager.instance.UnselectVillager("Exit");
        }
    }
    private void UpdateUI()
    {
        UIManager.instance.UpdateVillager(this);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!GetTrigger("UnderAttack") && !GetTrigger("IsAttackingVillage"))
        {
            if (other.name != "Terrain")
            {
                VillagerController villagerController = other.GetComponentInParent<VillagerController>();

                if (villagerController != null)
                {
                    if (villagerController != this)
                    {
                        int villageId = GetOwner();
                        int otherVillageId = villagerController.GetOwner();

                        if (villageId != -1 && otherVillageId != -1)
                        {
                            if (villagerController.GetOwner() != GetOwner())
                            {
                                if (!VillagerAlreadyTarget(villagerController))
                                {
                                    enemys.Add(villagerController.gameObject);
                                    SetIdle();
                                    SetBool("UnderAttack", true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
    void OnTriggerExit(Collider other)
    {

    }
    private bool VillagerAlreadyTarget(VillagerController villager)
    {
        for (int index = 0; index < enemys.Count; index++)
        {
            if (enemys[index] == villager)
            {
                return true;
            }
        }

        return false;
    }
    public void RemoveEnemy(GameObject enemy)
    {
        int enemyIndex = enemys.IndexOf(enemy.gameObject);
        enemys.RemoveAt(enemyIndex);
    }

    public void Kill()
    {
        if (villagerType == VillagerType.Soldier)
        {
            village.soldiers--;
        }
        else
        {
            village.villagers--;
        }

        SetIdle();
    }
}