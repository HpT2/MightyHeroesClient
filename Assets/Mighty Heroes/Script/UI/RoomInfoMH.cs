using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomInfoMH : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI RoomName;

    [SerializeField]
    private TextMeshProUGUI RoomMasterName;

    [SerializeField]
    private TextMeshProUGUI MemberCount;

    [SerializeField]
    private GameObject LockImg;

    [SerializeField]
    private GameObject UnlockImg;

    [SerializeField]
    private Button SelfBtn;

    private string Password = "";

    public void Init(string roomName, string masterName, int memberCount, string password)
    {
        RoomName.text = roomName;
        RoomMasterName.text = "Room Master: " + masterName;
        MemberCount.text = memberCount.ToString();
        Password = password;
    }

    private void Start()
    {
        if(Password.Length == 0)
        {
            LockImg.SetActive(false);
            UnlockImg.SetActive(true);
        }
        else
        {
            UnlockImg.SetActive(false);
            LockImg.SetActive(true);
        }

        SelfBtn.onClick.AddListener(OnRoomClicked);
    }

    private void OnDestroy()
    {
        SelfBtn.onClick.RemoveAllListeners();
    }

    void OnRoomClicked()
    {
        if(Password.Length == 0)
        {
            PhotonNetwork.JoinRoom(RoomName.text);
        }
        else
        {
            UIManager.AddDebugMessage("Open password panel");
        }
    }
}
