using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class VillagerInfoController : MonoBehaviour
{
    [SerializeField] private GameObject pnlPanel = null;
    [SerializeField] private Text txtVillagerType = null;
    [SerializeField] private Text txtVillagerFood = null;
    [SerializeField] private Text txtVillagerWood = null;
    [SerializeField] private Text txtVillagerIron = null;
    [SerializeField] private Text txtVillagerHealth = null;
    [SerializeField] private Text txtVillagerArmor = null;
    [SerializeField] private Text txtVillagerSpeed = null;

    public void SetState(bool state)
    {
        pnlPanel.SetActive(state);
    }

    public void ShowVillagerInfo(VillagerController villager)
    {
        txtVillagerType.text = villager.GetVillagerType().ToString();
        txtVillagerFood.text = villager.upkeepFood.ToString();
        txtVillagerWood.text = villager.upkeepWood.ToString();
        txtVillagerIron.text = villager.upkeepIron.ToString();
        txtVillagerArmor.text = villager.armor.ToString();
        txtVillagerSpeed.text = villager.walkSpeed.ToString();
        txtVillagerHealth.text = villager.health.ToString();
    }
}
