using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
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

    private bool PasswordVisible = false;

    // Start is called before the first frame update
    void Start()
    {
        LoginBtn.onClick.AddListener(OnLoginBtnCliecked);
        RegisterBtn.onClick.AddListener(OnRegisterBtnClicked);
        PassworldVisibleToggleBtn.onClick.AddListener(OnPasswordVisibleToggleClicked);
    }

    private void OnDestroy()
    {
        LoginBtn.onClick.RemoveAllListeners();
        RegisterBtn.onClick.RemoveAllListeners();
        PassworldVisibleToggleBtn.onClick.RemoveAllListeners();
    }

    void OnLoginBtnCliecked()
    {
        string username = UsernameInput.text;
        string password = PasswordInput.text;
        
        if(username == "" || password == "")
        {
            //Show notice
            return;
        }

        JObject Message = new JObject();
        Message.Add("username", username);
        Message.Add("password", password);

        SocketManager.Instance.EmitMessage(EventName.EVENT_LOGIN, Message.ToString());
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
}
