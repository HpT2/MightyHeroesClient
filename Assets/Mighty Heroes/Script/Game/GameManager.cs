using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum GameState
{
    LoggingIn,
    InDefaultRoom,
    InSharedRoom,
    Playing,
}

public class GameManager : MonoBehaviourPunCallbacks, IPunObservable
{
    public static GameManager Instance;

    public UserInfo ThisUserInfo;

    private GameState CurrentGameState;

    public static OnNickNameModified OnNickNameModifiedEvent;
    public static OnEnterDefaultRoom OnEnterDefaultRoomEvent;
    public static OnEnterSharedRoom OnEnterSharedRoomEvent;
    public static OnBeginPlaying OnBeginPlayingEvent;

    [SerializeField]
    private GameObject Character;

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

        CurrentGameState = GameState.LoggingIn;

        Login.OnLoginSuccessEvent += SaveUserInfo;
        OnNickNameModifiedEvent += OnNickNameModifiedCallback;

        if(Application.internetReachability == NetworkReachability.NotReachable)
        {
            PhotonNetwork.OfflineMode = true;
            UIManager.AddDebugMessage("Photon running in offline mode");
            //Offline mode will not be available in release
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    private void OnDestroy()
    {
        Login.OnLoginSuccessEvent -= SaveUserInfo;
        OnNickNameModifiedEvent -= OnNickNameModifiedCallback;
    }

    void SaveUserInfo(UserInfo userInfo)
    {
        ThisUserInfo = userInfo;
    }

    //Begin Photon interface
    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        UIManager.AddDebugMessage($"GameManager.OnConnectedToMaster: Connected to Photon Server", LogVerbose.Warning);
        Photon.Realtime.RoomOptions roomOptions = new Photon.Realtime.RoomOptions();
        PhotonNetwork.JoinOrCreateRoom("Test", roomOptions, Photon.Realtime.TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        PhotonNetwork.Instantiate("Bat", Vector3.zero, Quaternion.identity);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(ThisUserInfo);
        }
        else
        {
            ThisUserInfo = (UserInfo)stream.ReceiveNext();
        }
    }
    //End Photon Interface

    void OnNickNameModifiedCallback(string NewNickName)
    {
        PhotonNetwork.NickName = NewNickName;
    }

    public void ChangeState(GameState NewState)
    {
        CurrentGameState = NewState;
        switch(NewState)
        {
            case GameState.InDefaultRoom:
                OnEnterDefaultRoomEvent?.Invoke();
                break;
            case GameState.InSharedRoom:
                OnEnterSharedRoomEvent?.Invoke();
                break;
            case GameState.Playing:
                OnBeginPlayingEvent?.Invoke();
                break;
        }

        UIManager.AddDebugMessage($"GameManager.ChangeState: NewState: {NewState}");
    }

    public GameState GetCurrentGameState()
    {
        return CurrentGameState; 
    }
}

public delegate void OnNickNameModified(string NewNickName);
public delegate void OnEnterDefaultRoom();
public delegate void OnEnterSharedRoom();
public delegate void OnBeginPlaying();