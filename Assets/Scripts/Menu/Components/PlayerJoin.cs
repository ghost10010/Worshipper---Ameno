//Victor Adamse
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerJoin : ComponentBehaviour
{
    [HideInInspector]
    public bool Joined;
	public int PlayerID;

	public bool IsSinglePlayer = false;
	public bool IsMultiplayer = false;

	[SerializeField]
	private MenuMain _MenuScript;

	void Update()
	{
		if (!_MenuScript._PauseMenu)
		{
			if(Input.GetButtonDown("P" + PlayerID + "Start"))
			{
				if (Joined)
				{
					DeselectPlayer();
				}
				else
				{
					SelectPlayer();
				}
			}
		}
	}

	public void SelectPlayer()
	{
		Joined = true;

		_MenuScript.AddPlayer (PlayerID);

		base.SelectComponent ();
	}

	public void DeselectPlayer()
	{
		Joined = false;

		_MenuScript.RemovePlayer (PlayerID);

		base.DeselectComponent ();
	}

	public void ClickPlayerJoin()
	{
		if (Joined)
		{
			DeselectPlayer();
		}
		else
		{
			SelectPlayer();
		}
	}
}