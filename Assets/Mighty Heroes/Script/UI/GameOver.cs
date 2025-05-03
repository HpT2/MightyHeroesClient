using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOver : BaseUI
{
    public TextMeshProUGUI GameOverText;
    public TextMeshProUGUI GuideText;
    public Image BG;
    bool EnableTouch = false;
    [HideInInspector] 
    public List<GameObject> PlayerInRoom = new List<GameObject>();
    int FollowPlayerIndex = 0;

    private void Start()
    {
        GameManager.OnLeftRoomEvent += OnLeftRoom;
    }

    void OnLeftRoom()
    {
        PlayerInRoom.Clear();
        FollowPlayerIndex = 0;
        gameObject.SetActive(false);
    }

    public void BindOnDeath(Character Owner)
    {
        Owner.OnDeath += OnOwnerDeath;
    }

    void HideAll()
    {
        Color BGColor = BG.color;
        BGColor.a = 0;
        BG.color = BGColor;

        Color GameOverTextColor = GameOverText.color;
        GameOverTextColor.a = 0;
        GameOverText.color = GameOverTextColor;

        Color color = GuideText.color;
        color.a = 0;
        GuideText.color = color;
    }

    void OnOwnerDeath(Character Owner, Character Instigator)
    {
        HideAll();
        EnableTouch = false;
        gameObject.SetActive(true);
        StartCoroutine(ShowGameOver());
    }

    IEnumerator ShowGameOver()
    {
        yield return new WaitForSeconds(2.0f);

        float BGAlpha = 150.0f / 255;

        while(GameOverText.color.a < 1)
        {
            if(BG.color.a < BGAlpha)
            {
                Color BGColor = BG.color;
                BGColor.a += BGAlpha * Time.deltaTime;
                BG.color = BGColor;
            }

            Color GameOverTextColor = GameOverText.color;
            GameOverTextColor.a += 0.5f * Time.deltaTime;
            GameOverText.color = GameOverTextColor;

            yield return null;
        }

        StartCoroutine(ShowGuide());
    }

    IEnumerator ShowGuide()
    {
        yield return new WaitForSeconds(1.0f);
        while(GuideText.color.a < 1)
        {
            Color color = GuideText.color;
            color.a += 2 * Time.deltaTime;
            GuideText.color = color;
            yield return null;
        }
        UIManager.Instance.GetUIByType<Ingame>()?.HideOnDeath();
        EnableTouch = true;
    }

    void ChangePlayer()
    {
        if(FollowPlayerIndex == PlayerInRoom.Count - 1)
        {
            FollowPlayerIndex = -1;
        }

        for(int i = 0; i < PlayerInRoom.Count; i++)
        {
            if(PlayerInRoom[i] != null && !PlayerInRoom[i].GetComponent<Character>().IsDeath)
            {
                if(i > FollowPlayerIndex)
                {
                    FollowPlayerIndex = i;
                    break;
                }
            }
        }
        if(FollowPlayerIndex != -1)
        {
            PlayerInRoom[FollowPlayerIndex]?.GetComponent<Character>().BindCamera();
        }

    }

    private void Update()
    {
        if(EnableTouch)
        {
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0); // Get the first touch

                if (touch.phase == TouchPhase.Began)
                {
                    HideAll();
                    ChangePlayer();
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                HideAll();
                ChangePlayer();
            }
        }
    }
}
