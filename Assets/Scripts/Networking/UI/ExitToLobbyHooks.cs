using System;
using UnityEngine;
using UnityEngine.UI;

public class ExitToLobbyHooks : MonoBehaviour
{
	public delegate void CanvasHook();

	public CanvasHook OnExitHook;

	public Button firstButton;

	public void UIExit()
	{
        Debug.Log("UIExitClick");
		if (OnExitHook != null)
        {
            Debug.Log("Hook");
            OnExitHook.Invoke();
        }
        else
        {
            Debug.Log("NoHook");
        }
			
	}
}
