using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

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

        Socket.On("message", (response) =>
        {
            Debug.Log(response);

            Thread.Sleep(2000);
            Socket.Emit("message", "Hello from client");
        });

        Socket.Connect();
    }
}
