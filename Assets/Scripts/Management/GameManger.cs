using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManger : MonoBehaviour
{
    public static GameManger instance;

    [SerializeField] private List<Power> powers = new List<Power>();
    [SerializeField] private List<Village> villages = new List<Village>();
    [SerializeField] private List<Player> players = new List<Player>();

    [SerializeField] private TerrainDeformer terrainDeformer;
    [SerializeField] private CameraController cameraController;
    public Village selectedVillage;

    public AreaOfEffect areaOfEfffect;

    public bool allowMultiplePickups = false;
    public bool movingObjects = false;
    public bool gameEnded = false;
    private bool isInitilized = false;
    public bool isBuilding = false;
    public bool isLevelEditor = false;
    public bool isMenu = false;

    private float resourceTimer = 0.0f;
    private float resourceTime = 5.0f;

    public Player player;
    public int playerCount = 2;
    public bool testScene = false;

    public int spawnedVillages = 0;

    void Start ()
    {
        if (!isLevelEditor && !isMenu)
        {
            UIManager.instance.TogglePanelState("VillageInfo", false);
            UIManager.instance.SetBuildButtons(false);

            MusicScript.instance.PlayRandomMusic();
        }
    }
	void Update ()
    {
        Init();

        if (IsServer() && !isLevelEditor && !isMenu)
        {
            resourceTimer -= Time.deltaTime;

            if (resourceTimer < 0)
            {
                resourceTimer = resourceTime;

                for (int index = 0; index < villages.Count; index++)
                {
                    if (CheckIfVillagersAlive(villages[index].GetVillagerControllers()))
                    {
                        villages[index].UpdateVillage();
                    }
                    else
                    {
                        if (villages[index].isInitilized)
                        {
                            Debug.Log("Endgame");
                            NetworkController netController = GetLocalNetworkController();
                            netController.EndGame(villages[index].GetOwner());
                        }
                    }
                }
            }
        }

        if (areaOfEfffect != null)
        {
            if (areaOfEfffect.IsActive())
            {
                cameraController.blockZoom = true;
                if (Input.GetAxis("Mouse ScrollWheel") > 0f) // forward
                {
                    areaOfEfffect.IncreaseSize();
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0f) // backwards
                {
                    areaOfEfffect.DecreaseSize();
                }
            }
        }
    }
    void Init()
    {
        if (!isInitilized)
        {
            if (GetLocalPlayer() != null)
            {
                UIManager.instance.SetPlayerInfo(GetLocalPlayer());
                isInitilized = false;
            }
        }
    }
    void Awake()
    {
        instance = this;
    }

    private bool CheckIfVillagersAlive(List<VillagerController> villagers)
    {
        for (int index = 0; index < villagers.Count; index++)
        {
            if (villagers[index].health > 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsServer()
    {
        NetworkController netController = GetLocalNetworkController();

        if (netController != null)
        {
            if (netController.isServer)
            {
                return true;
            }
        }
        return false;
    }

    public void InitGame()
    {
        cameraController.SetLevelWidth(2048);
    }

    public Village GetSelectedVillage()
    {
        return selectedVillage;
    }
    public void SelectVillage(Village village)
    {
        if (selectedVillage != null)
        {
            selectedVillage.isSelected = false;
        }

        selectedVillage = village;
        selectedVillage.isSelected = true;

        UIManager.instance.TogglePanelState("VillageInfo", true);
        UIManager.instance.SetVillageInfo(selectedVillage);
        UIManager.instance.SetBuildButtons(true);
    }
    public void DeselectVillage()
    {
        if (selectedVillage != null)
        {
            selectedVillage.isSelected = false;
            selectedVillage = null;
        }

        UIManager.instance.TogglePanelState("BuildingBuild", false);
        UIManager.instance.TogglePanelState("Buildings", false);
        UIManager.instance.TogglePanelState("VillageInfo", false);
        UIManager.instance.SetBuildButtons(false);
    }

    public Power GetPower(int id)
    {
        return powers[id];
    }
    public GameObject GetBuilding(int id)
    {
        if (ObjectManager.instance.buildingPrefabs.Count >= id)
        {
            return ObjectManager.instance.buildingPrefabs[id];
        }

        return null;
    }
    public GameObject GetBuildingTrans(int id)
    {
        if (ObjectManager.instance.buildingTransPrefab.Count >= id)
        {
            return ObjectManager.instance.buildingTransPrefab[id];
        }

        return null;
    }
    public GameObject GetBuildingOfType(BuildingType type)
    {
        foreach (GameObject building in ObjectManager.instance.buildingPrefabs)
        {
            if (building.GetComponent<Building>().buildingType == type)
            {
                return building;
            }
        }
        return null;
    }

    public void AddVillage(Village village)
    {
        villages.Add(village);
    }
    public List<Village> GetAllVillages()
    {
        return villages;
    }
    public Village GetVillageWithID(int villageID)
    {
        return villages[villageID];
    }

    public Player GetPlayer(int playerID)
    {
        return players[playerID];
    }
    public void AddPlayer(Player player)
    {
        players.Add(player);
    }

    public int GetPlayerID(Player player)
    {
        for (int index = 0; index < players.Count; index++)
        {
            if (players[index] == player)
            {
                return index;
            }
        }

        return -1;
    }

    public Player GetPlayerWithId(int Id)
    {
        GameObject[] controllers = GameObject.FindGameObjectsWithTag("Player");

        for (int index = 0; index < controllers.Length; index++)
        {
            if (controllers[index].GetComponent<Player>().playerID == Id)
            {
                return controllers[index].GetComponent<Player>();
            }
        }

        return null;
    }
    public Player GetLocalPlayer()
    {
        GameObject[] controllers = GameObject.FindGameObjectsWithTag("Player");

        for (int index = 0; index < controllers.Length; index++)
        {
            if (controllers[index].GetComponent<NetworkController>().isLocalPlayer)
            {
                return controllers[index].GetComponent<Player>();
            }
        }

        return null;
    }
    public Village GetLocalVillage(int playerId)
    {
        GameObject[] allVillages = GameObject.FindGameObjectsWithTag("Village");

        for (int index = 0; index < allVillages.Length; index++)
        {
            if (allVillages[index].GetComponent<Village>().GetOwner() == playerId)
            {
                return allVillages[index].GetComponent<Village>();
            }
        }

        return null;
    }
    public List<Village> GetPlayerVillages(int playerID)
    {
        List<Village> playerVillages = new List<Village>();

        GameObject[] allVillages = GameObject.FindGameObjectsWithTag("Village");

        for (int index = 0; index < allVillages.Length; index++)
        {
            if (allVillages[index].GetComponent<Village>().GetOwner() == playerID)
            {
                playerVillages.Add(allVillages[index].GetComponent<Village>());
            }
        }

        return playerVillages;
    }
    public NetworkController GetLocalNetworkController()
    {
        GameObject[] controllers = GameObject.FindGameObjectsWithTag("Player");

        for (int index = 0; index < controllers.Length; index++)
        {
            if (controllers[index].GetComponent<NetworkController>().isLocalPlayer)
            {
                return controllers[index].GetComponent<NetworkController>();
            }
        }

        return null;
    }
    public NetworkController GetPlayerNetworkController(int playerId)
    {
        GameObject[] controllers = GameObject.FindGameObjectsWithTag("Player");

        for (int index = 0; index < controllers.Length; index++)
        {
            if (controllers[index].GetComponent<Player>().playerID == playerId)
            {
                return controllers[index].GetComponent<NetworkController>();
            }
        }

        return null;
    }

    public int[] GetPlayerScores()
    {
        int[] playerScores = new int[playerCount];
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int index = 0; index < players.Length; index++)
        {
            Player scorePlayer = players[index].GetComponent<Player>();
            playerScores[scorePlayer.playerID] = scorePlayer.score;
        }

        return playerScores;
    }

    public TerrainDeformer GetTerrainDeformer()
    {
        return terrainDeformer;
    }
}