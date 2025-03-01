using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomList : MonoBehaviour
{
    public static OnGetRoom_Delegate GetRoomEvent;

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

    private void Start()
    {
        SocketEvent.OnGetRoom += OnGetRoom;
    }

    private void OnGetRoom(JObject data)
    {
        //Add data to room list
    }

    private void OnEnable()
    {
        CloseBtn.onClick.AddListener(() =>
        {
            gameObject.SetActive(false);
        });

        CreateBtn.onClick.AddListener(() =>
        {
            UIManager.AddDebugMessage("Create room btn", LogVerbose.Warning);
        });

        FindBtn.onClick.AddListener(() =>
        {
            UIManager.AddDebugMessage("Find room : " + RoomNameInput.text, LogVerbose.Warning);
        });

        //get new room
        JObject Message = new JObject();
        SocketManager.Instance.EmitMessage(SocketEventName.EVENT_GET_ROOM_LIST, Message.ToString());
    }

    private void OnDisable()
    {
        CloseBtn.onClick.RemoveAllListeners();
        CreateBtn.onClick.RemoveAllListeners();
        FindBtn.onClick.RemoveAllListeners();

        //Clear old room
        for (int i = RoomListContent.content.childCount - 1; i >= 0; i--)
        {
            Transform child = RoomListContent.content.GetChild(i);
            Destroy(child.gameObject);
        }
    }
}