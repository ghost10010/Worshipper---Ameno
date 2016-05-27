using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BuildingInfoController : InfoController
{
    [SerializeField] private GameObject pnlPanel = null;
    [SerializeField] private Text txtBuildingName = null;
    [SerializeField] private Text txtBuildingFood = null;
    [SerializeField] private Text txtBuildingWood = null;
    [SerializeField] private Text txtBuildingIron = null;
    [SerializeField] private Text txtBuildingDescription = null;

    [SerializeField] private string type = "";

    public override void SetState(bool state)
    {
        pnlPanel.SetActive(state);
    }

    public void ShowInfo(Building building)
    {
        txtBuildingName.text = Language.instance.GetTextWithKey(building.buildingKey);

        if (type == "Info")
        {
            txtBuildingFood.text = building.upkeepFood + " / " + building.increaseFood;
            txtBuildingWood.text = building.upkeepWood + " / " + building.increaseWood;
            txtBuildingIron.text = building.upkeepIron + " / " + building.increaseIron;
        }
        else if (type == "Build")
        {
            txtBuildingFood.text = building.buildCostFood + " / " + building.increaseFood;
            txtBuildingWood.text = building.buildCostWood + " / " + building.increaseWood;
            txtBuildingIron.text = building.buildCostIron + " / " + building.increaseIron;
        }
        
        txtBuildingDescription.text = Language.instance.GetTextWithKey(building.descriptionKey);
    }

    public void EnterBuildingButton(int buildingId)
    {
        SetState(true);
        UIManager.instance.PointerEnter();
        ShowInfo(GameManger.instance.GetBuilding(buildingId).GetComponent<Building>());
    }
    public void ExitBuildingButton()
    {
        SetState(false);
        UIManager.instance.PointerExit();
    }
}
