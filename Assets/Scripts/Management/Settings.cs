using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Settings : MonoBehaviour
{
	public static Settings instance;

    public int terrainID = 0;

	public int FirstJoined = -1; //used for menus
	public int PlayerPaused = -1; //used for pause menu
    public int PlayerCount = -1;

    public int GameSettingsMenuID = -1;

    public bool ShowPromoInMenu = false;
    public bool ShowLobby = false;
    public bool ReturnFromGame = false;
    public bool LoadFromSave = false;

    public string Version = "Alpha v0.1";

    void Start()
    {
        GetShowMenu();
    }

	void Awake ()
	{
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
	}

    public void SetShowMenu(bool menuState)
    {
        PlayerPrefs.SetInt("MenuState", (menuState ? 1 : 0));
        ShowLobby = menuState;
    }
    public void GetShowMenu()
    {
        ShowLobby = PlayerPrefs.GetInt("MenuState") == 1 ? true : false;
    }

    void OnApplicationQuit() //Delete playerprefs to disable players of previous session when restarting the game
    {
        PlayerPrefs.DeleteKey("P1");
        PlayerPrefs.DeleteKey("P2");
        PlayerPrefs.DeleteKey("P3");
        PlayerPrefs.DeleteKey("P4");
    }
}