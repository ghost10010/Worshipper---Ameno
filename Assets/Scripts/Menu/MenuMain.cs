using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuMain : MonoBehaviour 
{
	public GameObject[] PlayerObjects;
	public GameObject[] MenuObjects;	//All menu gameobjects

	public float _Timer = 25.0f;
	private float _Time = 25.0f;

	public Text TxtBuildVersion;

	public int TotalPlayers = 0;

	public bool _PauseMenu = false;
	public bool _LoadMenu = false;

	void Start()
	{
		if (!_PauseMenu)
		{
            if (Settings.instance.GameSettingsMenuID == -1)
            {
                if (Settings.instance.ShowLobby)
                {
                    MenuObjects[0].GetComponent<MenuController>().UpdateLanguage();
                    HideMenu();
                }
                else if (Settings.instance.ReturnFromGame)
                {
                    Settings.instance.ReturnFromGame = false;
                    MenuObjects[0].GetComponent<MenuController>().UpdateLanguage();
                    HideMenu();
                }
                else
                {
                    UpdateBuildNumber();

                    if (GameJoltManager.instance.loggedIn)
                    {
                        ShowMenu(0);//show main menu
                    }
                    else
                    {
                        ShowMenu(8);//show login menu
                    }
                }
            }
            else
            {
                ShowMenu(Settings.instance.GameSettingsMenuID);
                Settings.instance.GameSettingsMenuID = -1;
            }
		}
		if (!_LoadMenu)
		{
        	GetPlayers();
		}
    }

	void Update()
	{
		if (Settings.instance.ShowPromoInMenu)
		{
			if (_Timer > 0 && !_PauseMenu && !_LoadMenu)
			{
				_Timer -= Time.deltaTime;

				if (_Timer < 0)
				{
					Application.LoadLevel("Promo");
				}
			}
		}
	}

	void Awake()
    {
		//show the cursor
        Cursor.visible = true;
    }

	public void ShowMenu(int MenuId)
	{
		for (int index = 0; index < MenuObjects.Length; index++)
		{
			MenuObjects[index].SetActive(false);
		}

		MenuObjects [MenuId].SetActive (true);
        MenuObjects[MenuId].GetComponent<MenuController>().UpdateLanguage();

		ResetTimer ();
	}
    public void HideMenu()
    {
        for (int index = 0; index < MenuObjects.Length; index++)
        {
            MenuObjects[index].SetActive(false);
        }
    }

    private void GetPlayers() //Get previous session's players when returning to menu
    {
		if (PlayerObjects.Length > 0)
		{
			if (PlayerPrefs.GetInt("P1") == 1)
			{
				PlayerObjects[0].GetComponent<PlayerJoin>().SelectPlayer();
			}
			if (PlayerPrefs.GetInt("P2") == 1)
			{
				PlayerObjects[1].GetComponent<PlayerJoin>().SelectPlayer();
			}
			if (PlayerPrefs.GetInt("P3") == 1)
			{
				PlayerObjects[2].GetComponent<PlayerJoin>().SelectPlayer();
			}
			if (PlayerPrefs.GetInt("P4") == 1)
			{
				PlayerObjects[3].GetComponent<PlayerJoin>().SelectPlayer();
			}
		}
    }

	public int SetPlayers()
	{
		int PlayerCount = -1;
		if (TotalPlayers > 0)
		{
			int count = 0;

			if (PlayerObjects[0].GetComponent<PlayerJoin>().Joined) { count++; PlayerPrefs.SetInt("P1", 1); } else { PlayerPrefs.SetInt("P1", 0); }
			if (PlayerObjects[1].GetComponent<PlayerJoin>().Joined) { count++; PlayerPrefs.SetInt("P2", 1); } else { PlayerPrefs.SetInt("P2", 0); }
			if (PlayerObjects[2].GetComponent<PlayerJoin>().Joined) { count++; PlayerPrefs.SetInt("P3", 1); } else { PlayerPrefs.SetInt("P3", 0); }
			if (PlayerObjects[3].GetComponent<PlayerJoin>().Joined) { count++; PlayerPrefs.SetInt("P4", 1); } else { PlayerPrefs.SetInt("P4", 0); }

			PlayerCount = count;
			Settings.instance.PlayerCount = PlayerCount;
		}

		return PlayerCount;
	}
	public int GetPlayerCount()
	{
		int PlayerCount = 0;

		if (PlayerObjects[0].GetComponent<PlayerJoin>().Joined) { PlayerCount++; }
		if (PlayerObjects[1].GetComponent<PlayerJoin>().Joined) { PlayerCount++; }
		if (PlayerObjects[2].GetComponent<PlayerJoin>().Joined) { PlayerCount++; }
		if (PlayerObjects[3].GetComponent<PlayerJoin>().Joined) { PlayerCount++; }
		
		return PlayerCount;
	}

	public void AddPlayer(int PlayerID)
	{
		ResetTimer ();

		if (Settings.instance.FirstJoined == -1)
		{
			Settings.instance.FirstJoined = PlayerID;
		}
		TotalPlayers++;
	}
	public void RemovePlayer(int PlayerID)
	{
		ResetTimer ();

		if (Settings.instance.FirstJoined == PlayerID)
		{
			Settings.instance.FirstJoined = -1;

			for (int index = 0; index < PlayerObjects.Length; index++)
			{
				if (PlayerObjects[index].GetComponent<PlayerJoin>().Joined == true && (index + 1) != PlayerID)
				{
					Settings.instance.FirstJoined = (index + 1);
				}
			}
		}

		TotalPlayers--;
	}

	public void UpdateLanguage()
	{
		for (int index = 0; index < MenuObjects.Length; index++)
		{
			MenuObjects[index].GetComponent<MenuController>().UpdateLanguage();
		}

		UpdateBuildNumber ();
	}

	void UpdateBuildNumber()
	{
		if (TxtBuildVersion != null)
		{
			TxtBuildVersion.text = Language.instance.GetTextWithKey("menu.txt.Build") + Settings.instance.Version;
		}
	}

	public void ResetTimer()
	{
		_Timer = _Time;
	}
}
