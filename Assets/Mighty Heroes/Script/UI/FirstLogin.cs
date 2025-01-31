using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FirstLogin : MonoBehaviour
{
    [SerializeField]
    private Button ConfirmBtn;

    [SerializeField]
    private TMP_InputField NickNameInput;

    private void Awake()
    {
        ConfirmBtn.onClick.AddListener(() =>
        {
            if(NickNameInput.text == "")
            {
                return;
            }

            JObject Message = new JObject();
            Message.Add("NickName", NickNameInput.text);
            SocketManager.Instance.EmitMessage(SocketEventName.EVENT_MODIFY_NICKNAME, Message.ToString());
            ConfirmBtn.interactable = false;
            NickNameInput.interactable = false;
        });
    }

    private void Start()
    {
        if(GameManager.Instance.ThisUserInfo.IsFirstLogin)
        {
            GameManager.Instance.OnNickNameModifiedEvent += OnFirstNickNameCreated;
        }
    }

    void OnFirstNickNameCreated(string NickName)
    {
        GameManager.Instance.OnNickNameModifiedEvent -= OnFirstNickNameCreated;
        gameObject.SetActive(false);

        GameManager.Instance.ThisUserInfo.IsFirstLogin = false;
        GameManager.Instance.ChangeState(GameState.InDefaultRoom);

        UIManager.AddDebugMessage($"FirstLogin.OnFirstNickNameCreated: {NickName}");
    }
}
