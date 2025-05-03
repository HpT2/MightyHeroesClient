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

    [SerializeField]
    private List<GameObject> GameInput;

    [SerializeField]
    private Button LeaveRoomBtn;

    public Image CooldownImg;

    public override void Init()
    {
        Login.OnLoginSuccessEvent += OnLoginSuccess;
        MainSkillBtn.onClick.AddListener(() =>
        {
            OnMainSkillClicked?.Invoke();
        });

        LeaveRoomBtn.onClick.AddListener(() =>
        {
            if(PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
            }
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
        GameManager.OnCharacterSpawned += EnableInput;
        GameManager.OnLeftRoomEvent += DisableInput;
    }

    public override void Deinit()
    {
        Login.OnLoginSuccessEvent -= OnLoginSuccess;
        GameManager.OnCharacterSpawned -= EnableInput;
    }
    
    void EnableInput()
    {
        foreach (var Input in GameInput)
        {
            Input.SetActive(true);
        }
    }

    void DisableInput()
    {
        foreach (var Input in GameInput)
        {
            Input.SetActive(false);
        }
    }

    public void HideOnDeath()
    {
        for(int i = 0; i < SkillBtnList.Count - 1; i++)
        {
            GameInput[i].SetActive(false);
        }
    }

    void OnLoginSuccess(UserInfo userInfo)
    {
        gameObject.SetActive(true);
        if (userInfo.IsFirstLogin)
        {
            FirstLoginUI.SetActive(true);
        }
        else
        {
            FirstLoginUI.SetActive(false);
        }
    }
}
