using Newtonsoft.Json.Linq;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomList : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject RoomPrefab;

    [SerializeField]
    private Button CloseBtn;

    [SerializeField]
    private Button CreateBtn;

    [SerializeField]
    private Button FindBtn;

    [SerializeField]
    private TMP_InputField RoomNameInput;

    [SerializeField]
    private ScrollRect RoomListContent;

    [SerializeField]
    private GameObject RoomCreatePanel;

    public override void OnEnable()
    {
        base.OnEnable();

        CloseBtn.onClick.AddListener(() =>
        {
            //UIManager.AddDebugMessage("Hello");
            gameObject.SetActive(false);
        });

        CreateBtn.onClick.AddListener(() =>
        {
            RoomCreatePanel.SetActive(true);
        });

        FindBtn.onClick.AddListener(() =>
        {
            UIManager.AddDebugMessage("Find room : " + RoomNameInput.text, LogVerbose.Warning);
        });

        TypedLobby typedLobby = new TypedLobby("Mighty Heroes", LobbyType.SqlLobby);
        PhotonNetwork.GetCustomRoomList(typedLobby, "1=1");
    }

    public override void OnDisable()
    {
        CloseBtn.onClick.RemoveAllListeners();
        CreateBtn.onClick.RemoveAllListeners();
        FindBtn.onClick.RemoveAllListeners();

        RoomCreatePanel.SetActive(false);

        base.OnDisable();
    }

    void ClearRoom()
    {
        for (int i = RoomListContent.content.childCount - 1; i >= 0; i--)
        {
            Transform child = RoomListContent.content.GetChild(i);
            Destroy(child.gameObject);
        }

    }

    public override void OnRoomListUpdate(List<Photon.Realtime.RoomInfo> roomList)
    {
        base.OnRoomListUpdate(roomList);
        ClearRoom();

        //Add data to room list
        foreach (RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList)
                continue;

            string RoomName = roomInfo.Name;
            string MasterName = (string)roomInfo.CustomProperties["MN"];
            int MemberCount = roomInfo.PlayerCount;
            string Password = (string)roomInfo.CustomProperties["RPW"];
            UIManager.AddDebugMessage(roomInfo.CustomProperties.ToStringFull());
            GameObject Room = GameObject.Instantiate(RoomPrefab, RoomListContent.content);

            //Add data
            Room.GetComponent<RoomInfoMH>().Init(RoomName, MasterName, MemberCount, Password);
        }
    }
}