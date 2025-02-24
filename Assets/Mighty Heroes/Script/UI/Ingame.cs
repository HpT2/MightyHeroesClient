using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnJoystickDrag_Delegate(Vector2 Input);
public delegate void OnMainSkillClicked_Delegate();
public delegate void OnSkillClicked_Delegate(int SkillIndex);

public class Ingame : BaseUI
{
    [SerializeField]
    private GameObject FirstLoginUI;

    public OnJoystickDrag_Delegate OnJoystickDrag;
    public OnMainSkillClicked_Delegate OnMainSkillClicked;
    public OnSkillClicked_Delegate OnSkillClicked;

    [SerializeField]
    private Button MainSkillBtn;

    [SerializeField]
    private List<Button> SkillBtnList;

    private void Awake()
    {
        Login.OnLoginSuccessEvent += OnLoginSuccess;
        MainSkillBtn.onClick.AddListener(() =>
        {
            OnMainSkillClicked?.Invoke();
        });

        int i = 1;
        foreach (var SkillBtn in SkillBtnList)
        {
            int temp = i;
            SkillBtn.onClick.AddListener(() =>
            {
                OnSkillClicked?.Invoke(temp);
            });
            i++;
        }

    }

    private void OnDestroy()
    {
        Login.OnLoginSuccessEvent -= OnLoginSuccess;
    }

    void OnLoginSuccess(UserInfo userInfo)
    {
        PhotonNetwork.NickName = userInfo.NickName;
        if (userInfo.IsFirstLogin)
        {
            FirstLoginUI.SetActive(true);
        }
        else
        {
            FirstLoginUI.SetActive(false);
            GameManager.Instance.ChangeState(GameState.InDefaultRoom);
        }
    }
}
