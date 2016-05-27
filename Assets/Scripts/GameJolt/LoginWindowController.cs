using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LoginWindowController : MonoBehaviour
{
    [SerializeField] private InputField inpUsername;
    [SerializeField] private InputField inpPassword;
    [SerializeField] private Text txtLoginError;

    private MenuMain menuMain;

	// Use this for initialization
	void Start ()
    {
        menuMain = GameObject.FindObjectOfType<MenuMain>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void ClickLogin()
    {
        Debug.Log("ClickLogin");
        GameJolt.API.Objects.User user = new GameJolt.API.Objects.User(inpUsername.text, inpPassword.text);
        user.SignIn((bool success) =>
        {
            if (success)
            {
                txtLoginError.gameObject.SetActive(false);
                GameJoltManager.instance.loggedIn = true;
                Debug.Log("Success!");
                menuMain.ShowMenu(0);
                //show next menu
            }
            else
            {
                txtLoginError.gameObject.SetActive(true);
                Debug.Log("The user failed to signed in :(");
            }
        });
    }
}