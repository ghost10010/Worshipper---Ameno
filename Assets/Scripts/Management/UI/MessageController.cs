using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MessageController : MonoBehaviour
{
    public static MessageController instance;

    [SerializeField] private GameObject pnlMessage = null;
    [SerializeField] private Text txtMessage = null;

    public float messageTimer = 0.0f;
    public bool showMessage = false;

    // Use this for initialization
    void Start () {
	
	}

    void Awake()
    {
        instance = this;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (showMessage)
        {
            messageTimer -= Time.deltaTime;

            if (messageTimer <= 0.0f)
            {
                pnlMessage.SetActive(false);

                showMessage = false;
            }
        }
    }

    public void ShowMessage(string text, float time, Color textColor)
    {
        if (UIInitilized())
        {
            pnlMessage.SetActive(true);

            //txtMessage.color = textColor;
            txtMessage.text = text;

            showMessage = true;
            messageTimer = time;
        }
    }

    private bool UIInitilized()
    {
        if (pnlMessage != null)
        {
            return true;
        }
        return false;
    }
}