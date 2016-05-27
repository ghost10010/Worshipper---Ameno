using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectorController : ComponentBehaviour
{
	public GameObject[] Values;	//All Values

	private int _LastSelectedValue = 0;
	private int _SelectedValue = 0;

	[SerializeField] private MenuController _MenuScript;

	void Awake()
	{
		_MenuScript = GetComponentInParent<MenuController> ();

		for (int index = 0; index < Values.Length; index++)
		{
			Values[index].GetComponent<ComponentBehaviour>().SetMenuID(index);
			Values[index].GetComponent<SelectorValueController>().SetValueID(index);
		}
	}

	// Update is called once per frame
	void Update ()
	{

	}

	public override void InputLeft ()
	{
		SelectValue (1);
	}

	public override void InputRight ()
	{
		SelectValue (-1);
	}

	void SelectValue(int NextID)
	{
		_LastSelectedValue = _SelectedValue;
		_SelectedValue -= NextID;

        if (_SelectedValue < 0)
        {
            _SelectedValue = 0;
        }
        else if (_SelectedValue >= Values.Length)
        {
            _SelectedValue = (Values.Length - 1);
        }
        else
        {
            SetSettings(_SelectedValue);

            Values[_SelectedValue].GetComponent<ComponentBehaviour>().SelectComponent();
            Values[_LastSelectedValue].GetComponent<ComponentBehaviour>().DeselectComponent();
         }
	}

    public void SelectSelector()
	{
		_MenuScript.SelectComponentWithID(MenuID);
	}
	public void SetValue(int ValueID)
	{
		if (ValueID != _SelectedValue)
		{
			_LastSelectedValue = _SelectedValue;
			_SelectedValue = ValueID;
			SetSettings(_SelectedValue);

			Values[_SelectedValue].GetComponent<SelectorValueController>().SelectComponent();
			Values[_SelectedValue].GetComponent<SelectorValueController>().IsSelected = true;
			Values[_LastSelectedValue].GetComponent<SelectorValueController>().DeselectComponent();
			Values[_LastSelectedValue].GetComponent<SelectorValueController>().IsSelected = false;
		}
	}
	void SetSettings(int ValueID)
	{
		bool Setting = false; //default value No
		if (ValueID == 0) //if value is Yes
		{
			Setting = true;
		}
		
		switch(ComponentID)
		{
		case 53: //music
			MusicScript.instance.MusicMuted = !Setting;
			MusicScript.instance.ToggleMusic();
			break;
		case 54: //Sound effects
			MusicScript.instance.SoundsMuted = !Setting;
			MusicScript.instance.ToggleSounds();
			break;
		}
	}
	public void SetDefaultValue(int ValueID)
	{
		_SelectedValue = ValueID;
		_LastSelectedValue = ValueID;

		SetSettings (_SelectedValue);
	}
}