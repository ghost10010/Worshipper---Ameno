using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class Player : NetworkBehaviour
{
    [SyncVar] public int playerID = -1;

    public int mana = 100;
    
    public int maxMana = 1000;

    private float updateTimer = 5.0f;
    private float updateTime = 5.0f;

    private bool hasControl = false;

    [SyncVar] public int colorId = -1;
    [SyncVar] public int score = 0;

    [SerializeField] private List<Color> playerColors = new List<Color>();

    public override void OnStartServer()
    {
        NetworkController netController = GetComponent<NetworkController>();

        if (localPlayerAuthority)
        {
            Vector3 position = new Vector3(0.0f, 0.0f, 0.0f);
            netController.AddPlayer(this.gameObject);
            netController.SpawnVillage(position, this.gameObject);
        }
    }

    void Update()
    {
        if (localPlayerAuthority)
        {
            updateTimer -= Time.deltaTime;

            if (updateTimer < 0)
            {
                updateTimer = updateTime;

                List<Village> villages = GameManger.instance.GetPlayerVillages(playerID);

                int totalManaIncrease = 0;

                for (int index = 0; index < villages.Count; index++)
                {
                    totalManaIncrease += villages[index].increaseMana;
                }

                mana += totalManaIncrease;
                UIManager.instance.SetPlayerInfo(this);
                UIManager.instance.UpdatePowerButtonsState();
                limitMana();
            }
        }
    }

    public void ChangeScore(int amount)
    {
        score += amount;
    }

    public void SetPlayerID(int id)
    {
        playerID = id;
    }
    public void SetColorId(int id)
    {
        colorId = id;
    }

    public void limitMana()
    {
        if (mana > maxMana)
        {
            mana = maxMana;
        }
    }

    public Color GetPlayerColor()
    {
        if (colorId == -1 || colorId > playerColors.Count)
        {
            return Color.grey;
        }

        return playerColors[colorId];
    }
}