using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Loading : BaseUI
{
    public Image BG;
    public TextMeshProUGUI LoadingText;

    public override void Init()
    {
        base.Init();

        GameManager.OnLeftRoomEvent += OnLeftRoom;
        GameManager.OnJoinedLobbyEvent += OnJoinedLobby;
    }

    public override void Deinit()
    {
        GameManager.OnLeftRoomEvent -= OnLeftRoom;
        GameManager.OnJoinedLobbyEvent -= OnJoinedLobby;
        base.Deinit();
    }

    public void Show()
    {
        OnLeftRoom();
    }

    void OnLeftRoom()
    {
        LoadingText.text = "Loading";
        Color color = BG.color;
        color.a = 1;
        BG.color = color;

        gameObject.SetActive(true);

        StartCoroutine(PlayTextRoutine());
        StartCoroutine(PlayBGRoutine());
    }

    void OnJoinedLobby()
    {
        StopAllCoroutines();
        gameObject.SetActive(false);
    }

    IEnumerator PlayTextRoutine()
    {
        int Count = 0;
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (Count == 3)
            {
                Count = 0;
            }
            else
            {
                Count++;
            }

            LoadingText.text = "Loading";
            for (int i = 0; i < Count; i++)
            {
                LoadingText.text += ".";
            }
        }
    }

    IEnumerator PlayBGRoutine()
    {
        bool IsIncrease = false;
        while (true)
        {
            yield return null;
            Color color = BG.color;
            if(color.a < 0.2f)
            {
                IsIncrease = true;
            }
            else if(color.a >= 1)
            {
                IsIncrease = false;
            }

            if(IsIncrease)
            {
                color.a += 0.5f * Time.deltaTime;
            }
            else
            {
                color.a -= 0.5f * Time.deltaTime;
            }

            BG.color = color;
        }
    }
}
