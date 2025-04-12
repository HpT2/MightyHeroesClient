using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RoomCreate : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private Button CancelBtn;

    [SerializeField]
    private Button ConfirmBtn;

    [SerializeField]
    private TMP_InputField RoomNameInput;

    [SerializeField]
    private TMP_InputField PasswordInput;

    //Photon Interface
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();

        RoomNameInput.interactable = PasswordInput.interactable = ConfirmBtn.interactable = CancelBtn.interactable = true;
        gameObject.SetActive(false);

        UIManager.AddDebugMessage(PhotonNetwork.CurrentRoom.CustomProperties.ToString());
    }
    //End photon interface

    public override void OnEnable()
    {
        base.OnEnable();

        CancelBtn.onClick.AddListener(OnCancelBtnClicked);
        ConfirmBtn.onClick.AddListener(OnConfirmBtnClicked);
    }

    public override void OnDisable()
    {
        CancelBtn.onClick.RemoveAllListeners();
        ConfirmBtn.onClick.RemoveAllListeners();

        RoomNameInput.text = "";
        PasswordInput.text = "";

        base.OnDisable();
    }

    void OnCancelBtnClicked()
    {
        gameObject.SetActive(false);
    }

    void OnConfirmBtnClicked()
    {
        string RoomName = RoomNameInput.text;
        string RoomPassword = PasswordInput.text;

        if(RoomName == "")
        {
            //Show Error msg
            return;
        }

        //Send room create info
        RoomNameInput.interactable = PasswordInput.interactable = ConfirmBtn.interactable = CancelBtn.interactable = false;

        CreateRoom(RoomName, RoomPassword);
    }

    void CreateRoom(string RoomName, string Password)
    {
        //Show loading
        // Set custom properties (including password)
        ExitGames.Client.Photon.Hashtable roomProperties = new ExitGames.Client.Photon.Hashtable();
        roomProperties.Add("RPW", Password);
        roomProperties.Add("MN", PhotonNetwork.NickName);

        RoomOptions opt = new RoomOptions
        {
            MaxPlayers = 4,
            CustomRoomProperties = roomProperties,
            CustomRoomPropertiesForLobby = new string[2] { "RPW", "MN" }
        };

        PhotonNetwork.CreateRoom(RoomName, opt, new TypedLobby("Mighty Heroes", LobbyType.SqlLobby));

        Camera.main.transform.SetParent(null);
    }

}
