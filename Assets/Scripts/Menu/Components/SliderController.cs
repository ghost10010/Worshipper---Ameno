using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SliderController : ComponentBehaviour
{
	[SerializeField] private Text _TxtValue;
	[SerializeField] private Slider _Slider;
	
	private float _InputTimer = 0.15f;
	private float _InputTime = 0.15f;
	private bool _NextInput = false;
	private bool _ControllerInput = false;
	private bool _PauseMenu = false;

	private MenuController _MenuScript;

	private int _Value = 100;
	private int _IncrementStep = 2;
	private int _TotalIncremention;

	void Start ()
	{
		_MenuScript = GetComponentInParent<MenuController> ();
		_PauseMenu = _MenuScript.PauseMenu;

		base.UpdateLanguage ();
	}

	void Awake()
	{
		//show start values
		float DefaultValue = 0.0f;

		switch(ComponentID)
		{
		case 75: //music slider
			DefaultValue = MusicScript.instance.GetMusicVolume();
			break;
		case 76: //sounds slider
			DefaultValue = MusicScript.instance.GetSoundsVolume();
			break;
		case 77: //master slider
			DefaultValue = MusicScript.instance.GetMasterVolume();
			break;
		}
		Debug.Log (DefaultValue);
		SetDefaultValue(DefaultValue);
	}

	// Update is called once per frame
	void Update ()
	{
		int PlayerID = -1;
		
		if (_PauseMenu)
		{
			PlayerID = Settings.instance.PlayerPaused;
		}
		else
		{
			PlayerID = Settings.instance.FirstJoined;
		}

		if (PlayerID != -1)
		{
			if (Input.GetAxis("P" + PlayerID + "Right") == 0)
			{
				_IncrementStep = 2;
				_TotalIncremention = 0;
			}
		}
	}

	public override void InputLeft ()
	{
		_ControllerInput = true;
		
		SetValue(-_IncrementStep);
	}
	public override void InputRight ()
	{
		_ControllerInput = true;
		
		SetValue(_IncrementStep);
	}

	public void OnMouseEnter() //PointerEnter
	{
		_MenuScript.SelectComponentWithID (MenuID);
	}
	
	public void OnMouseExit() //PointerExit
	{
	}
	
	public void OnMouseDown() //PointerClick
	{
		_MenuScript.SelectComponentWithID (MenuID);
	}
	public void OnMouseDown1()
	{
		_MenuScript.SelectComponentWithID (MenuID);
	}

	void SetValue(int ValueChange)
	{
		_Value += ValueChange;
		_TotalIncremention += Mathf.Abs(ValueChange);

		if (_Value <= 0)
		{
			_TotalIncremention = 0;
			_Value = 0;
		}
		else if (_Value > 100)
		{
			_TotalIncremention = 0;
			_Value = 100;
		}

		SetSetting(true);
		ShowValue(false);
		SetIncrementStep();

		_InputTimer = _InputTime;
		_NextInput = false;
		_MenuScript.GetMainMenu().ResetTimer();
	}
	void SetIncrementStep()
	{
		if (_TotalIncremention < 10)
		{
			_IncrementStep = 2;
		}
		else if (_TotalIncremention < 30)
		{
			_IncrementStep = 5;
		}
		else
		{
			_IncrementStep = 10;
		}
	}
	public void SetSlider(float Value)
	{
		if (!_ControllerInput)
		{
			if (Value > 0 && Value <= 1)
			{
				_Value = (int)(Value * 100);

				_IncrementStep = 2;
				_TotalIncremention = 0;

				SetSetting(true);
				ShowValue(true);

				if (_MenuScript != null)
				{
					_MenuScript.SelectComponentWithID(MenuID);
				}
			}
		}
		else
		{
			_ControllerInput = false;
		}
	}
	public void SetDefaultValue(float Value)
	{
		if (Value >= 0 && Value <= 1)
		{
			_Value = (int)(Value * 100);
			
			SetSetting(false);
			ShowValue(false);
		}
	}
	void ShowValue(bool MouseInput)
	{
		_TxtValue.text = _Value.ToString();

		if (!MouseInput)
		{
			_Slider.value = _Value * 0.01f;
		}
	}
	void SetSetting(bool Save)
	{
		switch (ComponentID)
		{
		case 75: //music slider
			MusicScript.instance.MusicVolume = _Value;

			if (_Value == 0)
			{
				MusicScript.instance.MusicMuted = true;
			}
			else
			{
				MusicScript.instance.MusicMuted = false;
			}

			MusicScript.instance.ToggleMusic();
			MusicScript.instance.SetMusicVolume(Save);
			break;
		case 76: //sounds slider
			MusicScript.instance.SoundsVolume = _Value;

			if (_Value == 0)
			{
				MusicScript.instance.SoundsMuted = true;
			}
			else
			{
				MusicScript.instance.SoundsMuted = false;
			}

			MusicScript.instance.ToggleSounds();
			MusicScript.instance.SetSoundsVolume(Save);
			break;
		case 77:
			MusicScript.instance.MasterVolume = _Value;

			if (_Value ==	0)
			{
				MusicScript.instance.MusicMuted = true;
				MusicScript.instance.SoundsMuted = true;
			}
			else
			{
				MusicScript.instance.MusicMuted = false;
				MusicScript.instance.SoundsMuted = false;
			}

			MusicScript.instance.SetMusicVolume(Save);
			MusicScript.instance.SetSoundsVolume(Save);
			MusicScript.instance.ToggleMusic();
			MusicScript.instance.ToggleSounds();
			break;
		}
	}
}