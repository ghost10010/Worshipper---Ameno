using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextScrollerController : MonoBehaviour
{
	[SerializeField] private Sprite[] _TipImages;
	[SerializeField] private string[] _TipText;

	[SerializeField] private Image _ImgTip;
	[SerializeField] private Text _TxtTip;

	[SerializeField] private Text _TxtCount;
	[SerializeField] private Text _TxtTotal;

	[SerializeField] private bool _RandomStart = true;

	private float _ScrollerTimer = 0.0f;
	[SerializeField] private float _ScrollerTime = 2.5f;

	private int CurrentTip = 0;

	// Use this for initialization
	void Start ()
	{
		if (_RandomStart)
		{
			CurrentTip = Random.Range (0, (_TipImages.Length - 1));
		}
		else
		{
			CurrentTip = 0;
		}

		ShowTip (CurrentTip);
	}
	
	// Update is called once per frame
	void Update ()
	{
		_ScrollerTimer += Time.deltaTime;

		if (_ScrollerTimer > _ScrollerTime)
		{
			CurrentTip++;

			if (CurrentTip > (_TipText.Length - 1))
			{
				CurrentTip = 0;
			}

			ShowTip(CurrentTip);

			_ScrollerTimer = 0.0f;
		}
	}

	void ShowTip(int ID)
	{
        if (_TipImages[ID] != null)
        {
            _ImgTip.sprite = _TipImages[ID];
			_ImgTip.color = new Color(1,1,1,1);
        }
		else
		{
			_ImgTip.sprite = null;
			_ImgTip.color = new Color(1,1,1,0);
		}

		_TxtTip.text = Language.instance.GetTextWithKey(_TipText[ID]);

		ShowScrollerCount ();
	}

	void ShowScrollerCount()
	{
		if (_TxtCount != null && _TxtTotal != null)
		{
			_TxtCount.text = (CurrentTip + 1).ToString();
			_TxtTotal.text = _TipImages.Length.ToString();
		}
	}
}
