using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EventName
{
    public static string EVENT_LOGIN = "login";
}

public class SocketManager : MonoBehaviour 
{
    public static SocketManager Instance { get; private set; }

    public SocketIOUnity Socket { get; private set; }

    public string ServerHost;
    public string ServerPort;

    private void Awake()
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
            Debug.Log("Connected to Server");
        };

        Socket.OnDisconnected += (sender, e) =>
        {
            Debug.Log("Disconnected from server");
        };

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
