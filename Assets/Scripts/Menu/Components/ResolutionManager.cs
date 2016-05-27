using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{
	public static ResolutionManager instance;

	public OptionListController OptionList;


	[SerializeField] private string[] _ExcludedResolutions;
	[SerializeField] private Image _ImgFadeoutBackground;
	
	private float _FadeTimer = 0.0f;
	private float _FadeTime = 1.0f;
	private bool _FadeOut = false;
	private bool _FadeIn = false;

	private MenuController _MenuScript;

	// Use this for initialization
	void Start ()
	{
		int DefaultValue = 0;

		_MenuScript = GetComponentInParent<MenuController>();

		Resolution[] resolutions = Screen.resolutions;

		//test resolutions
		#if UNITY_EDITOR
		OptionList.AddOption("1024x768");
		OptionList.AddOption("1366x768");
		OptionList.AddOption("1920x1080");
		#endif

		for (int index = 0; index < resolutions.Length; index++)
		{
			if (!ResolutionExcluded(resolutions[index].width + "x" + resolutions[index].height))
			{
				OptionList.AddOption(resolutions[index].width + "x" + resolutions[index].height);
			}
		}

		DefaultValue = GetCurrentResolutionOption ();

		if (DefaultValue != -1)
		{
			OptionList.SetDefaultValue(DefaultValue);
		}
	}

	void Update()
	{
		if (_FadeOut)
		{
			_FadeTimer += GetTime();
			_ImgFadeoutBackground.color = new Color(0,0,0, _FadeTimer);
			
			if (_FadeTimer > 1)
			{
				_FadeOut = false;
				_FadeIn = true;
				SetResolution(OptionList.GetSelectedOption());
			}
		}
		if (_FadeIn)
		{
			_FadeTimer -= GetTime();
			_ImgFadeoutBackground.color = new Color(0,0,0, _FadeTimer);
			
			if (_FadeTimer < 0)
			{
				_FadeIn = false;
				_FadeTimer = 0.0f;
				_ImgFadeoutBackground.gameObject.SetActive(false);
				_ImgFadeoutBackground.color = new Color(0,0,0,0);
			}
		}
	}

	void Awake ()
	{
		instance = this;
	}

	float GetTime()
	{
		float TimePassed = 0.0f;

		if (_MenuScript.PauseMenu)
		{
			TimePassed = Time.fixedDeltaTime;
		}
		else
		{
			TimePassed = Time.deltaTime;
		}

		return TimePassed;
	}

	bool ResolutionExcluded(string Res)
	{
		for (int index = 0; index < _ExcludedResolutions.Length; index++)
		{
			if (Res == _ExcludedResolutions[index])
			{
				return true;
			}
		}

		return false;
	}

	public void ChangeResolution(string NewResolution)
	{
		_ImgFadeoutBackground.gameObject.SetActive(true);
		_FadeOut = true;
	}

	void SetResolution(string NewResolution)
	{
		int NewWidth = -1;
		int NewHeight = -1;
		
		string[] ResolutionSplit = NewResolution.Split ('x');
		
		int.TryParse (ResolutionSplit [0].ToString(), out NewWidth);
		int.TryParse (ResolutionSplit [1], out NewHeight);
		
		Screen.SetResolution (NewWidth, NewHeight, true);
	}

	public int GetCurrentResolutionOption()
	{
		int CurrentOption = -1;

		for (int index = 0; index < OptionList.Options.Count; index++)
		{
			if (OptionList.Options[index] == (Screen.currentResolution.width + "x" + Screen.currentResolution.height))
			{
				CurrentOption = index;
				break;
			}
		}

		return CurrentOption;
	}
}