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

    [SerializeField]
    private GameObject RoomList;

    public override void Init()
    {
        base.Init();

        SettingBtn.onClick.AddListener(() =>
        {
            UIManager.AddDebugMessage("Setting Btn Clicked");
        });

        RoomBtn.onClick.AddListener(() =>
        {
            RoomList.SetActive(true);
        });

        BagBtn.onClick.AddListener(() =>
        {
            UIManager.AddDebugMessage("Bag Btn Clicked");
        });

        GameManager.OnJoinedRoomEvent += OnJoinedRoom;
        GameManager.OnLeftRoomEvent += OnLeftRoom;
    }

    public override void Deinit()
    {
        base.Deinit();

        SettingBtn.onClick.RemoveAllListeners();
        RoomBtn.onClick.RemoveAllListeners();
        BagBtn.onClick.RemoveAllListeners();

        GameManager.OnJoinedRoomEvent -= OnJoinedRoom;
        GameManager.OnLeftRoomEvent -= OnLeftRoom;
    }

    private void OnEnable()
    {
        UpdateCoin();
    }

    void UpdateCoin()
    {
        CultureInfo customCulture = new CultureInfo("en-US");
        customCulture.NumberFormat.CurrencyGroupSeparator = " ";
        customCulture.NumberFormat.NumberGroupSeparator = " ";

        CoinValue.text = GameManager.Instance.ThisUserInfo.Coin.ToString("N0", customCulture);
    }

    void OnJoinedRoom()
    {
        RoomBtn.gameObject.SetActive(false);
        RoomList.SetActive(false);
    }

    void OnLeftRoom()
    {
        RoomBtn.gameObject.SetActive(true);
    }

    void OnBeginPlaying()
    {
        gameObject.SetActive(false);
    }
}
