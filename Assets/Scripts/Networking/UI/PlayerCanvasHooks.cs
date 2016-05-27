using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerCanvasHooks : MonoBehaviour
{
	public delegate void CanvasHook();

	public CanvasHook OnReadyHook;
	public CanvasHook OnColorChangeHook;
	public CanvasHook OnRemoveHook;

	public ButtonController playButton;
	public ButtonController colorButton;
	public ButtonController removeButton;
	public Text readyText;
	public Text nameText;
	public RectTransform panelPos;

	bool isLocalPlayer;

	void Awake()
	{
		removeButton.gameObject.SetActive(false);
	}

	public void UIReady()
	{
		if (OnReadyHook != null)
        {
            OnReadyHook.Invoke();
        }
			
	}

	public void UIColorChange()
	{
		if (OnColorChangeHook != null)
			OnColorChangeHook.Invoke();
	}

	public void UIRemove()
	{
		if (OnRemoveHook != null)
			OnRemoveHook.Invoke();
	}

	public void SetLocalPlayer()
	{
        UIManager.instance.AddDebug("SetLocalPlayer: " + Language.instance.GetTextWithKey("lobby.btn.Ready") + "\n");
		isLocalPlayer = true;
		nameText.text = "YOU";
        readyText.text = Language.instance.GetTextWithKey("lobby.btn.Ready");
        playButton.SetKey("lobby.btn.Ready");
        removeButton.gameObject.SetActive(true);
	}

	public void SetColor(Color color)
	{
        if (colorButton != null)
        {
            colorButton.GetComponent<Image>().color = color;
        }
	}

	public void SetReady(bool ready)
	{
        Debug.Log("SetReady: " + ready);
		if (ready)
		{
            playButton.SetKey("lobby.btn.Ready");
            readyText.text = Language.instance.GetTextWithKey("lobby.btn.Ready");
        }
		else
		{
			if (isLocalPlayer)
			{
                playButton.SetKey("lobby.btn.Ready");
                readyText.text = Language.instance.GetTextWithKey("lobby.btn.Ready");
            }
			else
			{
                playButton.SetKey("lobby.btn.Unready");
                readyText.text = Language.instance.GetTextWithKey("lobby.btn.Unready");
            }
		}
	}
}
