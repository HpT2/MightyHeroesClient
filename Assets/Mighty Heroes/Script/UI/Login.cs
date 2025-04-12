using Newtonsoft.Json.Linq;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Login : BaseUI
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

    public override void Init()
    {
        base.Init();

        LoginBtn.onClick.AddListener(OnLoginBtnCliecked);
        RegisterBtn.onClick.AddListener(OnRegisterBtnClicked);
        PassworldVisibleToggleBtn.onClick.AddListener(OnPasswordVisibleToggleClicked);
    }

    public override void Deinit()
    {
        base.Deinit();

        LoginBtn.onClick.RemoveAllListeners();
        RegisterBtn.onClick.RemoveAllListeners();
        PassworldVisibleToggleBtn.onClick.RemoveAllListeners();
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

        WWWForm Form = new WWWForm();
        Form.AddField(Constant.PLAYER_USERNAME, username);
        Form.AddField(Constant.PLAYER_PASSWORD, password);

        StartCoroutine(WebServiceAPI.PostRequest($"{URL.AUTHENTICATION_URL}/Login.php", Form, OnLoginCallback));
    }

    void OnRegisterBtnClicked()
    {
        //Popup register UI
        UIManager.AddDebugMessage("Login.OnRegisterBtnClicked");
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

    void OnLoginCallback(string response)
    {
        UsernameInput.interactable = true;
        PasswordInput.interactable = true;
        LoginBtn.interactable = true;
        RegisterBtn.interactable = true;

        JObject data = JObject.Parse(response);
        string status = data[Constant.RESPONSE_STATUS]?.Value<string>();
        if(status == "SUCCESS")
        {
            JObject UserData = data[Constant.USER_DATA]?.Value<JObject>();
            OnLoginSuccess(UserData);
        }
        else
        {
            OnLoginFailure(data[Constant.FAILED_REASON]?.Value<string>());
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
        LoginErrorText.text = reason;
        LoginErrorText.gameObject.SetActive(true);
    }
}

public delegate void OnLoginSuccess_Delegate(UserInfo info);