using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public enum CharacterRole
{
    CHARACTER_ROLE_PLAYER,
    CHARACTER_ROLE_CREEP,
}

public class Character : MonoBehaviourPun, IPunObservable
{
    public PlayerControllerComponent Controller;
    public PlayerStat BasePlayerStat;
    public PlayerStat IngamePlayerStat;

    public Animator AnimController;

    public CharacterData CharacterData;
    public bool IsLocalControl;
    public bool IsAI;
    public bool OfflineTest;
    public Rigidbody Rigidbody;
    public CharacterRole Role;

    public static Character LocalChar;

    private Transform MainCamTrans;
    private bool FirstFrame;

    private bool IsCoolingDownSkill = false;

    [HideInInspector]
    public GameObject TargetingEnemy;

    public Character() : base()
    {
        BasePlayerStat = new PlayerStat();
        IngamePlayerStat = new PlayerStat();
        Controller = new PlayerControllerComponent(this);
    }

    protected virtual void Start()
    {
        if (!IsAI && ((photonView.IsMine) || IsLocalControl))
        {
            Controller.BindInput();

            MainCamTrans = Camera.main.transform;
            MainCamTrans.position = new Vector3(transform.position.x, 10, transform.position.z - 8);
            MainCamTrans.rotation = Quaternion.Euler(45, 0, 0);

            GameManager.OnCharacterSpawned?.Invoke();
            Ingame IngameUI = UIManager.Instance.GetUIByType<Ingame>();

            IngameUI.OnMainSkillClicked += OnMainSkillTrigger;

            Rigidbody = gameObject.AddComponent<Rigidbody>();
            Rigidbody.freezeRotation = true;
            Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            Rigidbody.useGravity = false;

            if(IsLocalControl)
            {
                LocalChar = this;
            }
        }
        else
        {
            FirstFrame = true;
        }

        BasePlayerStat.OnStatChange += Controller.OnStatChange;
        IngamePlayerStat.OnStatChange += Controller.OnStatChange;
        AnimController = GetComponent<Animator>();
        //Controller.CachedTransform(transform);

        InitCharacterWithData();
    }

    private void OnDestroy()
    {
        if(!IsAI && (photonView.IsMine || IsLocalControl))
        {
            Controller.UnbindInput();

            Ingame IngameUI = UIManager.Instance.GetUIByType<Ingame>();
            IngameUI.OnMainSkillClicked -= OnMainSkillTrigger;
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

    protected virtual void Update()
    {
        Controller.Update();
    }

    private void LateUpdate()
    {
        if (!IsAI && (photonView.IsMine || IsLocalControl))
        {
            MainCamTrans.position = new Vector3(transform.position.x, 10, transform.position.z - 8);
        }
    }

    public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(Controller.MoveDirection);
            stream.SendNext(Controller.MoveSpeed);

            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            Controller.MoveDirection = (Vector3)stream.ReceiveNext();
            Controller.MoveSpeed = (float)stream.ReceiveNext();
            Controller.TargetPosition = (Vector3)stream.ReceiveNext() + Controller.MoveDirection * Controller.MoveSpeed * Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            Controller.TargetRotation = (Quaternion)stream.ReceiveNext();
            Controller.UseExtrapolate = false;

            if(FirstFrame)
            {
                FirstFrame = false;
                transform.position = Controller.TargetPosition;
                transform.rotation = Controller.TargetRotation;
            }

            //transform.position = (Vector3)stream.ReceiveNext();
            //transform.rotation = (Quaternion)stream.ReceiveNext();
            //transform.position = Vector3.Lerp(transform.position, (Vector3)stream.ReceiveNext(), Time.deltaTime * Controller.MoveSpeed);
        }
    }

    public float GetTotalStat(StatName Name)
    {
        if(false) // check game playing
        {
            return IngamePlayerStat.GetTotalStat(Name);
        }
        else
        {
            return BasePlayerStat.GetTotalStat(Name);
        }
    }

    public void OnMainSkillTrigger()
    {
        if (IsCoolingDownSkill)
        {
            UIManager.AddDebugMessage("Cannot activate skill, cooldown not complete", LogVerbose.Warning);
            return;
        }

        if (IsLocalControl || OfflineTest)
        {
            OnMainSkillTriggerRPC();
        }
        else
        {
            photonView.RPC("OnMainSkillTriggerRPC", RpcTarget.All);
        }
    }

    public void MulticastSpawnProjectile(string ProjectilePath, Vector3 Position, Vector3 Direction)
    {
        if(!PhotonNetwork.IsConnected)
        {
            SpawnProjectTile(ProjectilePath, Position, Direction, -1);
        }
        else
        {
            photonView.RPC("SpawnProjectTile", RpcTarget.All, ProjectilePath, Position, Direction, photonView.ViewID);
        }
    }

    [PunRPC]
    public void OnMainSkillTriggerRPC()
    {
        CharacterData.CharacterSkill.ActivateSkill(this);
    }

    [PunRPC]
    public void SpawnProjectTile(string ProjectilePath, Vector3 Position, Vector3 Direction, int SpawnerViewID)
    {
        GameObject ProjectilePrefab = Resources.Load(ProjectilePath) as GameObject;
        GameObject Projectile = GameObject.Instantiate(ProjectilePrefab, Position, Quaternion.identity);
        Projectile.transform.forward = Direction;
        PhotonView SpawnerView = PhotonView.Find(SpawnerViewID);
        if(SpawnerView)
        {
            Projectile.GetComponent<AttackComponent>()?.SetSpawner(SpawnerView.gameObject);
        }    
        else
        {
            Projectile.GetComponent<AttackComponent>()?.SetSpawner(gameObject);
        }
        //UIManager.AddDebugMessage(Projectile.name);
    }

    public IEnumerator StartCooldown()
    {
        IsCoolingDownSkill = true;
        yield return new WaitForSeconds(CharacterData.CharacterSkill.CooldownTime);
        IsCoolingDownSkill = false;
    }

    public bool IsSameTeam(GameObject Other)
    {
        return tag == Other.tag;
    }
}
