using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Translate : MonoBehaviour
{
	private Text _Txt;
	[SerializeField] private string _Key;

	// Use this for initialization
	void Start ()
	{
		_Txt = GetComponent<Text> ();

		Language.instance.AddTranslateField (this);

		UpdateLanguage ();
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

	public void UpdateLanguage()
	{
		_Txt.text = Language.instance.GetTextWithKey (_Key);
	}
}
