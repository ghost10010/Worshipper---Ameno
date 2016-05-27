using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TrophyObject : MonoBehaviour
{
    public CanvasGroup group;
    public Image image;
    public Text txtTitle;
    public Text txtDescription;

    public void Init(GameJolt.API.Objects.Trophy trophy)
    {
        Debug.Log(trophy.Title + " is " + trophy.Unlocked);
        group.alpha = trophy.Unlocked ? 1f : .6f;
        txtTitle.text = trophy.Title;
        txtDescription.text = trophy.Description;

        if (trophy.Image != null)
        {
            image.sprite = trophy.Image;
        }
        else
        {
            trophy.DownloadImage((success) => {
                if (success)
                {
                    image.sprite = trophy.Image;
                }
            });
        }
    }
}
