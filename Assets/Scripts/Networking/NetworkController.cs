using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;

public class NetworkController : NetworkBehaviour
{
    public List<Vector3> villagePositions;
    public int spawnedVillages = 0;

    private TerrainObject terrainObject;
    private TerrainDeformer terrainDeformer;

    private bool terrainSpawned = false;

    public override void OnStartServer()
    {
        if (isLocalPlayer)
        {
            GameManger.instance.InitGame();
        }
    }

    void Start()
    {
        if (isServer)
        {
            CmdSpawnTerrain();
        }
    }

    //Spawn Terrain
    [Command]
    public void CmdSpawnTerrain()
    {
        if (GameObject.FindGameObjectWithTag("Terrain") != null)
        {
            return;
        }

        GameObject terrainPrefab = ObjectManager.instance.GetTerrainPrefab();
        
        float width = terrainPrefab.GetComponent<Terrain>().terrainData.size.x;
        float height = terrainPrefab.GetComponent<Terrain>().terrainData.size.z;

        Vector3 terrainPosition = new Vector3(-(width / 2), 0.0f, -(height / 2));

        GameObject spawnedTerrain = (GameObject)Instantiate(terrainPrefab, terrainPosition, Quaternion.identity);
        NetworkServer.Spawn(spawnedTerrain);

        RpcSpawnTerrain();
        SetTerrainData();
    }
    private void LoadTerrainObjects()
    {
        Hashtable terrainObjects = SaveLoad.instance.GetTerrainObjects();

        for (int index = 0; index < (terrainObjects.Count / 6); index++)
        {
            Debug.Log("index: " + index);
            string objectId = "Object" + index;

            int type = 0;
            int amount = 0;
            int parent = 0;
            int age = 0;
            int id = 0;
            //Debug.Log(terrainObjects[objectId + "Position"].ToString());
            Vector3 position = GetVectorFromString(terrainObjects[objectId + "Position"].ToString());
            //Vector3 position = Vector3.zero;
            int.TryParse(terrainObjects[objectId + "Type"].ToString(), out type);
            int.TryParse(terrainObjects[objectId + "Amount"].ToString(), out amount);
            int.TryParse(terrainObjects[objectId + "Parent"].ToString(), out parent);
            int.TryParse(terrainObjects[objectId + "Age"].ToString(), out age);
            int.TryParse(terrainObjects[objectId + "Id"].ToString(), out id);
            
            GameObject prefab = ObjectManager.instance.GetObjectOfType((ResourceType)type, id);
            GameObject spawnedObject = (GameObject)Instantiate(prefab, position, Quaternion.identity);

            if (parent != -1)
            {
                Village village = GameManger.instance.GetVillageWithID(parent);
                spawnedObject.transform.SetParent(village.gameObject.transform);
            }
            else
            {
                GameObject terrain = GameObject.FindGameObjectWithTag("Terrain");
                spawnedObject.transform.SetParent(terrain.transform);
            }

            Grow growwing = spawnedObject.GetComponent<Grow>();
            if (growwing != null)
            {
                Debug.Log("SetAge");
                growwing.SetAge(age);
            }
            else
            {
                Debug.Log("No Grow");
            }

            ResourcePoint resourcePoint = spawnedObject.GetComponent<ResourcePoint>();
            if (resourcePoint != null)
            {
                Debug.Log("SetAmount");
                resourcePoint.amount = amount;
            }
            else
            {
                Debug.Log("No ResourcePoint");
            }

            NetworkServer.Spawn(spawnedObject);
        }
    }
    [ClientRpc]
    public void RpcSpawnTerrain()
    {
        SetTerrainData();
    }
    private void SetTerrainData()
    {
        GameObject terrain = GameObject.FindGameObjectWithTag("Terrain");
        terrainDeformer = terrain.GetComponent<TerrainDeformer>();
        terrainObject = terrain.GetComponent<TerrainObject>();

        GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CameraController>().SetTerrainObject(terrainObject);
    }

    //Add Player
    public void AddPlayer(GameObject player)
    {
        CmdAddPlayer(player);
    }
    [Command]
    void CmdAddPlayer(GameObject player)
    {
        GameManger.instance.AddPlayer(player.GetComponent<Player>());

        int playerId = GameManger.instance.GetPlayerID(player.GetComponent<Player>());
        Player currentPlayer = GameManger.instance.GetPlayer(playerId);

        if (Settings.instance.LoadFromSave)
        {
            Hashtable playerData = SaveLoad.instance.GetPlayerData(playerId);

            if (playerData.Keys.Count > 0)
            {
                int score = 0;
                int mana = 0;

                int.TryParse(playerData["Score"].ToString(), out mana);
                int.TryParse(playerData["Mana"].ToString(), out mana);

                currentPlayer.score = score;
                currentPlayer.mana = mana;
            }
            else
            {
                Debug.Log("No PlayerData");
            }
        }

        currentPlayer.SetPlayerID(playerId);
        currentPlayer.SetColorId(playerId);
    }

    //SpawnVillage
    public void SpawnVillage(Vector3 position, GameObject player)
    {
        CmdAddVillage(position, player);
    }
    [Server]
    void CmdAddVillage(Vector3 position, GameObject player)
    {
        Debug.Log("CmdAddVillage: " + GameManger.instance.spawnedVillages);
        Player currentPlayer = player.GetComponent<Player>();
        int playerID = GameManger.instance.GetPlayerID(currentPlayer);

        GameObject spawnedObject = (GameObject)Instantiate(ObjectManager.instance.GetVillagePrefab(), villagePositions[playerID], Quaternion.identity);
        Village spawnedVillage = spawnedObject.GetComponent<Village>();
        spawnedVillage.SetOwner(currentPlayer.playerID);
        NetworkServer.SpawnWithClientAuthority(spawnedObject, player);
        GameManger.instance.AddVillage(spawnedObject.GetComponent<Village>());

        GameManger.instance.spawnedVillages++;

        if (Settings.instance.LoadFromSave)
        {
            LoadVillageStats(playerID, spawnedVillage);
            LoadVillageBuildings(playerID, spawnedVillage);

            if (GameManger.instance.spawnedVillages == 2)
            {
                Debug.Log("SpawnObjects");
                LoadTerrainObjects();
            }
        }
    }
    private void LoadVillageStats(int playerId, Village spawnedVillage)
    {
        Hashtable villageData = SaveLoad.instance.GetVillageData(playerId);

        int food = 0;
        int wood = 0;
        int iron = 0;
        int villagers = 0;
        int maxVillagers = 0;
        int soldiers = 0;
        int maxSoldiers = 0;

        int.TryParse(villageData["Food"].ToString(), out food);
        int.TryParse(villageData["Wood"].ToString(), out wood);
        int.TryParse(villageData["Iron"].ToString(), out iron);
        int.TryParse(villageData["Villagers"].ToString(), out villagers);
        int.TryParse(villageData["MaxVillagers"].ToString(), out maxVillagers);
        int.TryParse(villageData["Soldiers"].ToString(), out soldiers);
        int.TryParse(villageData["MaxSoldiers"].ToString(), out maxSoldiers);

        spawnedVillage.food = food;
        spawnedVillage.wood = wood;
        spawnedVillage.iron = iron;
        spawnedVillage.maxVillagers = maxVillagers;
        spawnedVillage.startVillagers = villagers;
        spawnedVillage.soldiers = soldiers;
        spawnedVillage.maxSoldiers = maxSoldiers;
    }
    private void LoadVillageBuildings(int playerId, Village spawnedVillage)
    {
        Hashtable buildingData = SaveLoad.instance.GetVillageBuildings(playerId);

        for (int index = 0; index < (buildingData.Keys.Count / 3); index++)
        {
            string buildingId = "Building" + index;

            int buildingType = 0;
            Vector3 buildingPosition = GetVectorFromString(buildingData[buildingId + "Position"].ToString());
            Vector3 buildingRotation = GetVectorFromString(buildingData[buildingId + "Rotation"].ToString());

            int.TryParse(buildingData[buildingId + "Type"].ToString(), out buildingType);
            Debug.Log((BuildingType)buildingType);
            Debug.Log(buildingPosition);
            Debug.Log(buildingRotation);

            CmdSpawnBuildingLoad(spawnedVillage.gameObject, (BuildingType)buildingType, buildingPosition, buildingRotation);
        }
    }
    private Vector3 GetVectorFromString(string rString)
    {
        string[] temp = rString.Substring(1, rString.Length - 2).Split(',');
        float x = float.Parse(temp[0]);
        float y = float.Parse(temp[1]);
        float z = float.Parse(temp[2]);
        Vector3 rValue = new Vector3(x, y, z);
        return rValue;
    }

    //SpawnVillager
    public void SpawnVillager(VillagerType type, GameObject village, bool spawnParticle, int playerId)
    {
        CmdSpawnVillager(type, village, playerId, spawnParticle);
    }
    [Command]
    public void CmdSpawnVillager(VillagerType type, GameObject villageObject, int playerId, bool spawnParticle)
    {
        Village villageController = villageObject.GetComponent<Village>();

        GameObject spawnPrefab = null;
        if (type == VillagerType.Soldier)
        {
            spawnPrefab = ObjectManager.instance.soldierPrefab;
        }
        else
        {
            spawnPrefab = ObjectManager.instance.villagerPrefab;
        }

        Vector3 spawnPosition = villageController.GetVillagerSpawnPosition();
        spawnPosition.y = 1.0f;
        GameObject spawnedVillager = (GameObject)Instantiate(spawnPrefab, spawnPosition, Quaternion.identity);
        spawnedVillager.transform.position = spawnPosition;
        VillagerController spawnedController = spawnedVillager.GetComponent<VillagerController>();
        spawnedController.SetVillagerType(type);
        spawnedController.SetVillagerStats();

        NetworkServer.Spawn(spawnedVillager);

        RpcSetVillagerVillage(spawnedVillager, type, villageObject);

        if (spawnParticle)
        {
            NetworkController villageNetController = GameManger.instance.GetPlayerNetworkController(playerId);
            RpcSpawnParticleOnClient("Spawn", (int)villageNetController.netId.Value, spawnPosition);
        }
    }
    [ClientRpc]
    public void RpcSetVillagerVillage(GameObject villager, VillagerType type, GameObject village)
    {
        SetVillagerInfo(villager, type, village);
    }
    private void SetVillagerInfo(GameObject villager, VillagerType type, GameObject village)
    {
        villager.transform.SetParent(village.transform);

        VillagerController spawnedController = villager.GetComponent<VillagerController>();
        spawnedController.SetVillagerType(type);
        spawnedController.SetVillage(village.GetComponent<Village>());
        spawnedController.SetVillagerLook();
    }

    //Spawn Particle on Client
    [ClientRpc]
    public void RpcSpawnParticleOnClient(string particle, int networkId, Vector3 position)
    {
        NetworkController netController = GameManger.instance.GetLocalNetworkController();

        if (netController.netId.Value == networkId)
        {
            GameObject particleObject = null;
            switch(particle)
            {
                case "Spawn":
                    particleObject = ObjectManager.instance.spawnParticle;
                    break;
                case "Wood":
                    particleObject = ObjectManager.instance.woodParticle;
                    break;
                case "Iron":
                    particleObject = ObjectManager.instance.ironParticle;
                    break;
                case "Food":
                    particleObject = ObjectManager.instance.foodParticle;
                    break;
                case "Mana":
                    particleObject = ObjectManager.instance.manaParticle;
                    break;
            }

            if (particleObject != null)
            {
                Instantiate(particleObject, position, Quaternion.identity);
            }
        }
    }

    //Move Object
    [Command]
    void CmdMoveObject(GameObject moveObject)
    {
        moveObject.GetComponent<MovableObject>().isMoving = true;
    }
    public void MoveObject(GameObject moveObject)
    {
        CmdMoveObject(moveObject);
    }

    //Change position
    [Command]
    void CmdChangeObjectPosition(GameObject moveObject, Vector3 position)
    {
        moveObject.transform.position = position;
        moveObject.GetComponent<MovableObject>().isMoving = false;
    }
    public void ChangePosition(GameObject moveObject, Vector3 position)
    {
        CmdChangeObjectPosition(moveObject, position);
    }

    //Remove object(Movable)
    [Command]
    void CmdRemoveObject(GameObject moveObject)
    {
        NetworkServer.UnSpawn(moveObject);
    }
    public void RemoveObject(GameObject moveObject)
    {
        CmdRemoveObject(moveObject);
    }

    //SpawnObject
    [Command]
    void CmdAddObject(int powerID, int objectID, Vector3 position, int playerId)
    {
        List<Village> playerVillage = GameManger.instance.GetPlayerVillages(playerId);
        Power power = GameManger.instance.GetPower(powerID);
        
        GameObject spawnedObject = (GameObject)Instantiate(power.spawnObjects[objectID], position, Quaternion.identity);
        spawnedObject.transform.SetParent(playerVillage[0].gameObject.transform);

        NetworkServer.Spawn(spawnedObject);
    }
    public void SpawnObject(List<GameObject> spawnObjects, int area, int amount, PowerType powerType, int playerId)
    {
        Player player = GameManger.instance.GetLocalPlayer();
        Vector3 position = Mouse.instance.GetMouseWorldPosition();

        for (int index = 0; index < amount; index++)
        {
            float randomX = Random.Range(position.x - area, position.x + area);
            float randomZ = Random.Range(position.z - area, position.z + area);
            float randomY = terrainObject.GetTerrainHeight(new Vector3(randomX, 40.0f, randomZ));

            Vector3 spawnPosition = new Vector3(randomX, randomY, randomZ);

            int nextSpawnObject = Random.Range(0, spawnObjects.Count);
            CmdAddObject((int)powerType, nextSpawnObject, spawnPosition, playerId);
        }
    }

    //End game
    [Command]
    public void CmdEndGame(int loser, int[] scores)
    {
        RpcEndGame(loser, scores);
    }
    [ClientRpc]
    public void RpcEndGame(int loser, int[] testscores)
    {
        GameManger.instance.gameEnded = true;
        UIManager.instance.EndGame(loser, testscores);
    }
    public void EndGame(int loser)
    {
        CmdEndGame(loser, GameManger.instance.GetPlayerScores());
    }

    //Deform terrain
    [ClientRpc]
    public void RpcDeformTerrain(Vector3 position, float size)
    {
        terrainDeformer.DestroyTerrain(position, size);
    }
    [Command]
    public void  CmdDeformTerrain(Vector3 position, float size)
    {
        RpcDeformTerrain(position, size);
    }
    public void DeformTerrain(Vector3 position, float size)
    {
        CmdDeformTerrain(position, size);
    }

    //Pauze/Unpauze
    [ClientRpc]
    public void RpcTimeScale(float scale, bool state)
    {
        Time.timeScale = scale;
        UIManager.instance.isPauzed = state;
        UIManager.instance.SetPauzeButtons();
    }
    [Command]
    public void CmdTimeScale(float scale, bool state)
    {
        RpcTimeScale(scale, state);
    }
    public void UnpauzeGame()
    {
        CmdTimeScale(1.0f, false);
    }
    public void PauzeGame()
    {
        CmdTimeScale(0.0f, true);
    }

    //Add resources
    [Command]
    void CmdAddResourcesToVillage(GameObject village, int amount, ResourceType resource, int playerId, Vector3 particlePosition)
    {
        village.GetComponent<Village>().AddResource(resource, amount);

        NetworkController villageNetController = GameManger.instance.GetPlayerNetworkController(playerId);
        RpcSpawnParticleOnClient(resource.ToString(), (int)villageNetController.netId.Value, particlePosition);
    }
    public void AddResourcesToVillage(GameObject village, int amount, ResourceType resource, int playerId, Vector3 particlePosition)
    {
        CmdAddResourcesToVillage(village, amount, resource, playerId, particlePosition);
    }

    //AttackVillage
    public void AttackVillage(GameObject source, GameObject target)
    {
        CmdAttackVillage(source, target);
    }
    [Command]
    public void CmdAttackVillage(GameObject source, GameObject target)
    {
        source.GetComponent<Village>().AttackVillage(target.GetComponent<Village>());
    }

    //AddBuildingToVillage
    public void SpawnBuilding(GameObject villageObject, BuildingType type, Vector3 position, Quaternion rotation)
    {
        CmdSpawnBuilding(villageObject, type, position, rotation);
    }
    [Command]
    public void CmdSpawnBuilding(GameObject villageObject, BuildingType type, Vector3 position, Quaternion rotation)
    {
        GameObject buildingObject = GameManger.instance.GetBuildingOfType(type);

        if (buildingObject != null)
        {
            Building placedBuilding = buildingObject.GetComponent<Building>();
            Village villageController = villageObject.GetComponent<Village>();

            placedBuilding.canMove = false;
            placedBuilding.SetVillage(villageController);

            GameObject spawnedObject = (GameObject)Instantiate(buildingObject, position, Quaternion.identity);

            spawnedObject.transform.SetParent(villageObject.transform);
            spawnedObject.transform.position = position;
            spawnedObject.transform.rotation = rotation;

            AddPointsToVillage(placedBuilding.GetPatrolPoints(), villageController);
            AddPointsToVillage(placedBuilding.GetGuardPositions(), villageController);
            AddBuildingIncrease(placedBuilding.increaseType, placedBuilding.increaseAmount, villageController);

            villageController.AddBuilding(spawnedObject);

            NetworkServer.Spawn(spawnedObject);
        }
    }
    [Command]
    public void CmdSpawnBuildingLoad(GameObject villageObject, BuildingType type, Vector3 position, Vector3 rotation)
    {
        GameObject buildingObject = GameManger.instance.GetBuildingOfType(type);

        if (buildingObject != null)
        {
            Building placedBuilding = buildingObject.GetComponent<Building>();
            Village villageController = villageObject.GetComponent<Village>();

            placedBuilding.canMove = false;
            placedBuilding.SetVillage(villageController);

            GameObject spawnedObject = (GameObject)Instantiate(buildingObject, position, Quaternion.identity);

            spawnedObject.transform.SetParent(villageObject.transform);
            spawnedObject.transform.position = position;
            spawnedObject.transform.eulerAngles = rotation;

            AddPointsToVillage(placedBuilding.GetPatrolPoints(), villageController);
            AddPointsToVillage(placedBuilding.GetGuardPositions(), villageController);
            AddBuildingIncrease(placedBuilding.increaseType, placedBuilding.increaseAmount, villageController);

            villageController.AddBuilding(spawnedObject);

            NetworkServer.Spawn(spawnedObject);
        }
    }
    private void AddBuildingIncrease(IncreaseType type, int amount, Village village)
    {
        switch(type)
        {
            case IncreaseType.Soldiers:
                village.maxSoldiers += amount;
                break;
            case IncreaseType.Storage:
                village.maxFood += amount;
                village.maxWood += amount;
                village.maxIron += amount;
                break;
            case IncreaseType.Villagers:
                village.maxVillagers += amount;
                break;
            case IncreaseType.Mana:
                village.increaseMana += amount;
                break;
        }
    }
    private void AddPointsToVillage(List<GameObject> points, Village village)
    {
        for (int index = 0; index < points.Count; index++)
        {
            if (points[index].name.Contains("Patrol"))
            {
                village.AddPatrolPoint(points[index]);
            }
            else
            {
                village.AddGuardPoint(points[index]);
            }
        }
    }

    //Score
    [Command]
    public void CmdAddScoreToPlayer(GameObject player, int score)
    {
        player.GetComponent<Player>().score += score;
    }
    public void AddScoreToPlayer(GameObject player, int score)
    {
        CmdAddScoreToPlayer(player, score);
    }
}