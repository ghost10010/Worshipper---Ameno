using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class MovableObject : NetworkBehaviour
{
    [SyncVar] public bool isMoving = false;

    public bool isDragging = false;
    public bool overBuilding = false;
    public bool overVillage = false;
    public bool overWarehouse = false;
    public bool isBuilding = false;
    public bool canRotate = false;

    private float rotateSpeed = 50.0f;
    private int resourceScore = 10;
    private int objectRange = 50;

    private Vector3 originalPosition;

    public ObjectType objectType;
    public ResourceType resourceType;

    private GameObject objectParent;

    private Building building;
    private TerrainObject terrainObject;
    private NetworkController netController;

    private NetworkTransform netTransform;

    [SerializeField] private GameObject particlePrefab;

    void Start()
    {
        objectParent = this.gameObject.transform.GetChild(0).gameObject;

        netController = GameManger.instance.GetLocalNetworkController();
        netTransform = GetComponent<NetworkTransform>();

        terrainObject = GameObject.FindGameObjectWithTag("Terrain").GetComponent<TerrainObject>();
    }

    void Update()
    {
        netTransform.enabled = !isMoving;

        if (isBuilding)
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnMouseUp();
                isBuilding = false;
            }

            if (Input.GetMouseButton(1) && canRotate)
            {
                transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
            }
        }

        if (isMoving && !isDragging)
        {
            objectParent.SetActive(false);
        }
        if (!isMoving && !isDragging)
        {
            objectParent.SetActive(true);
        }
        if (isDragging)
        {
            Vector3 dragPosition = Mouse.instance.GetMouseWorldPosition();

            if (dragPosition != Vector3.zero)
            {
                float terrainHeight = terrainObject.GetTerrainHeight(new Vector3(dragPosition.x, 40.0f, dragPosition.z));
                dragPosition.y = terrainHeight + 5.0f;
                this.gameObject.transform.position = dragPosition;
            }
        }
    }

    public void OnMouseDown()
    {
        if (!CanMoveObject() || (!CanPickup() && objectType != ObjectType.Building))
        {
            return;
        }

        if (netController == null)
        {
            netController = GameManger.instance.GetLocalNetworkController();
        }

        if (!isBuilding)
        {
            GameJoltManager.instance.AddData("pickupMovable", "1");
            GameJoltManager.instance.CheckTrophyPickups();
            netController.MoveObject(this.gameObject);

            if (objectType == ObjectType.Tree)
            {
                GetComponent<Grow>().PickupObject();
            }
        }
        else
        {
            GameManger.instance.isBuilding = true;
        }

        UIManager.instance.UnselectVillager("Click");

        originalPosition = this.gameObject.transform.position;
        isDragging = true;
        GameManger.instance.movingObjects = true;
    }
    public void OnMouseOver()
    {
        if (GameManger.instance.allowMultiplePickups && GameManger.instance.movingObjects)
        {
            isDragging = true;
        }
    }
    public void OnMouseUp()
    {
        if (!CanMoveObject() || (!CanPickup() && objectType != ObjectType.Building))
        {
            return;
        }

        if (objectType == ObjectType.Tree)
        {
            GetComponent<Grow>().DropObject();
        }

        isDragging = false;
        GameManger.instance.movingObjects = false;

        if (isBuilding)
        {
            if (!overBuilding)
            {
                AddBuildingToVillage();
            }
            else
            {
                MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.error.overbuilding"), 5.0f, Color.red);
                Destroy(this.gameObject);
            }

            GameManger.instance.isBuilding = false;
        }
        else if (building != null) //over building
        {
            if (overBuilding && (building.buildingType == BuildingType.Warehouse || building.buildingType == BuildingType.Church))
            {
                AddResourcesToVillage();
            }
            else
            {
                SetPosition(originalPosition);
            }
        }
        else //Not over building
        {
            if (building != null)
            {
                SetPosition(originalPosition);
            }
            else
            {
                Village currentVillage = GameManger.instance.GetLocalVillage(GameManger.instance.GetLocalPlayer().playerID);
                if (currentVillage != null)
                {
                    if (currentVillage.GetComponent<Village>().IsInRange(Mouse.instance.GetMouseWorldPosition(), objectRange)) //if in villager range
                    {
                        this.transform.SetParent(currentVillage.resourceParent.transform);
                        currentVillage.AddObject(GetComponent<ResourcePoint>());
                        Debug.Log("isInPlayerVillageRange");
                    }
                }

                SetPosition(Mouse.instance.GetMouseWorldPosition());
            }
        }
    }

    private void AddBuildingToVillage()
    {
        //Check if can build
        //spawn through network

        Vector3 buildingPosition = this.transform.position;
        buildingPosition.y = terrainObject.GetTerrainHeight(buildingPosition);
        Quaternion buildingRotation = this.transform.rotation;

        if (!overBuilding)
        {
            Debug.Log("overBuilding: false");
            Building placedBuilding = GetComponent<Building>();
            Village currentVillage = GameManger.instance.GetSelectedVillage();

            NetworkController netController = GameManger.instance.GetLocalNetworkController();
            netController.SpawnBuilding(currentVillage.gameObject, placedBuilding.buildingType, buildingPosition, buildingRotation);
            netController.AddScoreToPlayer(GameManger.instance.GetLocalPlayer().gameObject, placedBuilding.score);

            UIManager.instance.SetVillageInfo(currentVillage);

            Destroy(this.gameObject);
        }
        else
        {
            MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.error.overbuilding"), 5.0f, Color.red);

            Destroy(this.gameObject);
        }
    }
    private void AddResourcesToVillage()
    {
        int percentage = 100;
        float amount = 0.0f;

        if (building.GetVillage().owner == GameManger.instance.GetLocalPlayer().playerID)
        {
            if (building.GetVillage().HasStoragePlace(resourceType))
            {
                if (objectType == ObjectType.Tree)
                {
                    percentage = this.gameObject.GetComponent<Grow>().age * 10;
                }

                amount = (GetComponent<ResourcePoint>().amount / 100.0f) * percentage;

                if (resourceType != ResourceType.Mana)
                {
                    netController.AddResourcesToVillage(building.GetVillage().gameObject, (int)amount, resourceType, (int)netController.netId.Value, building.GetParticleSpawnPoint().transform.position);
                    netController.AddScoreToPlayer(GameManger.instance.GetLocalPlayer().gameObject, resourceScore);
                    GameJoltManager.instance.AddData("resourcesAdded", amount.ToString());
                }
                else
                {
                    GameManger.instance.GetLocalPlayer().mana += (int)amount;
                    GameJoltManager.instance.AddData("villagerSacrificed", "1");
                    GameJoltManager.instance.AddData("manaAdded", amount.ToString());
                }
                netController.RemoveObject(this.gameObject);

                UIManager.instance.SetVillageInfo(building.GetVillage());
            }
            else //Warehouse full
            {
                MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.error.resourcefull"), 5.0f, Color.red);
                SetPosition(originalPosition);

                building = null;
                overBuilding = false;
            }
        }
        else //Village not owned by player
        {
            MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.error.notvillageowner"), 5.0f, Color.red);
            SetPosition(originalPosition);
        }
    }

    private bool CanMoveObject()
    {
        if (objectType == ObjectType.Building)
        {
            Building movableBuilding = GetComponent<Building>();

            if (movableBuilding != null)
            {
                if (!movableBuilding.canMove)
                {
                    return false;
                }
            }
        }

        return true;
    }
    private bool CanPickup()
    {
        if (transform.root != null)//Get village
        {
            Village parentController = transform.root.GetComponent<Village>();

            if (parentController != null)
            {
                if (parentController.GetOwner() != GameManger.instance.GetLocalPlayer().playerID)
                {
                    MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.error.cannotpickup"), 5.0f, Color.red);
                    return false;
                }
            }
        }

        return true;
    }

    public void SetNetworkController(NetworkController netController)
    {
        this.netController = netController;
    }
    private void SetPosition(Vector3 position)
    {
        this.gameObject.transform.position = position;
        netController.ChangePosition(this.gameObject, position);
    }

    void OnTriggerEnter(Collider other)
    {
        building = other.GetComponent<Building>();

        if (building != null)
        {
            Debug.Log("overBuilding: True");
            overBuilding = true;
        }
    }
    void OnTriggerExit(Collider other)
    {
        building = other.GetComponent<Building>();

        if (building != null)
        {
            Debug.Log("overBuilding: False");
            //GetComponent<Building>().SetAlpha(1.0f);
            overBuilding = false;
        }
    }
}