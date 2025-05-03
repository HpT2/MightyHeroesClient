using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class GameManager : MonoBehaviourPunCallbacks
{
    public List<string> UsableCharacter = new List<string>();

    public static GameManager Instance;

    public UserInfo ThisUserInfo;

    public static OnNickNameModified OnNickNameModifiedEvent;
    public static OnCharacterSpawned_Delegate OnCharacterSpawned;
    public static OnJoinedRoom_Delegate OnJoinedRoomEvent;
    public static OnLeftRoom_Delegate OnLeftRoomEvent;
    public static OnJoinedLobby_Delegate OnJoinedLobbyEvent;
    [SerializeField]
    private GameObject CharacterData;

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        OnNickNameModifiedEvent += OnNickNameModifiedCallback;

        PhotonNetwork.KeepAliveInBackground = 120;

        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            PhotonNetwork.OfflineMode = true;
            UIManager.AddDebugMessage("Photon running in offline mode");
            //Offline mode will not be available in release
        }
        else
        {
            Login.OnLoginSuccessEvent += ConnectToPhotonServer;
        }
    }

    private void OnDestroy()
    {
        OnNickNameModifiedEvent -= OnNickNameModifiedCallback;
        Login.OnLoginSuccessEvent -= ConnectToPhotonServer;
    }

    public void SaveUserInfo(UserInfo userInfo)
    {
        ThisUserInfo = userInfo;
        PhotonNetwork.NickName = userInfo.NickName;
    }

    //Begin Photon interface
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        TypedLobby Lobby = new TypedLobby("Mighty Heroes", LobbyType.SqlLobby);
        PhotonNetwork.JoinLobby(Lobby);
        UIManager.AddDebugMessage($"GameManager.OnConnectedToMaster: Connected to Photon Server {PhotonNetwork.NickName}", LogVerbose.Warning);
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();

        OnJoinedLobbyEvent?.Invoke();
        //temp
        //GameObject TempChar = Resources.Load("Bee") as GameObject;
        //GameObject LocalChar = GameObject.Instantiate(TempChar, Vector3.zero, Quaternion.identity);
        //Character CharComp = LocalChar.GetComponent<Character>();
        //CharComp.IsLocalControl = true;
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        PhotonNetwork.Instantiate($"Character/{UsableCharacter[Random.Range(0, UsableCharacter.Count)]}", Vector3.zero, Quaternion.identity);
        OnJoinedRoomEvent?.Invoke();
        UIManager.AddDebugMessage($"Join other player room: {PhotonNetwork.CurrentRoom.Name}");
        //Destroy(Character.LocalChar.gameObject);
        //Hide Loading

        //test
        if(PhotonNetwork.IsMasterClient)
        {
            //PhotonNetwork.Instantiate($"Character/TestAI", Vector3.zero, Quaternion.identity);
        }
    }

    public override void OnLeftRoom()
    {
        base.OnLeftRoom();
        OnLeftRoomEvent?.Invoke();
    }

    //End Photon Interface

    void OnNickNameModifiedCallback(string NewNickName)
    {
        PhotonNetwork.NickName = NewNickName;
    }

    public void ConnectToPhotonServer(UserInfo info)
    {
        //PhotonNetwork.AuthValues = new AuthenticationValues(info.UserName);
        PhotonNetwork.ConnectUsingSettings();
        SaveUserInfo(info);
        //show loading screen
    }
}

public delegate void OnNickNameModified(string NewNickName);
public delegate void OnCharacterSpawned_Delegate();
public delegate void OnJoinedRoom_Delegate();
public delegate void OnLeftRoom_Delegate();
public delegate void OnJoinedLobby_Delegate();
public class Utils
{
    public static Vector3 FindGround(GameObject InGO)
    {
        RaycastHit Hit;
        Vector3 Start = InGO.transform.position;
        Start.y += 1f;
        if (Physics.Raycast(Start, Vector3.down, out Hit, 100, LayerMask.GetMask("WorldStatic")))
        {
            return new Vector3(Hit.point.x, Hit.point.y + 0.1f /* A little higher to the ground */, Hit.point.z);
        }

        return Vector3.zero;
    }
}