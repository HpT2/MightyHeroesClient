using Newtonsoft.Json.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    [SerializeField]
    private Button LoginBtn;

    [SerializeField]
    private Button RegisterBtn;

    [SerializeField]
    private Button PassworldVisibleToggleBtn;

    [SerializeField]
    private TMP_InputField UsernameInput;

    [SerializeField]
    private TMP_InputField PasswordInput;

    [SerializeField]
    private GameObject EyeOpenImg;

    [SerializeField]
    private GameObject EyeCloseImg;

    [SerializeField]
    private TextMeshProUGUI LoginErrorText;

    private bool PasswordVisible = false;

    public static OnLoginSuccess_Delegate OnLoginSuccessEvent;

    // Start is called before the first frame update
    void Start()
    {
        LoginBtn.onClick.AddListener(OnLoginBtnCliecked);
        RegisterBtn.onClick.AddListener(OnRegisterBtnClicked);
        PassworldVisibleToggleBtn.onClick.AddListener(OnPasswordVisibleToggleClicked);

        SocketEvent.OnLoginResponse += OnLoginResponse;
    }

    private void OnDestroy()
    {
        LoginBtn.onClick.RemoveAllListeners();
        RegisterBtn.onClick.RemoveAllListeners();
        PassworldVisibleToggleBtn.onClick.RemoveAllListeners();

        SocketEvent.OnLoginResponse -= OnLoginResponse;
    }

    void OnLoginBtnCliecked()
    {
        LoginErrorText.gameObject.SetActive(false);

        string username = UsernameInput.text;
        string password = PasswordInput.text;
        
        if(username == "" || password == "")
        {
            //Show notice
            LoginErrorText.text = "Username and Password must not be empty";
            LoginErrorText.gameObject.SetActive(true);
            return;
        }

        //disable button
        UsernameInput.interactable = false;
        PasswordInput.interactable = false;
        LoginBtn.interactable = false;
        RegisterBtn.interactable = false;

        JObject Message = new JObject();
        Message.Add("Username", username);
        Message.Add("Password", password);

        SocketManager.Instance.EmitMessage(SocketEventName.EVENT_LOGIN, Message.ToString());
    }

    void OnRegisterBtnClicked()
    {
        //Popup register UI
    }

    void OnPasswordVisibleToggleClicked()
    {
        PasswordVisible = !PasswordVisible;
        if (PasswordVisible)
        {
            EyeCloseImg.SetActive(false);
            EyeOpenImg.SetActive(true);
            PasswordInput.contentType = TMP_InputField.ContentType.Standard;
        }
        else
        {
            EyeCloseImg.SetActive(true);
            EyeOpenImg.SetActive(false);
            PasswordInput.contentType = TMP_InputField.ContentType.Password;
        }

        PasswordInput.ActivateInputField();
    }

    void OnLoginResponse(JObject data)
    {
        Thread.Sleep(1000);
        UsernameInput.interactable = true;
        PasswordInput.interactable = true;
        LoginBtn.interactable = true;
        RegisterBtn.interactable = true;

        JToken status = data.GetValue("State");
        if(status.Value<string>() == LoginStatus.SUCCESS)
        {
            JToken userData = data.GetValue("UserData");
            OnLoginSuccess(userData.Value<JObject>());
        }
        else
        {
            JToken reason = data.GetValue("Reason");
            OnLoginFailure(reason.Value<string>());
        }
    }

    void OnLoginSuccess(JObject UserData)
    {
        //UIManager.AddDebugMessage(UserData.ToString());
        UserInfo info = UserData.ToObject<UserInfo>();
        UIManager.AddDebugMessage("Login.OnLoginSuccess: " + JObject.FromObject(info));
        gameObject.SetActive(false);
        OnLoginSuccessEvent.Invoke(info);
    }

    void OnLoginFailure(string reason)
    {
        if (reason == LoginFailedConstants.WRONG_PASSWORD)
        {
            LoginErrorText.text = "Login Failed: Wrong password";
        }
        else if (reason == LoginFailedConstants.USERNAME_NOT_FOUND)
        {
            LoginErrorText.text = "Login Failed: Username not found";
        }

        LoginErrorText.gameObject.SetActive(true);
    }
}

class LoginFailedConstants
{
    public static string WRONG_PASSWORD = "wrong password";
    public static string USERNAME_NOT_FOUND = "username not found";
}

class LoginStatus
{
    public static string SUCCESS = "success";
    public static string FAIL = "failed";
}

public delegate void OnLoginSuccess_Delegate(UserInfo info);