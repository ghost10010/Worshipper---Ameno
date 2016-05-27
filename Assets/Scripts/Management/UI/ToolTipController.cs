using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ToolTipController : MonoBehaviour
{
    public static ToolTipController instance;

    public string tooltipCurrent = "";
    public string tooltipOldValue = "";

    [SerializeField] private GameObject pnlPanel = null;
    [SerializeField] private Text txtMana;
    [SerializeField] private Text txtScore;

    [SerializeField] private Text txtFood = null;
    [SerializeField] private Text txtWood = null;
    [SerializeField] private Text txtIron = null;
    [SerializeField] private Text txtVillagers = null;

    [SerializeField] private PowerInfoController powerInfo;

    void Awake()
    {
        instance = this;
    }

    public void SetState(bool state)
    {
        pnlPanel.SetActive(state);
    }

    public void EnterToolTip(string key)
    {
        UIManager.instance.PointerEnter();

        tooltipCurrent = key;

        string oldKey = "";

        if (key.Contains("Power"))
        {
            oldKey = key;
            key = "Power";
        }

        switch (key)
        {
            case "Mana":
                tooltipOldValue = txtMana.text;
                txtMana.text = Language.instance.GetTextWithKey("resource.name.Mana");
                break;
            case "Score":
                tooltipOldValue = txtScore.text;
                txtScore.text = Language.instance.GetTextWithKey("resource.name.Score");
                break;
            case "Power":
                tooltipCurrent = "Power";
                int powerId = -1;
                if (int.TryParse(oldKey.Substring(oldKey.Length - 1), out powerId))
                {
                    powerInfo.SetState(true);
                    powerInfo.ShowPowerInfo(GameManger.instance.GetPower(powerId));
                }
                break;
            case "Villagers":
                tooltipOldValue = txtVillagers.text;
                txtVillagers.text = Language.instance.GetTextWithKey("resource.name.Villagers");
                break;
            case "Food":
                tooltipOldValue = txtFood.text;
                txtFood.text = Language.instance.GetTextWithKey("resource.name.Food");
                break;
            case "Wood":
                tooltipOldValue = txtWood.text;
                txtWood.text = Language.instance.GetTextWithKey("resource.name.Wood");
                break;
            case "Iron":
                tooltipOldValue = txtIron.text;
                txtIron.text = Language.instance.GetTextWithKey("resource.name.Iron");
                break;
        }
    }
    public void ExitToolTip()
    {
        UIManager.instance.PointerExit();

        switch (tooltipCurrent)
        {
            case "Mana":
                txtMana.text = tooltipOldValue;
                break;
            case "Score":
                txtScore.text = tooltipOldValue;
                break;
            case "Power":
                powerInfo.SetState(false);
                break;
            case "Villagers":
                txtVillagers.text = tooltipOldValue;
                break;
            case "Food":
                txtFood.text = tooltipOldValue;
                break;
            case "Wood":
                txtWood.text = tooltipOldValue;
                break;
            case "Iron":
                txtIron.text = tooltipOldValue;
                break;
        }

        tooltipCurrent = "";
        tooltipOldValue = "";
    }
}