using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
	public List<GameObject> Components;	//All menu components
	
	private float _InputTimer = 0.2f;
	private float _InputTime = 0.2f;
	private int _LastSelectedComponent = 0;
	public int _SelectedComponent = 0;
	
	private bool _NextInput = false;

	[SerializeField] private MenuMain _MenuScript;

	[SerializeField] private Dialog _Dialog;

	private string _NotAvailableText = "";

	public bool PauseMenu = false;
	public bool AutoLoadComponents = false;
	public bool LockControllers = true;

	[SerializeField] private int _PreviousMenu = 0;

	void Start()
	{
		if (_MenuScript != null)
		{
			PauseMenu = _MenuScript._PauseMenu;
		}

		for (int index = 0; index < Components.Count; index++)
		{
			Components[index].GetComponent<ComponentBehaviour>().SetMenuID(index);
		}

		MusicScript.instance.GetAudioSources ();
		MusicScript.instance.SetSoundsVolume(false);
	}

	void Awake()
	{
		GetMenuComponents ();
		ShowComponent (0);
        UpdateLanguage();

		_NotAvailableText = Language.instance.GetTextWithKey("menu.err.NotAvailable");
	}
	void GetMenuComponents()
	{
		if (AutoLoadComponents)
		{
			Components.Clear ();

			for (int index = 0; index < gameObject.transform.childCount; index++)
			{
				GameObject Child = gameObject.transform.GetChild(index).gameObject;

				if (Child.activeSelf)
				{
					ComponentBehaviour ChildComponent = null;

					if (Child.GetComponent<ComponentBehaviour>())
					{
						ChildComponent = Child.GetComponent<ComponentBehaviour>();
					}
					else if (Child.GetComponentInChildren<ComponentBehaviour>())
					{
						ChildComponent = Child.GetComponentInChildren<ComponentBehaviour>();
					}

					if (ChildComponent != null)
					{
						if (ChildComponent.ComponentType != ComponentType.PlayerJoin)
						{
                            if (MusicScript.instance != null)
                            {
                                ChildComponent.gameObject.GetComponent<AudioSource>().volume = MusicScript.instance.GetSoundsVolumeWithMax();
                            }
							Components.Add(ChildComponent.gameObject);
						}
					}
				}
			}
		}
	}
	public void PressButton(int ButtonID) 
	{   //a button is pressed
		if (Components[_SelectedComponent].GetComponent<ComponentBehaviour>().IsAvailable)
		{
            if (ButtonID == 1) //ChangeScene
            {
                int NextScene = Components[_SelectedComponent].GetComponent<ButtonController>().GetNextScene();

                if (NextScene != -1)
                {
                    Application.LoadLevel(NextScene);
                }
            }
            if (ButtonID == 2) //ChangeMenu
            {
                ShowNextMenu(Components[_SelectedComponent].GetComponent<ButtonController>().GetNextMenu());
                /*int NextLevel = Components[_SelectedComponent].GetComponent<ButtonController>().GetNextMenu();

                if (NextLevel != -1)
                {
                    HideComponent(_SelectedComponent);
                    ShowComponent(0);

                    _SelectedComponent = 0;
                    _LastSelectedComponent = 0;

                    _MenuScript.ShowMenu(NextLevel);

                    MusicScript.instance.GetAudioSources();
                    MusicScript.instance.SetSoundsVolume(false);
                }
                else
                {
                    ShowError(DialogType.Error, "Cannot load the menu", -1);
                }*/
            }
            else if (ButtonID == 3) //btnQuit
            {
                PlayerPrefs.DeleteKey("MenuState");
                Application.Quit();
            }
            else if (ButtonID == 4) //open lobby
            {
                Settings.instance.SetShowMenu(true);
                _MenuScript.HideMenu();

                GuiLobbyManager lobbyManager = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<GuiLobbyManager>();
                //lobbyManager.playScene = "Game.unity";
                lobbyManager.ShowLobby();
            }
            else if (ButtonID == 5) //close lobby
            {
                Settings.instance.SetShowMenu(false);

                GuiLobbyManager lobbyManager = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<GuiLobbyManager>();
                lobbyManager.HideLobby();

                if (_MenuScript == null)
                {
                    _MenuScript = FindObjectOfType<MenuMain>();
                }
                _MenuScript.ShowMenu(0);
            }
            else if (ButtonID == 6)
            {
                Settings.instance.SetShowMenu(true);
                _MenuScript.HideMenu();

                GuiLobbyManager lobbyManager = GameObject.FindGameObjectWithTag("LobbyManager").GetComponent<GuiLobbyManager>();
                lobbyManager.ShowLobby();
            }
            else if (ButtonID == 7) //Trophies
            {
                if (GameJoltManager.instance.loggedIn)
                {
                    ShowNextMenu(Components[_SelectedComponent].GetComponent<ButtonController>().GetNextMenu());
                    GameObject.FindObjectOfType<TrophyWindowController>().Show();
                }
                else
                {
                    Debug.Log("No GameJolt data");
                }
            }
            else if (ButtonID == 8) //Leaderboards
            {
                ShowNextMenu(Components[_SelectedComponent].GetComponent<ButtonController>().GetNextMenu());
                GameObject.FindObjectOfType<LeaderboardWindowController>().Show();
            }
            else if (ButtonID == 9) //Stats
            {

            }
            else if (ButtonID == 21) //Language EN
            {
                Language.instance.SetLanguage("en");
            }
            else if (ButtonID == 22) //Language NL
            {
                Language.instance.SetLanguage("nl");
            }
		}
		else
		{
			ShowError(DialogType.Error, _NotAvailableText, -1);
		}
	}
    void ShowNextMenu(int levelId)
    {
        if (levelId != -1)
        {
            HideComponent(_SelectedComponent);
            ShowComponent(0);

            _SelectedComponent = 0;
            _LastSelectedComponent = 0;

            _MenuScript.ShowMenu(levelId);

            MusicScript.instance.GetAudioSources();
            MusicScript.instance.SetSoundsVolume(false);
        }
        else
        {
            ShowError(DialogType.Error, "Cannot load the menu", -1);
        }
    }
	void ShowError(DialogType Type, string ErrorText, int ActionID)
	{
		if (_Dialog != null)
		{
			_Dialog.SetupDialog (Type, ErrorText, ActionID);
			_Dialog.ShowDialog ();
		}
	}
	void HideError()
	{
		if (_Dialog != null)
		{
			_Dialog.HideDialog ();
		}
	}
	void SelectComponent(int NextId)
	{
		ResetInputTimer ();
		
		_LastSelectedComponent = _SelectedComponent;
		_SelectedComponent -= NextId;
		
		if (_SelectedComponent < 0)
		{
			_SelectedComponent = 0;
		}
		else if (_SelectedComponent >= Components.Count)
		{
			_SelectedComponent = (Components.Count - 1);
		}
		else
		{
			HideError();
			ShowComponent(_SelectedComponent);
			HideComponent(_LastSelectedComponent);
		}

		_MenuScript.ResetTimer ();
	}
	public void SelectComponentWithID(int ButtonId)
	{
		for (int index = 0; index < Components.Count; index++)
		{
			int CurrentButtonId = Components[index].GetComponent<ComponentBehaviour>().MenuID;

			if (CurrentButtonId == ButtonId)
			{
				_LastSelectedComponent = _SelectedComponent;
				_SelectedComponent = index;

				HideError();
				HideComponent(_LastSelectedComponent);
				ShowComponent(_SelectedComponent);
				break;
			}
		}

		//_MenuScript.ResetTimer ();
	}
	void HideComponent(int ComponentID)
	{
		ComponentBehaviour _Component = Components [ComponentID].GetComponent<ComponentBehaviour> ();
		_Component.DeselectComponent();
	}
	void ShowComponent(int ComponentID)
	{
		ComponentBehaviour _Component = Components [ComponentID].GetComponent<ComponentBehaviour> ();
		_Component.SelectComponent();
	}

	void ResetInputTimer()
	{
		_NextInput = false;

		_InputTimer = _InputTime;
	}

	public void UpdateLanguage()
	{
		for (int index = 0; index < Components.Count; index++)
		{
			Components[index].GetComponent<ComponentBehaviour>().UpdateLanguage();
		}
	}

	public MenuMain GetMainMenu()
	{
		return _MenuScript;
	}
}