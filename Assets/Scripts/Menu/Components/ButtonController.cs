//Victor Adamse
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ButtonController : ComponentBehaviour
{
    public MenuController MenuScript; //the menu main script

	[SerializeField] private Image _Highlight;

	[SerializeField] private int _NextMenu = -1;
    [SerializeField] private int _NextScene = -1;

    [SerializeField] private GameObject _LeftButton, _RightButton;

    void Start()
    {
        if (MenuScript == null)
        {
            MenuScript = GetComponentInParent<MenuController>();
        }
    }

	public void OnMouseEnter() //PointerEnter
	{
        if (isEnabled)
        {
            MenuScript.SelectComponentWithID(MenuID);
        }
        
	}

	public void OnMouseExit() //PointerExit
	{
        if (isEnabled)
        {

        }
	}
	
	public void OnMouseDown() //PointerClick
	{
        if (isEnabled)
        {
            MenuScript.PressButton(ComponentID);
        }
	}

    public void PointerClick()
    {
        if (isEnabled)
        {
            MenuScript.PressButton(ComponentID);
        }
    }

    public void ClickLoad()
    {
        Settings.instance.LoadFromSave = !Settings.instance.LoadFromSave;

        if (Settings.instance.LoadFromSave)
        {
            base.GetText().text = Language.instance.GetTextWithKey("menu.btn.Cancel");
        }
        else
        {
            base.GetText().text = Language.instance.GetTextWithKey("menu.btn.Load");
        }
    }

	public override void SelectComponent ()
	{
        if (isEnabled)
        {
            if (_Highlight != null)
            {
                _Highlight.color = new Color(1, 1, 1, 1);
            }

            base.SelectComponent();
        }
	}
	public override void DeselectComponent ()
	{
        if (isEnabled)
        {
            if (_Highlight != null)
            {
                _Highlight.color = new Color(1, 1, 1, 0);
            }

            base.DeselectComponent();
        }
	}

    public override void InputRight()
    {
        if (isEnabled)
        {
            if (_RightButton != null)
            {
                MenuScript.SelectComponentWithID(_RightButton.GetComponent<ComponentBehaviour>().MenuID);
            }
        }
    }

    public override void InputLeft()
    {
        if (isEnabled)
        {
            if (_LeftButton != null)
            {
                MenuScript.SelectComponentWithID(_LeftButton.GetComponent<ComponentBehaviour>().MenuID);
            }
        }
    }

	public override void InputClick ()
	{
        if (isEnabled)
        {
            MenuScript.PressButton(ComponentID);
        }
	}

    public void OnClick()
    {
        MenuScript.GetMainMenu().ShowMenu(0);
    }

	public int GetNextMenu()
	{
		return _NextMenu;
	}
    public int GetNextScene()
    {
        return _NextScene;
    }
}