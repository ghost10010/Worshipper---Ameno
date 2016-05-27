using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    [SerializeField] private List<Button> powerButtons = new List<Button>();

    //End game
    [SerializeField] private GameObject pnlEndGame = null;
    [SerializeField] private Text txtWinnar = null;
    [SerializeField] private List<Text> playerScores = null;

    //Main menu
    [SerializeField] private GameObject pnlMenu = null;
    [SerializeField] private ButtonController btnPauze = null;
    [SerializeField] private ButtonController btnUnPauze = null;

    //building menu
    [SerializeField] private GameObject pnlBuildings = null;
    [SerializeField] private List<Button> buildingButtons = null;

    //extra buttons
    [SerializeField] private Button btnAttack = null;
    [SerializeField] private Button btnBuild = null;

    //Debug
    [SerializeField] private Text txtDebug = null;

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
    public bool isBuilding = false;

    private GameObject movableBuilding;
    private VillagerController selectedVillager;
    private Building selectedBuilding;

    //UI controllers
    [SerializeField] private BuildingInfoController buildingInfo;
    [SerializeField] private BuildingInfoController buildingBuild;
    [SerializeField] private PlayerInfoController playerInfo;
    [SerializeField] private PowerInfoController powerInfo;
    [SerializeField] private VillageInfoController villageInfo;
    [SerializeField] private VillagerInfoController villagerInfo;
    [SerializeField] private ResourcePointInfoController resourcePointInfo;

    private GameObject exitPanel;
    private bool hasExitPanel = false;

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
        GetExitPanel();

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
	}

    void Awake()
    {
        instance = this;
    }

    private void GetExitPanel()
    {
        if (!hasExitPanel)
        {
            exitPanel = GameObject.FindGameObjectWithTag("ExitPanel");
            if (exitPanel != null)
            {
                hasExitPanel = true;
                exitPanel.SetActive(false);
            }
        }
    }

    public void TogglePanelState(string panel, bool state)
    {
        switch(panel)
        {
            case "VillageInfo":
                villageInfo.SetState(state);
                break;
            case "VillagerInfo":
                villagerInfo.SetState(state);
                break;
            case "BuildingInfo":
                buildingInfo.SetState(state);
                break;
            case "BuildingBuild":
                buildingBuild.SetState(state);
                showingBuildings = state;
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
            case "ResourcePoint":
                resourcePointInfo.SetState(state);
                break;
        }
    }
    public void EndGame(int loser, int[] testscores)
    {
        TogglePanelState("EndGame", true);
        Time.timeScale = 0.0f;

        NetworkController netController = GameManger.instance.GetLocalNetworkController();
        if (netController.isServer)
        {
            exitPanel.SetActive(true);
        }

        Settings.instance.ReturnFromGame = true;
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

        for (int index = 0; index < testscores.Length; index++)
        {
            playerScores[index].text = testscores[index].ToString();
            playerScores[index].gameObject.SetActive(true);
        }
    }
    
    public void SelectBuilding(Building building)
    {
        TogglePanelState("BuildingInfo", true);

        if (selectedBuilding != null)
        {
            selectedBuilding.selectedParticle.Stop();
        }

        selectedBuilding = building;
        selectedBuilding.selectedParticle.Play();
    }
    public void UnselectBuilding()
    {
        TogglePanelState("BuildingInfo", false);

        if (selectedBuilding != null)
        {
            selectedBuilding.selectedParticle.Stop();
        }
        if (selectedVillager != null)
        {
            UnselectVillager("Click");
        }

        selectedBuilding = null;
    }

    public void SelectVillager(VillagerController villager)
    {
        if (selectedVillager != null)
        {
            selectedVillager.SetVillagerLook();
        }
        if (selectedBuilding != null)
        {
            UnselectBuilding();
        }

        villagerSelected = true;
        selectedVillager = villager;
    }
    public void UnselectVillager(string type)
    {
        if (type == "Click")
        {
            villagerSelected = false;
            TogglePanelState("VillagerInfo", false);

            if (selectedVillager != null)
            {
                selectedVillager.SetVillagerLook();
                selectedVillager = null;
            }
        }
        else
        {
            if (!villagerSelected)
            {
                TogglePanelState("VillagerInfo", false);
            }
        }
    }
    public void UpdateVillager(VillagerController villager)
    {
        villagerInfo.ShowVillagerInfo(villager);
    }

    //Controller methodes
    public void SetPlayerInfo(Player player)
    {
        playerInfo.ShowPlayerInfo(player);
    }
    public void SetVillageInfo(Village village)
    {
        villageInfo.ShowVillageInfo(village);
    }
    public void SetVillagerInfo(VillagerController villager)
    {
        TogglePanelState("VillagerInfo", true);
        villagerInfo.ShowVillagerInfo(villager);
    }
    public void SetBuildingInfo(Building building)
    {
        SelectBuilding(building);
        //TogglePanelState("BuildingInfo", true);
        buildingInfo.ShowInfo(building);
    }
    public void SetResourcePoint(ResourcePoint point)
    {
        TogglePanelState("ResourcePoint", true);
        resourcePointInfo.ShowResourcePointInformation(point);
    }

    //OnClick methodes
    public void ClickPower(int power)
    {
        if (isFirstPower)
        {
            MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.message.powersizehelp"), 5.0f, Color.green);
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
        //GameObject building = GameManger.instance.GetBuilding(buildingId);
        GameObject building = GameManger.instance.GetBuildingTrans(buildingId);
        Building buildingController = building.GetComponent<Building>();

        if (isFirstBuilding)
        {
            MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.message.buildingrotate"), 5.0f, Color.green);
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
            MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.error.noresources"), 5.0f, Color.red);
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
        //GameManger.instance.GetSelectedVillage().AttackVillage(targetVillage);
        NetworkController netController = GameManger.instance.GetLocalNetworkController();
        netController.AttackVillage(GameManger.instance.GetSelectedVillage().gameObject, targetVillage.gameObject);
        Debug.Log("AttackVillage");
    }
    public void ClickSave()
    {
        if (SaveLoad.instance != null)
        {
            Debug.Log("ClickSave");
            SaveLoad.instance.Save();
        }
        else
        {
            Debug.Log("No SaveLoad");
        }
    }
    public void ClickLoad()
    {
        Debug.Log("ClickLoad");
        Settings.instance.LoadFromSave = !Settings.instance.LoadFromSave;
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

    public bool IsMouseOverUI()
    {
        return mouseOverUI;
    }

    //Debug
    public void AddDebug(string debug)
    {
        if (txtDebug != null)
        {
            txtDebug.text += debug;
        }
    }

    public void ClickTestTrohpy(int trophyId)
    {
        GameJoltManager.instance.AddTrophy(trophyId);
    }

    //FSM
    /*public void ClickInitFSMVillage()
    {
        Village village = fsmVillage.GetComponent<Village>();

        fsmVillage.SetActive(true);
        village.InitVillage();

        GameManger.instance.AddVillage(village);
    }*/
}