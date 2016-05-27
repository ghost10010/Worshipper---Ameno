using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Dialog : ComponentBehaviour
{
	public DialogType DialogType;

	public string Message;

	public int ActionID = -1;
	public int LastSelectedButton = 0;
	public int SelectedButton = 0;

	[SerializeField] private Image _ImgBackground;
	[SerializeField] private Text _TxtMessage;

	[SerializeField] private Sprite[] _Backgrounds;

	[SerializeField] private GameObject _DialogPanel;
	[SerializeField] private GameObject _BtnClose;

	public List<GameObject> Buttons;

	private bool _DialogOpen = false;

	// Use this for initialization
	void Start ()
	{
		_DialogPanel.SetActive (false);
	}
	void Awake()
	{
		for (int index = 0; index < Buttons.Count; index++)
		{
			DeselectButton(index);
		}

		SelectButton (SelectedButton);
	}

	public void OnMouseEnter(int ButtonID)
	{
		LastSelectedButton = SelectedButton;
		SelectedButton = ButtonID;

		DeselectButton (LastSelectedButton);
		SelectButton (SelectedButton);
	}
	public void OnMouseExit(int ButtonID)
	{

	}
	public void OnMouseDown(int ButtonID)
	{
		switch(ButtonID)
		{
		case 10: //Close
			break;
		case 0: //Oke
			ConfirmAction();
			break;
		case 1: //Cancel
			break;
		}

		HideDialog ();
	}

	public override void InputClick ()
	{
		if (DialogType == DialogType.Error)
		{
			OnMouseDown(10);
		}
		else
		{
			OnMouseDown (SelectedButton);
		}
	}
	public override void InputLeft ()
	{
		LastSelectedButton = SelectedButton;

		if (DialogType == DialogType.Message)
		{
			SelectedButton--;

			if (SelectedButton < 0)
			{
				SelectedButton = 0;
			}
		}

		DeselectButton (LastSelectedButton);
		SelectButton (SelectedButton);
	}
	public override void InputRight ()
	{
		LastSelectedButton = SelectedButton;

		if (DialogType == DialogType.Message)
		{
			SelectedButton++;
			
			if (SelectedButton > 1)
			{
				SelectedButton = 1;
			}
		}

		DeselectButton (LastSelectedButton);
		SelectButton(SelectedButton);
	}

	void SelectButton(int ID)
	{
		if (DialogType == DialogType.Error)
		{
			_BtnClose.GetComponent<ComponentBehaviour>().SelectComponent();
		}
		else
		{
			Buttons [ID].GetComponent<ComponentBehaviour> ().SelectComponent ();
		}
	}
	void DeselectButton(int ID)
	{
		if (DialogType == DialogType.Error)
		{
			_BtnClose.GetComponent<ComponentBehaviour>().DeselectComponent();
		}
		else
		{
			Buttons [ID].GetComponent<ComponentBehaviour> ().DeselectComponent ();
		}
	}
	void ConfirmAction()
	{
		switch(ActionID)
		{
		case 1:
			break;
		}
	}

	public void SetupDialog(DialogType Type, string Message, int ActionID)
	{
		this.DialogType = Type;
		this.Message = Message;
		this.ActionID = ActionID;
		
		_ImgBackground.sprite = _Backgrounds [(int)this.DialogType];
		_TxtMessage.text = this.Message;

		for (int index = 0; index < Buttons.Count; index++)
		{
			DeselectButton(index);
		}

		if (DialogType == DialogType.Error)
		{
			SelectedButton = 0;
			LastSelectedButton = 0;
		}
		else
		{
			SelectedButton = 1;
			LastSelectedButton = 1;
		}

		SetButtons ();
		SelectButton (SelectedButton);
	}
	void SetButtons()
	{
		if (this.DialogType == DialogType.Error)
		{
			_BtnClose.SetActive(true);
			Buttons[0].SetActive(false);
			Buttons[1].SetActive(false);
		}
		else
		{
			_BtnClose.SetActive(false);
			Buttons[0].SetActive(true);
			Buttons[1].SetActive(true);
		}

		_BtnClose.GetComponent<ComponentBehaviour> ().UpdateLanguage ();
		Buttons[0].GetComponent<ComponentBehaviour> ().UpdateLanguage ();
		Buttons[1].GetComponent<ComponentBehaviour> ().UpdateLanguage ();
	}

	public void ShowDialog()
	{
		_DialogOpen = true;
		_DialogPanel.SetActive (true);
	}
	public void HideDialog()
	{
		_DialogOpen = false;
		_DialogPanel.SetActive (false);
	}

	public bool IsDialogActive()
	{
		return _DialogOpen;
	}
}