using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InRoom : BaseUI
{
    [SerializeField]
    private Button SettingBtn;

    [SerializeField]
    private Button RoomBtn;

    [SerializeField]
    private Button BagBtn;

    [SerializeField]
    private TextMeshProUGUI CoinValue;

    public override void Init()
    {
        base.Init();
        
        GameManager.OnEnterDefaultRoomEvent += OnEnterDefaultRoom;
        GameManager.OnEnterSharedRoomEvent += OnEnterSharedRoom;
        GameManager.OnBeginPlayingEvent += OnBeginPlaying;

        SettingBtn.onClick.AddListener(() =>
        {
            UIManager.AddDebugMessage("Setting Btn Clicked");
        });

        RoomBtn.onClick.AddListener(() =>
        {
            UIManager.AddDebugMessage("Room Btn Clicked");
        });

        BagBtn.onClick.AddListener(() =>
        {
            UIManager.AddDebugMessage("Bag Btn Clicked");
        });
    }

    public override void Deinit()
    {
        base.Deinit();

        GameManager.OnEnterDefaultRoomEvent -= OnEnterDefaultRoom;
        GameManager.OnEnterSharedRoomEvent -= OnEnterSharedRoom;
        GameManager.OnBeginPlayingEvent -= OnBeginPlaying;
        SettingBtn.onClick.RemoveAllListeners();
        RoomBtn.onClick.RemoveAllListeners();
        BagBtn.onClick.RemoveAllListeners();
    }

    void UpdateCoin()
    {
        CultureInfo customCulture = new CultureInfo("en-US");
        customCulture.NumberFormat.CurrencyGroupSeparator = " ";
        customCulture.NumberFormat.NumberGroupSeparator = " ";

        CoinValue.text = GameManager.Instance.ThisUserInfo.Coin.ToString("N0", customCulture);
    }

    void OnEnterDefaultRoom()
    {
        gameObject.SetActive(true);
        RoomBtn.gameObject.SetActive(true);
        UpdateCoin();
    }

    void OnEnterSharedRoom()
    {
        gameObject.SetActive(true);
        UpdateCoin();
        RoomBtn.gameObject.SetActive(false);
    }

    void OnBeginPlaying()
    {
        gameObject.SetActive(false);
    }
}
