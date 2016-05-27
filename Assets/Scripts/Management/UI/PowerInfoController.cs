using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PowerInfoController : MonoBehaviour
{
    [SerializeField] private GameObject pnlPanel;
    [SerializeField] private Text txtPowerName;
    [SerializeField] private Text txtPowerCost;
    [SerializeField] private Text txtPowerEffect;

    public void SetState(bool state)
    {
        pnlPanel.SetActive(state);
    }

    public void ShowPowerInfo(Power power)
    {
        Debug.Log("ShowPowerInfo: " + power.nameKey);
        txtPowerName.text = Language.instance.GetTextWithKey(power.nameKey);
        txtPowerCost.text = power.manaCost.ToString();
        txtPowerEffect.text = Language.instance.GetTextWithKey(power.effectKey);
    }
}
