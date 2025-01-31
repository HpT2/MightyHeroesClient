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
}

public enum LogVerbose
{
    Log,
    Warning,
    Error
}
