using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LogoScript : MonoBehaviour
{
    private float    _Timer = 8;

	private float    _WaitTimer = 2;
	private float 	 _WaitTime = 2;
	private float    _FadeTimer = 3;
	private float    _FadeTime = 3;

	private float _BackgroundValue = 0.0f;

	private int _LogoCount = 0;

	[SerializeField] private Image[] _Logos;
	[SerializeField] private Image _FadeBackground;

	private bool PlayLogos = true;
    
	void Start()
	{
		Screen.SetResolution (Screen.currentResolution.width, Screen.currentResolution.height, true);
	}

	void Update()
    {
		_Timer -= Time.deltaTime;

		_WaitTimer -= Time.deltaTime;

		if (_WaitTimer < 0)
		{
			_FadeTimer -= Time.deltaTime;

			if (PlayLogos)
			{
				_Logos[_LogoCount].color = new Color(1,1,1, _FadeTimer * 0.5f);

				if ((_LogoCount + 1) != _Logos.Length && _FadeTimer < 1.0)
				{
					_Logos[_LogoCount + 1].color = new Color(1,1,1,1);
				}
			}
			else
			{
				_BackgroundValue += Time.deltaTime;
				_FadeBackground.color = new Color(0,0,0, _BackgroundValue);

				if (_BackgroundValue > 1)
				{
					Application.LoadLevel(1);
				}
			}

			if (_FadeTimer < 0)
			{
				if (PlayLogos)
				{
					_LogoCount++;
					_WaitTimer = _WaitTime;
					_FadeTimer = _FadeTime;

					if (_LogoCount == _Logos.Length)
					{
						Application.LoadLevel(1);
					}
				}
			}
		}

		if (Input.anyKeyDown)
		{
			_Logos[_LogoCount].color = new Color(1,1,1,0);

			PlayLogos = false;
			_BackgroundValue = 0.0f;
			_WaitTimer = -1.0f;
			_FadeTimer = _FadeTime;
		}
    }
}