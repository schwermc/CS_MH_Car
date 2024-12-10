using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.Events;

public class LoginRegister : MonoBehaviour
{
    public GameObject rooms;

    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public TextMeshProUGUI displayText;
    public UnityEvent onLoggedIn;

    //private Color gold = new Color(183/255f, 153/255f, 13/255f);
    //private Color cream = new Color(242/255f, 244/255f, 203/255f);

    [HideInInspector]
    public string playFabId;

    public static LoginRegister instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void OnRegister()
    {
        RegisterPlayFabUserRequest registerRequest = new RegisterPlayFabUserRequest
        {
            Username = usernameInput.text,
            DisplayName = usernameInput.text,
            Password = passwordInput.text,
            RequireBothUsernameAndEmail = false
        };

        PlayFabClientAPI.RegisterPlayFabUser(registerRequest,
            result =>
            {
                SetDisplayText(result.PlayFabId, Color.green);
            },
            error =>
            {
                SetDisplayText(error.ErrorMessage, Color.red);
            }
            );
    }

    public void OnLoginButton()
    {
        LoginWithPlayFabRequest loginRequest = new LoginWithPlayFabRequest
        {
            Username = usernameInput.text,
            Password = passwordInput.text
        };

        PlayFabClientAPI.LoginWithPlayFab(loginRequest,
            result =>
            {
                playFabId = result.PlayFabId;

                if(onLoggedIn != null)
                        onLoggedIn.Invoke();
                
                rooms.SetActive(true);
            },
            error => Debug.Log(error.ErrorMessage)
            );
    }

    void SetDisplayText(string text, Color color)
    {
        displayText.text = text;
        displayText.color = color;
    }
}
