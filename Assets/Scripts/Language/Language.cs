using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;

public class Language : MonoBehaviour
{
	public static Language instance;

	[SerializeField] private List<Translate> _TextFields;
	[SerializeField] private string CurrentLanguage;
	[SerializeField] private string DefaultLanguage = "en";

	private Hashtable TextStrings = new Hashtable();

	public MenuMain _MenuScript;

	// Use this for initialization
	void Start ()
	{
		_MenuScript = GameObject.Find ("Canvas").GetComponent<MenuMain> ();

		string SavedLanguage = PlayerPrefs.GetString ("Language", "");

		if (SavedLanguage != "")
		{
			SetLanguage(SavedLanguage);
		}
		else
		{
			SetLanguage (DefaultLanguage);
		}
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

	public void UpdateTextfields()
	{
		for (int index = 0; index < _TextFields.Count; index++)
		{
			_TextFields[index].UpdateLanguage();
		}
	}

	public void SetLanguage(string Language)
	{
		TextStrings.Clear ();

		CurrentLanguage = Language;

		string BasePath = "Assets/Resources/Language/";
        string ResourcesPath = "Language/";
		string PathText = BasePath + CurrentLanguage + "_Text.xml";
        
		if (System.IO.File.Exists(BasePath + CurrentLanguage + "_Text.xml"))
		{
			PathText = ResourcesPath + CurrentLanguage + "_Text";
		}
        else
        {
            PathText = ResourcesPath + DefaultLanguage + "_Text";
        }

		ReadXml (PathText, TextStrings);

		UpdateTextfields ();
		_MenuScript.UpdateLanguage ();
		PlayerPrefs.SetString("Language", Language);
	}
	void ReadXml(string Path, Hashtable Table)
	{
        TextAsset xmlFile = (TextAsset)Resources.Load(Path);

        XmlDocument xml = new XmlDocument();
        xml.LoadXml(xmlFile.text);

		XmlElement BaseNode = xml.DocumentElement;

		for (int index = 0; index < BaseNode.ChildNodes.Count; index++)
		{
			if (BaseNode.ChildNodes[index].Name != "#comment")
			{
				Table.Add(BaseNode.ChildNodes[index].Name, BaseNode.ChildNodes[index].InnerText);
			}
		}
	}

    public string GetLanguage()
	{
		return CurrentLanguage;
	}

    public string GetTextWithKey(string Key)
	{
		string Text = "";

		if (TextStrings.ContainsKey(Key) && Key != "")
		{
			Text = TextStrings[Key].ToString();
		}
        else
        {
            Debug.Log("Key not found: " + Key);
        }

		return Text;
	}

	public void AddTranslateField(Translate Field)
	{
		_TextFields.Add (Field);
	}
}