using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AreaOfEffect : MonoBehaviour
{
    private Projector projector;

    private bool showProjector = false;

    private PowerType powerType;
    private Power power;

    [SerializeField] private GameObject parent;

    [SerializeField] private List<Material> projectorMaterials;

    private TerrainDeformer terrainDeformer;

	// Use this for initialization
	void Start ()
    {
        projector = GetComponent<Projector>();

        if (GameManger.instance != null)
        {
            terrainDeformer = GameManger.instance.GetTerrainDeformer();
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (showProjector && !UIManager.instance.IsMouseOverUI())
        {
            Vector3 position = Mouse.instance.GetMouseWorldPosition();

            if (position != Vector3.zero)
            {
                position.y = 100.0f;
                this.gameObject.transform.position = position;
            }

            if (Input.GetMouseButtonDown(0))
            {
                CallPowerEffect();
                StopEffect();
            }
            if (Input.GetMouseButtonDown(1))
            {
                StopEffect();
            }

            projector.enabled = true;
        }
        else
        {
            projector.enabled = false;
        }
    }

    public void IncreaseSize()
    {
        if (power.bigger != -1)
        {
            Player player = GameManger.instance.GetLocalPlayer();

            if ((player != null && GameManger.instance.GetPower(power.bigger).manaCost <= player.mana) || GameManger.instance.isLevelEditor)
            {
                StartEffect((PowerType)power.bigger);
            }
            else
            {
                MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.error.increasepowermana"), 7.5f, Color.blue);
            }
        }
        else
        {
            MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.error.increasepowersize"), 7.5f, Color.red);
        }
    }
    public void DecreaseSize()
    {
        if (power.smaller != -1)
        {
            StartEffect((PowerType)power.smaller);
        }
        else
        {
            MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.error.decreasepowersize"), 7.5f, Color.red);
        }
    }

    public bool IsActive()
    {
        return showProjector;
    }

    private void CallPowerEffect()
    {
        Player player = GameManger.instance.GetLocalPlayer();

        if ((player != null && player.mana >= power.manaCost) || GameManger.instance.isLevelEditor)
        {
            if (!GameManger.instance.isLevelEditor)
            {
                GameJoltManager.instance.AddData("usedMana", power.manaCost.ToString());
                GameJoltManager.instance.CheckTrophyMana();

                player.mana -= power.manaCost;
            }

            if (power.type == "Terrain")
            {
                TerrainEffect();
            }
            else if (power.type == "Spawn")
            {
                SpawnEffect();
            }
        }
        else
        {
            MessageController.instance.ShowMessage(Language.instance.GetTextWithKey("game.error.nomana"), 5.0f, Color.red);
        }
    }
    private void SpawnEffect()
    {
        if (GameManger.instance.isLevelEditor)
        {

        }
        else
        {
            NetworkController networkController = GameManger.instance.GetLocalNetworkController();

            if (networkController != null)
            {
                networkController.SpawnObject(power.spawnObjects, power.areaSize, power.amount, powerType, GameManger.instance.GetLocalPlayer().playerID);
            }
        }
    }
    private void TerrainEffect()
    {
        NetworkController netController = GameManger.instance.GetLocalNetworkController();
        netController.DeformTerrain(Mouse.instance.GetMouseWorldPosition(), 10.0f);
    }

    public void StartEffect(PowerType type)
    {
        showProjector = true;
        power = GameManger.instance.GetPower((int)type);

        projector.material = projectorMaterials[power.color];
        projector.orthographicSize = power.areaSize;

        powerType = type;
    }
    public void StopEffect()
    {
        showProjector = false;

        powerType = PowerType.None;
    }
}