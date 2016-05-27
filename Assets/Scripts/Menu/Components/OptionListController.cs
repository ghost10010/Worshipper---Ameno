using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OptionListController : ComponentBehaviour
{
	public List<string> Options;

	private int _SelectedOption = 0;
	private int _UsedOption = 0;

	[SerializeField] private Text _TxtNewOption;
	[SerializeField] private Text _TxtCurrentOption;

	private float _InputTimer = 0.2f;
	private float _InputTime = 0.2f;

	private bool _NextInput = false;
	private bool _PauseMenu = false;
	private bool _DefaultValueIsSet = false;

	[SerializeField] private MenuController _MenuScript;
	[SerializeField] private GameObject _BtnLeft;
	[SerializeField] private GameObject _BtnRight;

	[SerializeField] private bool _NeedsConformation = false;
	[SerializeField] private bool _TranslateOptions = false;

	// Use this for initialization
	void Start ()
	{
		_MenuScript = GetComponentInParent<MenuController>();
		_PauseMenu = _MenuScript.PauseMenu;

		UpdateLanguage ();

		if (Options.Count > 0)
		{
			SetDefaultValue(0);
		}
	}

	void Update()
	{
		int PlayerID = Settings.instance.PlayerPaused;

		if (PlayerID != -1)
		{
			if (Input.GetAxisRaw("P" + Settings.instance.FirstJoined + "Right") == 0 && _PauseMenu)
			{
				_NextInput = true;
			}
		}
	}

	public override void InputLeft ()
	{
		SelectOption(1);
	}
	public override void InputRight ()
	{
		SelectOption(-1);
	}
	public override void InputClick ()
	{
		ClickOption ();
	}

	public void OnMouseEnter(string Type) //PointerEnter
	{
		switch (Type)
		{
		case "Left": 
			_BtnLeft.GetComponent<ComponentBehaviour>().SelectComponent();
			_BtnRight.GetComponent<ComponentBehaviour>().DeselectComponent();
			break;
		case "Right": 
			_BtnLeft.GetComponent<ComponentBehaviour>().DeselectComponent();
			_BtnRight.GetComponent<ComponentBehaviour>().SelectComponent();
			break;
		}

		_MenuScript.SelectComponentWithID (MenuID);
		_MenuScript.GetMainMenu ().ResetTimer ();
	}
	public void OnMouseExit(string Type) //PointerExit
	{
		if (Type == "Left" || Type == "Right")
		{
			_BtnLeft.GetComponent<ComponentBehaviour>().DeselectComponent();
			_BtnRight.GetComponent<ComponentBehaviour>().DeselectComponent();
		}
	}
	
	public void OnMouseDown(string Type) //PointerClick
	{
		switch (Type)
		{
		case "Left": SelectOption(1); break;
		case "Right": SelectOption(-1); break;
		case "List": ClickOption(); break;
		}
	}

	public void SelectOption(int NextID)
	{
		_NextInput = false;
		_InputTimer = _InputTime;

        _SelectedOption -= NextID;
		
		if (_SelectedOption < 0)
		{
			_SelectedOption = 0;
		}
		else if (_SelectedOption >= Options.Count)
		{
			_SelectedOption = (Options.Count - 1);
		}
		else
		{
			_TxtNewOption.text = Options [_SelectedOption];
		}
        
		if (!_NeedsConformation)
		{
			ClickOption();
		}
	}

	void ClickOption()
	{
		_UsedOption = _SelectedOption;
		_TxtCurrentOption.text = Options[_UsedOption];

		switch(ComponentID)
		{
		case 125: //Resolution switcher
			ResolutionManager.instance.ChangeResolution(Options[_SelectedOption]);
			break;
		case 127:
			QualityManager.instance.ChangeQuality(_SelectedOption);
			break;
		}
	}

	public void SetDefaultValue(string Value)
	{
		for (int index = 0; index < Options.Count; index++)
		{
			if (Options[index] == Value)
			{
				_UsedOption = index;
				_SelectedOption = index;
			}
		}
	}
	public void SetDefaultValue(int ID)
	{
		if (!_DefaultValueIsSet)
		{
			int DefaultValue = -1;

			if (ID == -1 && ID < Options.Count)
			{
				DefaultValue = 0;
			}
			else
			{
				DefaultValue = ID;
			}

			_UsedOption = DefaultValue;
			_SelectedOption = DefaultValue;
			
			_TxtCurrentOption.text = Options[DefaultValue];
			_TxtNewOption.text = Options [DefaultValue];

			_DefaultValueIsSet = true;
		}
	}

	public string GetText(string Key)
	{
		string Text = "";

		if (_TranslateOptions)
		{
			Text = Language.instance.GetTextWithKey(Key);
		}
		else
		{
			Text = Key;
		}

		return Text;
	}

	public void AddOption(string Option)
	{
		Options.Add (Option);
		_TxtNewOption.text = Options [0];
	}
	public string GetSelectedOption()
	{
		return Options[_SelectedOption];
	}

	public override void UpdateLanguage ()
	{
		for (int index = 0; index < Options.Count; index++)
		{
			Options[index] = GetText(Options[index]);
		}

		base.UpdateLanguage ();
	}
}