using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class QualityManager : MonoBehaviour
{
	public static QualityManager instance;

	public OptionListController OptionList;

	[SerializeField] private string[] _ExcludedQualitys;

	// Use this for initialization
	void Start ()
	{
		int DefaultValue = 0;

		string[] Qualities = QualitySettings.names;

		for (int index = 0; index < Qualities.Length; index++)
		{
			if (!QualityExcluded(Qualities[index]))
			{
				OptionList.AddOption(Qualities[index]);
			}
		}

		DefaultValue = GetCurrentQualityOption ();

		if (DefaultValue != -1)
		{
			OptionList.SetDefaultValue(DefaultValue);
		}
	}

	void Awake ()
	{
		instance = this;
	}

	bool QualityExcluded(string Qua)
	{
		for (int index = 0; index < _ExcludedQualitys.Length; index++)
		{
			if (Qua == _ExcludedQualitys[index])
			{
				return true;
			}
		}

		return false;
	}

	public void ChangeQuality(int ID)
	{
		Debug.Log (ID);
		QualitySettings.SetQualityLevel (ID, true);
	}

	public int GetCurrentQualityOption()
	{
		int CurrentOption = QualitySettings.GetQualityLevel();

		/*for (int index = 0; index < OptionList.Options.Count; index++)
		{
			if (OptionList.Options[index] == (Screen.currentResolution.width + "x" + Screen.currentResolution.height))
			{
				CurrentOption = index;
				break;
			}
		}*/

		return CurrentOption;
	}
}