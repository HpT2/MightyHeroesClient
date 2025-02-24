using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Character : MonoBehaviourPun, IPunObservable
{
    public PlayerControllerComponent Controller;
    public PlayerStat BasePlayerStat;
    public PlayerStat IngamePlayerStat;

    public CharacterData CharacterData;

    public Character() : base()
    {
        BasePlayerStat = new PlayerStat();
        IngamePlayerStat = new PlayerStat();
        Controller = new PlayerControllerComponent(this);
    }

    private void Start()
    {
        if(photonView && photonView.IsMine)
        {
            Controller.BindInput();
            //temp
            Camera.main.transform.SetParent(transform, false);
            Camera.main.transform.position = new Vector3(0, 2, -2);
        }

        BasePlayerStat.OnStatChange += Controller.OnStatChange;
        IngamePlayerStat.OnStatChange += Controller.OnStatChange;

        InitCharacterWithData();
    }

    private void OnDestroy()
    {
        if(photonView.IsMine)
        {
            Controller.UnbindInput();
        }
        BasePlayerStat.OnStatChange -= Controller.OnStatChange;
        IngamePlayerStat.OnStatChange -= Controller.OnStatChange;
        Controller = null;
    }

    void InitCharacterWithData()
    {
        foreach(var StatData in CharacterData.BaseCharacterStatData)
        {
            BasePlayerStat.SetBaseStat(StatData.Name, StatData.Value);
            UIManager.AddDebugMessage(gameObject.name + ".InitCharacterWithData: " + StatData.Name.ToString() + " = " + StatData.Value);
        }
    }

    private void Update()
    {
        Controller.Update();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Controller.MoveDirection);
            stream.SendNext(Controller.MoveSpeed);
        }
        else
        {
            Controller.MoveDirection = (Vector3)stream.ReceiveNext();
            Controller.MoveSpeed = (float)stream.ReceiveNext();
        }
    }

    public float GetTotalStat(StatName Name)
    {
        if(GameManager.Instance.GetCurrentGameState() == GameState.Playing)
        {
            return IngamePlayerStat.GetTotalStat(Name);
        }
        else
        {
            return BasePlayerStat.GetTotalStat(Name);
        }
    }
}
