using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using TMPro;
using UnityEngine.Events;

public class LoginRegister : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public TextMeshProUGUI displayText;
    public UnityEvent onLoggedIn;

    private Color green = new Color(138/255f, 229/255f, 131/255f);
    private Color red = new Color(165/255f, 16/255f, 16/255f);

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
                SetDisplayText(result.PlayFabId, green);
            },
            error =>
            {
                SetDisplayText(error.ErrorMessage, red);
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
                Debug.Log("Logged in as: " + result.PlayFabId);

                playFabId = result.PlayFabId;

                if(onLoggedIn != null)
                        onLoggedIn.Invoke();
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
