using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager_Old : MonoBehaviour
{
    public static UIManager_Old instance;

    [SerializeField] private Text txtMana = null;
    [SerializeField] private Text txtScore = null;

    [SerializeField] private Image imgMana = null;
    [SerializeField] private ParticleSystem manaParticles = null;

    [SerializeField] private GameObject pnlEndGame = null;
    [SerializeField] private Text txtWinnar = null;
    [SerializeField] private List<Text> playerScores = null;

    [SerializeField] private GameObject pnlMenu = null;
    [SerializeField] private ButtonController btnPauze = null;
    [SerializeField] private ButtonController btnUnPauze = null;

    [SerializeField] private GameObject pnlBuildings = null;
    [SerializeField] private GameObject pnlBuildingBuild = null;
    [SerializeField] private List<Button> buildingButtons = null;

    [SerializeField] private Button btnAttack = null;
    [SerializeField] private Button btnBuild = null;

    [SerializeField] private TerrainDeformer terrainDeformer = null;

    private AreaOfEffect areaOfEffect;

    private bool showingMenu = false;
    private bool showingBuildings = false;
    private bool mouseOverUI = false;
    private bool villagerSelected = false;
    public bool isPauzed = false;
    public bool isPauzedByPlayer = false;
    public bool isFirstBuilding = true;
    public bool isFirstPower = true;

    private GameObject movableBuilding;
    public bool isBuilding = false;

    [SerializeField] private GameObject pnlMessage = null;
    [SerializeField] private Text txtMessage = null;
    public float messageTimer = 0.0f;
    public bool showMessage = false;

    public GameObject fsmVillage;

    private string tooltipOldValue = "";
    private string tooltipCurrent = "";
    [SerializeField] private GameObject pnlPowerEffect;
    [SerializeField] private Text txtPowerName;
    [SerializeField] private Text txtPowerCost;
    [SerializeField] private Text txtPowerEffect;

    [SerializeField] private List<Button> powerButtons = new List<Button>();

    // Use this for initialization
    void Start ()
    {
        if (GameManger.instance != null)
        {
            areaOfEffect = GameManger.instance.areaOfEfffect;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isBuilding && movableBuilding != null)
            {
                Destroy(movableBuilding);
                isBuilding = false;
            }
            else if (areaOfEffect.IsActive() && !showingMenu && !showingBuildings)
            {
                areaOfEffect.StopEffect();
            }
            else if (showingBuildings)
            {
                ClickBuildings();
            }
            else
            {
                ClickMenu();
            }
        }

        if (Input.GetKey(KeyCode.Pause))
        {
            NetworkController netController = GameManger.instance.GetLocalNetworkController();
            if (isPauzed && isPauzedByPlayer)
            {
                netController.UnpauzeGame();
            }
            else
            {
                netController.PauzeGame();
            }
        }

        if (showMessage)
        {
            messageTimer -= Time.deltaTime;

            if (messageTimer <= 0.0f)
            {
                pnlMessage.SetActive(false);

                showMessage = false;
            }
        }
	}

    void Awake()
    {
        instance = this;
    }

    public void TogglePanelState(string panel, bool state)
    {
        switch(panel)
        {
            case "VillageInfo":
                //pnlVillageInfo.SetActive(state);
                break;
            case "VillagerInfo":
                //pnlVillagerInfo.SetActive(state);
                break;
            case "BuildingInfo":
                //pnlBuildingInfo.SetActive(state);
                break;
            case "BuildingBuild":
                showingBuildings = state;
                pnlBuildingBuild.SetActive(state);
                break;
            case "EndGame":
                pnlEndGame.SetActive(state);
                break;
            case "Menu":
                pnlMenu.SetActive(state);
                break;
            case "Buildings":
                pnlBuildings.SetActive(state);
                break;
        }
    }
    public void EndGame(int loser, int[] testscores)
    {
        TogglePanelState("EndGame", true);
        Time.timeScale = 0.0f;

        GameJoltManager.instance.AddScore(GameManger.instance.GetLocalPlayer().score, "");

        if (GameManger.instance.GetLocalPlayer().playerID == loser)
        {
            GameJoltManager.instance.AddData("losses", "1");
            txtWinnar.text = Language.instance.GetTextWithKey("game.end.Lost");
        }
        else
        {
            GameJoltManager.instance.AddData("wins", "1");
            GameJoltManager.instance.CheckTrophyWins();
            txtWinnar.text = Language.instance.GetTextWithKey("game.end.Won");
        }
        Debug.Log("scoreCount: " + testscores.Length);
        for (int index = 0; index < testscores.Length; index++)
        {
            Debug.Log("Score " + index + ": " + testscores[index].ToString());
            playerScores[index].text = testscores[index].ToString();
            playerScores[index].gameObject.SetActive(true);
        }
    }
    public void SetPlayerInfo(Player player)
    {
        if (tooltipCurrent != "Mana")
        {
            txtMana.text = player.mana + "/" + player.maxMana;
        }
        else
        {
            tooltipOldValue = player.mana + "/" + player.maxMana;
        }
        
        if (tooltipCurrent != "Score")
        {
            txtScore.text = player.score.ToString();
        }
        else
        {
            tooltipOldValue = player.score.ToString();
        }

        float calc1 = 1 / ((float)player.maxMana);
        float calc2 = calc1 * ((float)player.mana);

        imgMana.fillAmount = calc2;

        manaParticles.Play();
    }
    public void SetVillageInfo(Village village)
    {
        /*txtFood.text = village.food + "/" + village.maxFood;
        txtWood.text = village.wood + "/" + village.maxWood;
        txtIron.text = village.iron + "/" + village.maxIron;
        txtVillagers.text = (village.villagers + village.soldiers) + "/" + village.maxVillagers;

        SetResourceBalance(txtFoodBalance, (village.increaseFood - village.decreaseFood));
        SetResourceBalance(txtWoodBalance, (village.increaseWood - village.decreaseWood));
        SetResourceBalance(txtIronBalance, (village.increaseIron - village.decreaseIron));*/
    }
    private void SetResourceBalance(Text txt, int value)
    {
        txt.text = "";

        if (value < 0)
        {
            txt.text = "- ";
            txt.color = Color.red;
        }
        else if (value == 0)
        {
            txt.color = Color.black;
        }
        else
        {
            txt.text = "+ ";
            txt.color = Color.green;
        }

        txt.text += Mathf.Abs(value).ToString();
    }

    public void ShowBuildingInfo(string panel, Building building)
    {
        if (villagerSelected)
        {
            return;
        }

        /*if (panel == "Build")
        {
            TogglePanelState("BuildingBuild", true);
            txtBuildName.text = Language.instance.GetTextWithKey(building.buildingKey);
            txtBuildFood.text = building.upkeepFood + "/" + building.increaseFood;
            txtBuildWood.text = building.upkeepWood + "/" + building.increaseWood;
            txtBuildIron.text = building.upkeepIron + "/" + building.increaseIron;
            txtBuildDescription.text = Language.instance.GetTextWithKey(building.descriptionKey);
        }
        else if (panel == "Info")
        {
            TogglePanelState("BuildingInfo", true);
            txtInfoName.text = Language.instance.GetTextWithKey(building.buildingKey);
            txtInfoFood.text = building.upkeepFood + "/" + building.increaseFood;
            txtInfoWood.text = building.upkeepWood + "/" + building.increaseWood;
            txtInfoIron.text = building.upkeepIron + "/" + building.increaseIron;
            txtInfoDescription.text = Language.instance.GetTextWithKey(building.descriptionKey);
        }
        else
        {

        }*/
    }
    public void SelectVillager()
    {
        villagerSelected = true;
    }
    public void UnselectVillager(string type)
    {
        if (type == "Click")
        {
            villagerSelected = false;
            TogglePanelState("VillagerInfo", false);
        }
        else
        {
            if (!villagerSelected)
            {
                TogglePanelState("VillagerInfo", false);
            }
        }
    }
    public void SetVillagerInfo(VillagerController villager)
    {
        TogglePanelState("VillagerInfo", true);
        ShowVillagerInfo(villager);
    }
    public void UpdateVillager(VillagerController villager)
    {
        ShowVillagerInfo(villager);
    }
    private void ShowVillagerInfo(VillagerController villager)
    {
        /*txtVillagerType.text = villager.GetVillagerType().ToString();
        txtVillagerFood.text = villager.upkeepFood.ToString();
        txtVillagerWood.text = villager.upkeepWood.ToString();
        txtVillagerIron.text = villager.upkeepIron.ToString();
        txtVillagerArmor.text = villager.armor.ToString();
        txtVillagerSpeed.text = villager.walkSpeed.ToString();

        int villagerHealth = villager.health;

        if (villagerHealth <= 25)
        {
            txtVillagerHealth.color = Color.red;
        }
        else
        {
            txtVillagerHealth.color = Color.green;
        }

        txtVillagerHealth.text = villagerHealth.ToString();*/
    }

    public void ClickPower(int power)
    {
        if (isFirstPower)
        {
            SetMessage(Language.instance.GetTextWithKey("game.message.powersizehelp"), 5.0f, Color.green);
            isFirstPower = false;
        }

        areaOfEffect.StartEffect((PowerType)power);
    }
    public void ClickDeformTest()
    {
        terrainDeformer.DestroyTerrain(new Vector3(0, -5.0f, 0), 10.0f);
    }
    public void ClickPauze()
    {
        NetworkController netController = GameManger.instance.GetLocalNetworkController();
        netController.PauzeGame();

        //btnPauze.gameObject.SetActive(false);
        //btnUnPauze.gameObject.SetActive(true);

        isPauzedByPlayer = true;
    }
    public void ClickUppauze()
    {
        if (btnUnPauze.isEnabled)
        {
            NetworkController netController = GameManger.instance.GetLocalNetworkController();
            netController.UnpauzeGame();

            isPauzedByPlayer = false;
        }
    }
    public void ClickMenu()
    {
        showingMenu = !showingMenu;
        TogglePanelState("Menu", showingMenu);
    }
    public void ClickBuildings()
    {
        showingBuildings = !showingBuildings;
        TogglePanelState("Buildings", showingBuildings);
    }
    public void ClickBuild(int buildingId)
    {
        List<Village> villages = GameManger.instance.GetPlayerVillages(GameManger.instance.GetLocalPlayer().playerID);
        GameObject building = GameManger.instance.GetBuilding(buildingId);
        Building buildingController = building.GetComponent<Building>();

        if (isFirstBuilding)
        {
            SetMessage(Language.instance.GetTextWithKey("game.message.buildingrotate"), 5.0f, Color.green);
            isFirstBuilding = false;
        }

        //check if in village range
        if (buildingController.buildCostFood <= villages[0].food &&
            buildingController.buildCostIron <= villages[0].iron &&
            buildingController.buildCostWood <= villages[0].wood)
        {
            GameObject spawnedObject = (GameObject)Instantiate(building, Mouse.instance.GetMouseWorldPosition(), Quaternion.identity);
            Building spawnedBuilding = spawnedObject.GetComponent<Building>();
            spawnedBuilding.canMove = true;

            MovableObject spawnedController = spawnedObject.GetComponent<MovableObject>();
            spawnedController.isBuilding = true;
            spawnedController.OnMouseDown();
            spawnedController.SetNetworkController(GameManger.instance.GetLocalNetworkController());
            spawnedController.canRotate = true;

            movableBuilding = spawnedObject;
            isBuilding = true;
        }
        else
        {
            SetMessage(Language.instance.GetTextWithKey("game.error.noresources"), 5.0f, Color.red);
        }
    }
    public void ClickQuit()
    {
        foreach (var player in GuiLobbyManager.s_Singleton.lobbySlots)
        {
            if (player != null)
            {
                var playerLobby = player as PlayerLobby;
                if (playerLobby)
                {
                    playerLobby.CmdExitToLobby();
                }
            }
        }
    }
    public void ClickAttackVillage()
    {
        Village currentVillage = GameManger.instance.GetSelectedVillage();
        Village targetVillage = null;
        List<Village> villages = GameManger.instance.GetAllVillages();

        for (int index = 0; index < villages.Count; index++)
        {
            if (currentVillage.owner != villages[index].owner)
            {
                targetVillage = villages[index];
                continue;
            }
        }
        GameManger.instance.GetSelectedVillage().AttackVillage(targetVillage);
        //NetworkController netController = GameManger.instance.GetLocalNetworkController();
        //netController.AttackVillage();
        //Debug.Log("AttackVillage");
    }

    public void SetPauzeButtons()
    {
        if (isPauzed)
        {
            btnPauze.gameObject.SetActive(false);
            btnUnPauze.gameObject.SetActive(true);

            if (isPauzedByPlayer)
            {
                btnUnPauze.Enable();
            }
            else
            {
                btnUnPauze.Disable();
            }
        }
        else
        {
            btnPauze.gameObject.SetActive(true);
            btnUnPauze.gameObject.SetActive(false);
        }
    }
    public void SetBuildButtons(bool state)
    {
        btnAttack.interactable = state;
        btnBuild.interactable = state;
    }

    public void UpdatePowerButtonsState()
    {
        for (int index = 0; index < powerButtons.Count; index++)
        {
            Player player = GameManger.instance.GetLocalPlayer();
            Power power = GameManger.instance.GetPower(GetPowerIDFromName(powerButtons[index].name));

            if (player.mana > power.manaCost)
            {
                powerButtons[index].interactable = true;
            }
            else
            {
                powerButtons[index].interactable = false;
            }
        }
    }
    private int GetPowerIDFromName(string name)
    {
        string number = name.Substring(name.Length - 1);

        return int.Parse(number);
    }
    public void UpdateBuildingsButtonsState()
    {
        for (int index = 0; index < buildingButtons.Count; index++)
        {
            List<Village> villages = GameManger.instance.GetPlayerVillages(GameManger.instance.GetLocalPlayer().playerID);
            GameObject building = GameManger.instance.GetBuilding(index);
            Building buildingController = building.GetComponent<Building>();
            //check if in village range
            if (buildingController.buildCostFood <= villages[0].food &&
                buildingController.buildCostIron <= villages[0].iron &&
                buildingController.buildCostWood <= villages[0].wood)
            {
                buildingButtons[index].interactable = true;
            }
            else
            {
                buildingButtons[index].interactable = false;
            }
        }
    }

    public void PointerEnter()
    {
        mouseOverUI = true;
    }
    public void PointerExit()
    {
        mouseOverUI = false;
    }
    public void EnterBuildingButton(int buildingId)
    {
        ShowBuildingInfo("Build", GameManger.instance.GetBuilding(buildingId).GetComponent<Building>());
    }
    public void ExitBuildingButton()
    {
        TogglePanelState("BuildingBuild", false);
    }
    public void EnterToolTip(string key)
    {
        PointerEnter();

        tooltipCurrent = key;

        if (key == "Mana")
        {
            tooltipOldValue = txtMana.text;
            txtMana.text = Language.instance.GetTextWithKey("resource.name.Mana");
        }
        else if (key == "Score")
        {
            tooltipOldValue = txtScore.text;
            txtScore.text = Language.instance.GetTextWithKey("resource.name.Score"); ;
        }
        else  if (key.Contains("Power"))
        {
            int powerId = -1;
            if (int.TryParse(key.Substring(key.Length - 1), out powerId))
            {
                ShowPowerInfo(powerId);
            }
        }
    }
    public void ExitToolTip(string key)
    {
        PointerExit();

        tooltipCurrent = "";

        if (key == "Mana")
        {
            txtMana.text = tooltipOldValue;
        }
        else if (key == "Score")
        {
            txtScore.text = tooltipOldValue;
        }
        else if (key.Contains("Power"))
        {
            HidePowerInfo();
        }

        tooltipOldValue = "";
    }

    public void ShowPowerInfo(int powerId)
    {
        Power currentPower = GameManger.instance.GetPower(powerId);
        pnlPowerEffect.SetActive(true);

        txtPowerName.text = Language.instance.GetTextWithKey(currentPower.nameKey);
        txtPowerCost.text = currentPower.manaCost.ToString();
        txtPowerEffect.text = Language.instance.GetTextWithKey(currentPower.effectKey);
    }
    public void HidePowerInfo()
    {
        pnlPowerEffect.SetActive(false);
    }

    public bool IsMouseOverUI()
    {
        return mouseOverUI;
    }

    public void SetMessage(string text, float time, Color textColor)
    {
        pnlMessage.SetActive(true);

        txtMessage.color = textColor;
        txtMessage.text = text;

        showMessage = true;
        messageTimer = time;
    }

    public void ClickInitFSMVillage()
    {
        Village village = fsmVillage.GetComponent<Village>();

        fsmVillage.SetActive(true);
        village.InitVillage();

        GameManger.instance.AddVillage(village);
    }
}