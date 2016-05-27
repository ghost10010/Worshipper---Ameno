using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreObject : MonoBehaviour
{
    public Text txtId;
    public Text txtUserName;
    public Text txtScore;

    public void Init(int Id, GameJolt.API.Objects.Score score)
    {
        Debug.Log("Score: " + score.UserName);

        if (score.UserName != "")
        {
            txtUserName.text = score.UserName;
        }
        else
        {
            txtUserName.text = "Guest";
        }

        txtId.text = Id.ToString();
        txtScore.text = score.Value.ToString();
    }

    public void Hide()
    {
        txtId.text = "";
        txtUserName.text = "";
        txtScore.text = "";
    }
}
