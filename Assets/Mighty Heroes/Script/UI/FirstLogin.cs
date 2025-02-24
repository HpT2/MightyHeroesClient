using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

            JObject Message = new JObject();
            Message.Add("NickName", NickNameInput.text);
            SocketManager.Instance.EmitMessage(SocketEventName.EVENT_MODIFY_NICKNAME, Message.ToString());
            ConfirmBtn.interactable = false;
            NickNameInput.interactable = false;
        });

        Login.OnLoginSuccessEvent += OnLoginSuccess;
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
            GameManager.OnNickNameModifiedEvent += OnFirstNickNameCreated;
        }
    }

    void OnFirstNickNameCreated(string NickName)
    {
        GameManager.OnNickNameModifiedEvent -= OnFirstNickNameCreated;
        Login.OnLoginSuccessEvent -= OnLoginSuccess;

        gameObject.SetActive(false);

        GameManager.Instance.ThisUserInfo.IsFirstLogin = false;
        GameManager.Instance.ChangeState(GameState.InDefaultRoom);

        UIManager.AddDebugMessage($"FirstLogin.OnFirstNickNameCreated: {NickName}");
    }
}
