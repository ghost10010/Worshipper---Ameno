using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SelectorValueController : ComponentBehaviour
{
	public int ValueID;
	private SelectorController _SelectorScript;
	
	[SerializeField] private bool _IsDefaultValue;
	[SerializeField] private Image _Highlight; 

	public bool IsSelected = false;

	void Start()
	{
		_SelectorScript = GetComponentInParent<SelectorController> ();

		if(_IsDefaultValue)
		{
			IsSelected = true;

			SelectComponent();
			_SelectorScript.SetDefaultValue(ValueID);
		}
	}

	public override void SelectComponent ()
	{
		if (_Highlight != null)
		{
			_Highlight.color = new Color(1,1,1,1);
		}

		base.SelectComponent();
	}
	public override void DeselectComponent ()
	{
		if (_Highlight != null)
		{
			_Highlight.color = new Color(1,1,1,0);
		}

		base.DeselectComponent();
	}

	public void OnMouseEnter() 
	{
		_SelectorScript.SelectSelector ();
		base.SelectComponent ();
	}
	
	public void OnMouseExit()
	{
		if (!IsSelected)
		{
			base.DeselectComponent ();
		}
	}
	
	public void OnMouseDown()
	{
		_SelectorScript.SetValue (ValueID);
	}

	public void SetValueID(int ID)
	{
		ValueID = ID;
	}
}