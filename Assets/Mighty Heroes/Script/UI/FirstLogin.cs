using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class FirstLogin : BaseUI
{
    [SerializeField]
    private Button ConfirmBtn;

    [SerializeField]
    private TMP_InputField NickNameInput;

    public override void Init()
    {
        base.Init();

        ConfirmBtn.onClick.AddListener(() =>
        {
            if (NickNameInput.text == "")
            {
                return;
            }

            WWWForm Form = new WWWForm();
            Form.AddField(Constant.NICK_NAME, NickNameInput.text);
            Form.AddField(Constant.PLAYER_USERNAME, GameManager.Instance.ThisUserInfo.UserName);

            StartCoroutine(WebServiceAPI.PostRequest($"{URL.SERVICES_URL}/NameChange.php", Form, OnNameChangeCallback));

            ConfirmBtn.interactable = false;
            NickNameInput.interactable = false;
        });

        Login.OnLoginSuccessEvent += OnLoginSuccess;
        GameManager.OnNickNameModifiedEvent += OnFirstNickNameCreated;
    }

    public override void Deinit()
    {
        base.Deinit();
        ConfirmBtn.onClick.RemoveAllListeners();
    }

    void OnLoginSuccess(UserInfo info)
    {
        if (info.IsFirstLogin)
        {
            gameObject.SetActive(true);
            GameManager.OnNickNameModifiedEvent += OnFirstNickNameCreated;
        }
    }

    void OnNameChangeCallback(string response)
    {
        JObject data = JObject.Parse(response);
        string status = data[Constant.RESPONSE_STATUS]?.Value<string>();
        if (status == "SUCCESS")
        {
            string NewNickName = data[Constant.NICK_NAME]?.Value<string>();
            GameManager.OnNickNameModifiedEvent?.Invoke(NewNickName);
        }
        else
        {
            UIManager.AddDebugMessage(data[Constant.FAILED_REASON]?.Value<string>(), LogVerbose.Error);
        }
    }

    void OnFirstNickNameCreated(string NickName)
    {
        GameManager.OnNickNameModifiedEvent -= OnFirstNickNameCreated;
        Login.OnLoginSuccessEvent -= OnLoginSuccess;

        gameObject.SetActive(false);

        GameManager.Instance.ThisUserInfo.IsFirstLogin = false;

        UIManager.AddDebugMessage($"FirstLogin.OnFirstNickNameCreated: {NickName}");
    }
}
