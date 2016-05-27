using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class TrophyWindowController : MonoBehaviour
{
    public GameObject trophyPrefab;
    public List<GameObject> trophyObjects = new List<GameObject>();

    public ScrollRect scrollRect;

    // Use this for initialization
    void Start()
    {
        Debug.Log("Trophies Start");
        //CreateList();
        //Show();
    }
    void Awake()
    {
        Debug.Log("Trophies awake");
        Show();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Show()
    {
        GameJolt.API.Trophies.Get((GameJolt.API.Objects.Trophy[] trophies) =>
        {
            if (trophies != null)
            {
                // Update children's text.
                for (int i = 0; i < trophies.Length; ++i)
                {
                    scrollRect.content.GetChild(i).GetComponent<TrophyObject>().Init(trophies[i]);
                }
            }
        });
    }
}