using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ComponentBehaviour : MonoBehaviour
{
	public int ComponentID;
	[HideInInspector] public int MenuID;

	public bool IsAvailable = true;
    public bool isEnabled = true;

	public ComponentType ComponentType;

	[SerializeField] private Image _BackgroundUnselected;
	[SerializeField] private Image _BackgroundSelected;

	[SerializeField] private Text _TxtTitle;
	[SerializeField] private string _Key;

    [SerializeField] private bool AvailableOnAndroid;
    [SerializeField] private bool AvailableOnPc;

    void Start()
	{
		GetBackgrounds ();
		GetTitle ();

		UpdateLanguage ();
	}

	void Awake()
	{

	}

    public void Disable()
    {
        isEnabled = false;

        Color unSelected = _BackgroundUnselected.color;
        unSelected.a = 0.5f;
        Color selected = _BackgroundSelected.color;
        selected.a = 0.0f;

        _BackgroundUnselected.color = unSelected;
        _BackgroundSelected.color = selected;
    }
    public void Enable()
    {
        isEnabled = true;

        Color unSelected = _BackgroundUnselected.color;
        unSelected.a = 0.0f;
        Color selected = _BackgroundSelected.color;
        selected.a = 1.0f;

        _BackgroundUnselected.color = unSelected;
        _BackgroundSelected.color = selected;
    }

    public void SetKey(string Key)
    {
        _Key = Key;
    }

	public virtual void SelectComponent()
	{
		if (_BackgroundSelected == null && _BackgroundUnselected == null)
		{
			GetBackgrounds();
		}
        
        Color unSelected = _BackgroundUnselected.color;
        unSelected.a = 0.0f;
        Color selected = _BackgroundSelected.color;
        selected.a = 1.0f;

        _BackgroundUnselected.color = unSelected;
        _BackgroundSelected.color = selected;

        if (!MusicScript.instance.SoundsMuted)
		{
			GetComponent<AudioSource>().Play();
		}
	}
	public virtual void DeselectComponent()
	{
        Color unSelected = _BackgroundUnselected.color;
        unSelected.a = 1.0f;
        Color selected = _BackgroundSelected.color;
        selected.a = 0.0f;

        _BackgroundUnselected.color = unSelected;
        _BackgroundSelected.color = selected;

        if (!MusicScript.instance.SoundsMuted)
		{
			GetComponent<AudioSource>().Play();
		}
	}

	void GetBackgrounds()
	{
		for (int index = 0; index < this.transform.childCount; index++)
		{
			if (this.transform.GetChild(index).name.Contains("Unselected"))
			{
				_BackgroundUnselected = this.transform.GetChild(index).GetComponent<Image>();
			}
			else if (this.transform.GetChild(index).name.Contains("Selected"))
			{
				_BackgroundSelected = this.transform.GetChild(index).GetComponent<Image>();
			}
		}
	}
	void GetTitle()
	{
		if (_TxtTitle == null)
		{
			_TxtTitle = GetComponent<Text> ();
		}
	}
    public Text GetText()
    {
        return _TxtTitle;
    }

	//ShowBackground
	//HideBackground

	public virtual void InputRight()
	{

	}
	public virtual void InputLeft()
	{

	}
	public virtual void InputUp()
	{

	}
	public virtual void InputDown()
	{

	}
	public virtual void InputClick()
	{

	}

	//SelectComponent
	//SelectComponentMouse
	//SelectComponentWithID
	//SetDefaultValue
	
	public virtual void SelectComponent1()
	{

	}
	public virtual void SelectComponentMouse()
	{

	}
	public virtual void SelectComponentWithID()
	{

	}
	public virtual void SetDefaultValue()
	{

	}

	public virtual void UpdateLanguage()
	{
		if (_Key.Trim() != "")
		{
            _TxtTitle.text = Language.instance.GetTextWithKey (_Key);
		}
	}

	public void SetMenuID(int ID)
	{
		MenuID = ID;
	}
}