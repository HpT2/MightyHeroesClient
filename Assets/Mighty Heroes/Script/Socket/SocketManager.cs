using Newtonsoft.Json.Linq;
using Photon.Pun;
using System;
using UnityEngine;

public delegate void SocketEvent_OnLoginResponse(JObject data);
public delegate void OnGetRoom_Delegate(JObject data);
public class SocketEventName
{
    public static string EVENT_LOGIN = "login";
    public static string EVENT_MODIFY_NICKNAME = "modify nickname";
    public static string EVENT_GET_ROOM_LIST = "get room list";
}

public class SocketEvent
{
    public static SocketEvent_OnLoginResponse OnLoginResponse;
    public static OnGetRoom_Delegate OnGetRoom;
}

public class SocketManager : MonoBehaviour 
{
    public static SocketManager Instance { get; private set; }

    public SocketIOUnity Socket { get; private set; }

    public string ServerHost;
    public string ServerPort;

    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            return;
        }

        var uri = new Uri("http://" + ServerHost + ":" + ServerPort);
        Socket = new SocketIOUnity(uri);

        Socket.OnConnected += (sender, e) =>
        {
            UIManager.AddDebugMessage("Connected to Socket Server");
        };

        Socket.OnDisconnected += (sender, e) =>
        {
            UIManager.AddDebugMessage("Disconnected from server");
        };

        Socket.OnUnityThread(SocketEventName.EVENT_LOGIN, (SocketIOClient.SocketIOResponse response) =>
        {
            JObject data = JObject.Parse(response.GetValue<object>().ToString());
            SocketEvent.OnLoginResponse?.Invoke(data);
        });


        Socket.OnUnityThread(SocketEventName.EVENT_MODIFY_NICKNAME, (SocketIOClient.SocketIOResponse response) =>
        {
            JObject data = JObject.Parse(response.GetValue<object>().ToString());
            GameManager.OnNickNameModifiedEvent?.Invoke(data.Value<string>("NickName"));   
        });

        Socket.OnUnityThread(SocketEventName.EVENT_GET_ROOM_LIST, (SocketIOClient.SocketIOResponse response) =>
        {
            JObject data = JObject.Parse(response.GetValue<object>().ToString());
        });

        Socket.Connect();
    }

    public async void EmitMessage(string EventName,  string Message)
    {
        if(Socket.Connected)
        {
            await Socket.EmitAsync(EventName, Message);
        }
    }

    private void OnDestroy()
    {
        Socket.Disconnect();
    }
}
