using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class LeaderboardWindowController : MonoBehaviour
{
    public int tableId = 123022;
    public int rowCount = 25;

    public GameObject scorePrefab;
    public List<GameObject> scoreObjects = new List<GameObject>();

    public ScrollRect scrollRect;

	// Use this for initialization
	void Start ()
    {
        //CreateList();
        Show();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Show()
    {
        GameJolt.API.Scores.Get((GameJolt.API.Objects.Score[] scores) =>
        {
            if (scores != null)
            {
                for (int index = 0; index < scores.Length; ++index)
                {
                    scrollRect.content.GetChild(index).GetComponent<ScoreObject>().Init((index + 1), scores[index]);
                }

                if (scores.Length < rowCount)
                {
                    for (int index = scores.Length; index < rowCount; index++)
                    {
                        scrollRect.content.GetChild(index).GetComponent<ScoreObject>().Hide();
                    }
                }
            }
        }, tableId, rowCount);
    }
}