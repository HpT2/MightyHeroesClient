using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [SerializeField]
    private GameObject DebugMessagePrefab;

    [SerializeField]
    private GameObject DebugContent;

    [SerializeField]
    private GameObject DebugPanel;

    [SerializeField]
    private Button ShowDebugBtn;

    [SerializeField]
    private Button HideDebugBtn;

    [SerializeField]
    private List<BaseUI> UIList = new List<BaseUI>();

    [SerializeField]
    private TextMeshProUGUI PingText;

    Coroutine PingRoutine;

    private void Awake()
    {
        if(!Instance)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        ShowDebugBtn.onClick.AddListener(() =>
        {
            DebugPanel.SetActive(true);
        });

        HideDebugBtn.onClick.AddListener(() =>
        {
            DebugPanel.SetActive(false);
        });

        for(int i = 0; i < UIList.Count; i++)
        {
            UIList[i].Init();
        }
    }

    private void Start()
    {
        GameManager.OnJoinedLobbyEvent += OnJoinedLobby;
    }

    void OnJoinedLobby()
    {
        if(PingRoutine != null)
        {
            StopCoroutine(PingRoutine);
        }
        PingRoutine = StartCoroutine(UpdatePing());
    }

    IEnumerator UpdatePing()
    {
        while(true)
        {
            int Ping = PhotonNetwork.GetPing();
            Color color;
            if(Ping < 20)
            {
                color = Color.green;
            }
            else if(Ping < 60)
            {
                color = Color.yellow;
            }
            else
            {
                color = Color.red;
            }
            PingText.text = Ping + " ms";
            PingText.color = color;
            yield return new WaitForSeconds(3);
        }
    }

    public static void AddDebugMessage(string msg, LogVerbose verbose = LogVerbose.Log, bool LogInConsole = true)
    {
        Color color = Color.black;
        switch(verbose)
        {
            case LogVerbose.Log:
                color = Color.blue;
                if (LogInConsole) Debug.Log(DateTime.Now.ToString("HH:mm:ss") + ": " + msg);
                break;
            case LogVerbose.Warning:
                color = Color.yellow;
                if (LogInConsole) Debug.LogWarning(DateTime.Now.ToString("HH:mm:ss") + ": " + msg);
                break;
            case LogVerbose.Error:
                color = Color.red;
                if (LogInConsole) Debug.LogError(DateTime.Now.ToString("HH:mm:ss") + ": " + msg);
                break;
        }

        GameObject GO = GameObject.Instantiate(Instance.DebugMessagePrefab, Instance.DebugContent.transform);
        TextMeshProUGUI text = GO.GetComponent<TextMeshProUGUI>();
        text.text = DateTime.Now.ToString("HH:mm:ss") + ": " + msg;
        text.color = color;

        Canvas.ForceUpdateCanvases();
        Instance.DebugPanel.GetComponent<ScrollRect>().verticalNormalizedPosition = 0f;
    }

    public T GetUIByType<T>() where T : BaseUI
    {
        foreach(var UI in UIList)
        {
            if (UI.GetType() == typeof(T))
                return (T)UI;
        }
        return null;
    }
}

public enum LogVerbose
{
    Log,
    Warning,
    Error
}
