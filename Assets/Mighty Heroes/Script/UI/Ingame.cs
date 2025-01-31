using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingame : MonoBehaviour
{
    [SerializeField]
    private GameObject FirstLoginUI;

    private void Awake()
    {
        Login.OnLoginSuccessEvent += OnLoginSuccess;
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
