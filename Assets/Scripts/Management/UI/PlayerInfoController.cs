using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerInfoController : MonoBehaviour
{
    [SerializeField] private GameObject pnlPanel;

    [SerializeField] private Image imgMana = null;
    [SerializeField] private ParticleSystem manaParticles = null;

    [SerializeField] private Text txtMana;
    [SerializeField] private Text txtScore;

    public void SetState(bool state)
    {

    }

    public void ShowPlayerInfo(Player player)
    {
        string toolTipCurrent = ToolTipController.instance.tooltipCurrent;

        if (toolTipCurrent != "Mana")
        {
            txtMana.text = player.mana + "/" + player.maxMana;
        }
        else
        {
            ToolTipController.instance.tooltipOldValue = player.mana + "/" + player.maxMana;
        }

        if (toolTipCurrent != "Score")
        {
            txtScore.text = player.score.ToString();
        }
        else
        {
            ToolTipController.instance.tooltipOldValue = player.score.ToString();
        }

        float calc1 = 1 / ((float)player.maxMana);
        float calc2 = calc1 * ((float)player.mana);

        imgMana.fillAmount = calc2;

        manaParticles.Play();
    }
}
