using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VillageInfoController : MonoBehaviour
{
    [SerializeField] private GameObject pnlPanel = null;
    [SerializeField] private Text txtFood = null;
    [SerializeField] private Text txtFoodBalance = null;
    [SerializeField] private Image imgFood = null;
    [SerializeField] private Text txtWood = null;
    [SerializeField] private Text txtWoodBalance = null;
    [SerializeField] private Image imgWood = null;
    [SerializeField] private Text txtIron = null;
    [SerializeField] private Text txtIronBalance = null;
    [SerializeField] private Image imgIron = null;
    [SerializeField] private Text txtVillagers = null;

    [SerializeField] private Sprite sptFood = null;
    [SerializeField] private Sprite sptFoodEmpty = null;
    [SerializeField] private Sprite sptWood = null;
    [SerializeField] private Sprite sptWoodEmpty = null;
    [SerializeField] private Sprite sptIron = null;
    [SerializeField] private Sprite sptIronEmpty = null;

    private int offset = 50;

    public void SetState(bool state)
    {
        pnlPanel.SetActive(state);
    }

    public void ShowVillageInfo(Village village)
    {
        //txtFood.text = village.food + " / " + village.maxFood;
        //txtWood.text = village.wood + " / " + village.maxWood;
        //txtIron.text = village.iron + " / " + village.maxIron;

        //if (ToolTipController.instance.tooltipCurrent != "Villagers")
        //{
        //    txtVillagers.text = village.villagers + " / " + village.soldiers + " / " + (village.maxVillagers + village.maxSoldiers);
        //}
        //else
        //{
        //    ToolTipController.instance.tooltipOldValue = village.villagers + " / " + village.soldiers + " / " + (village.maxVillagers + village.maxSoldiers);
        //}
        //txtVillagers.text = village.villagers + " / " + village.soldiers + " / " + (village.maxVillagers + village.maxSoldiers);

        SetVillageText(txtFood, "Food", village.food + " / " + village.maxFood);
        SetVillageText(txtWood, "Wood", village.wood + " / " + village.maxWood);
        SetVillageText(txtIron, "Iron", village.iron + " / " + village.maxIron);
        SetVillageText(txtVillagers, "Villagers", village.villagers + " / " + village.soldiers + " / " + (village.maxVillagers + village.maxSoldiers));

        SetResourceBalance(txtFoodBalance, (village.increaseFood - village.decreaseFood));
        SetResourceBalance(txtWoodBalance, (village.increaseWood - village.decreaseWood));
        SetResourceBalance(txtIronBalance, (village.increaseIron - village.decreaseIron));

        if (CheckToolTip("Food"))
        {
            SetTotalBalance(txtFood, imgFood, village.food, (0 + offset), (village.maxFood - offset), sptFood, sptFoodEmpty);
        }
        if (CheckToolTip("Wood"))
        {
            SetTotalBalance(txtWood, imgWood, village.wood, (0 + offset), (village.maxWood - offset), sptWood, sptWoodEmpty);
        }
        if (CheckToolTip("Iron"))
        {
            SetTotalBalance(txtIron, imgIron, village.iron, (0 + offset), (village.maxIron - offset), sptIron, sptIronEmpty);
        }
    }
    private void SetVillageText(Text txt, string type, string text)
    {
        if (CheckToolTip(type))
        {
            txt.text = text;
        }
        else
        {
            ToolTipController.instance.tooltipOldValue = text;
        }
    }
    private bool CheckToolTip(string type)
    {
        if (ToolTipController.instance.tooltipCurrent != type)
        {
            return true;
        }

        return false;
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
            txt.color = Color.white;
        }
        else
        {
            txt.text = "+ ";
            txt.color = Color.green;
        }

        txt.text += Mathf.Abs(value).ToString();
    }
    private void SetTotalBalance(Text txt, Image img, int value, int lowTreshold, int highTreshold, Sprite sptNormal, Sprite sptLow)
    {
        if (value <= lowTreshold)
        {
            img.sprite = sptLow;
            txt.color = Color.red;
        }
        else if (value >= highTreshold)
        {
            img.sprite = sptNormal;
            txt.color = Color.green;
        }
        else
        {
            img.sprite = sptNormal;
            txt.color = Color.black;
        }
    }
}
