using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UIElements;

public enum CharacterRole
{
    CHARACTER_ROLE_PLAYER,
    CHARACTER_ROLE_CREEP,
}

public delegate void OnDeath_Delegate(Character DeathChar, Character Instigator);
public delegate void OnGetDamaged_Delegate(Character DamagedChar, float Damage);
public delegate void OnLevelUp_Delegate(int NewLevel);
public delegate void OnGainExp_Delegate(float CurrentExp, float MaxExp);

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

    [HideInInspector]
    public bool StopMoving;

    static GameObject HPBarPrefab;

    public OnDeath_Delegate OnDeath;
    public OnGetDamaged_Delegate OnGetDamage;
    public OnLevelUp_Delegate OnLevelUp;
    public OnGainExp_Delegate OnGainExp;

    [HideInInspector]
    public bool IsDeath = false;

    float CurrentExp = 0;
    public int CurrentLevel = 1;

    public Character() : base()
    {
        BasePlayerStat = new PlayerStat();
        IngamePlayerStat = new PlayerStat();
        Controller = new PlayerControllerComponent(this);
    }

    public void BindCamera()
    {
        MainCamTrans = Camera.main.transform;
        MainCamTrans.position = new Vector3(transform.position.x, 10, transform.position.z - 8);
        MainCamTrans.rotation = Quaternion.Euler(45, 0, 0);
    }

    protected virtual void Start()
    {
        if (!IsAI && ((photonView.IsMine) /*|| IsLocalControl*/))
        {
            Controller.BindInput();

            BindCamera();

            GameManager.OnCharacterSpawned?.Invoke();
            Ingame IngameUI = UIManager.Instance.GetUIByType<Ingame>();

            IngameUI.OnMainSkillClicked += OnMainSkillTrigger;

            if(IsLocalControl)
            {
                LocalChar = this;
            }

            UIManager.Instance.GetUIByType<GameOver>()?.BindOnDeath(this);
        }
        else
        {
            FirstFrame = true;
        }

        Rigidbody = gameObject.AddComponent<Rigidbody>();
        Rigidbody.freezeRotation = true;
        Rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Rigidbody.useGravity = false;

        BasePlayerStat.OnStatChange += Controller.OnStatChange;
        IngamePlayerStat.OnStatChange += Controller.OnStatChange;
        AnimController = GetComponent<Animator>();
        //Controller.CachedTransform(transform);

        InitCharacterWithData();
        
        if(!HPBarPrefab)
        {
            HPBarPrefab = Resources.Load("UI/HPBar") as GameObject;
        }

        GameObject HPBar = GameObject.Instantiate(HPBarPrefab, transform);
        RectTransform HPBarTrans = HPBar.GetComponent<RectTransform>();
        HPBarTrans.localPosition = new Vector3(0, 3.5f, 0);

        float scale = transform.localScale.x;
        HPBarTrans.localScale = new Vector3(1/scale, 1/scale, 1/scale);
        HPBarTrans.localPosition = new Vector3(0, 3.5f / scale, 0);

        HPBar.GetComponent<HPBar>()?.Init(this);

        OnDeath += OnSelfDeath;

        if(tag == "Player")
        {
            UIManager.Instance.GetUIByType<GameOver>()?.PlayerInRoom.Add(gameObject);
        }
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
            IngamePlayerStat.SetBaseStat(StatData.Name, StatData.Value);
            //UIManager.AddDebugMessage(gameObject.name + ".InitCharacterWithData: " + StatData.Name.ToString() + " = " + StatData.Value);
        }
    }

    protected virtual void Update()
    {
        if(!StopMoving)
        {
            Controller.Update();
        }
    }

    private void LateUpdate()
    {
        if (MainCamTrans)
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
            Controller.TargetPosition = (Vector3)stream.ReceiveNext() /*+ Controller.MoveDirection * Controller.MoveSpeed * Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime))*/;
            Controller.TargetRotation = (Quaternion)stream.ReceiveNext();
            Controller.UseExtrapolate = false;
            //transform.position = Vector3.MoveTowards(transform.position, Controller.TargetPosition, Controller.MoveSpeed * Time.deltaTime);
            //Controller.ElapsedTime = 0.0f;
            //Controller.StartPosition = transform.position;
            Rigidbody.velocity = Vector3.zero;

            if (FirstFrame)
            {
                FirstFrame = false;
                transform.position = Controller.TargetPosition + Controller.MoveDirection * Controller.MoveSpeed * Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
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

    public void OnSelfDeath(Character DeathChar, Character Instigator)
    {
        if(tag == "Player")
        {
            UIManager.Instance.GetUIByType<GameOver>()?.PlayerInRoom.Remove(gameObject);
        }

        if (Instigator && Instigator.gameObject.tag == "Player" && (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected))
        {
            Instigator.MulticastGainExp(CharacterData.BaseExpThrowWhenDeath * CurrentLevel);
        }

        AnimController.Play("Die", 0, 0);
        gameObject.tag = "Untagged";
        GetComponent<Collider>().enabled = false;
        if(Rigidbody)
        {
            Rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        IsDeath = true;
        StartCoroutine(Destroy());
    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3f);
        if(Role != CharacterRole.CHARACTER_ROLE_PLAYER)
        {
            if(PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.Destroy(gameObject);
            }
            else if(!PhotonNetwork.IsConnected)
            {
                GameObject.Destroy(gameObject);
            }
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public void OnMainSkillTrigger()
    {
        if(IsDeath)
        {
            return;
        }

        if (IsCoolingDownSkill)
        {
            //UIManager.AddDebugMessage("Cannot activate skill, cooldown not complete", LogVerbose.Warning);
            return;
        }

        if (!PhotonNetwork.IsConnected)
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

    public void MulticastGetDamaged(float Damage, int InstigatorID)
    {
        if (!PhotonNetwork.IsConnected)
        {
            GetDamaged(Damage, InstigatorID);
        }
        else
        {
            photonView.RPC("GetDamaged", RpcTarget.All, Damage, InstigatorID);
        }
    }

    public void MulticastGainExp(float Exp)
    {
        if (!PhotonNetwork.IsConnected)
        {
            GainExpRPC(Exp);
        }
        else
        {
            photonView.RPC("GainExpRPC", RpcTarget.All, Exp);
        }
    }

    [PunRPC]
    public void GainExpRPC(float Exp)
    {
        CurrentExp += Exp;
        float MaxExp = CharacterData.BaseExpNeedToLevelUp * CurrentLevel * 1.5f; 
        if(CurrentExp >=  MaxExp)
        {
            CurrentLevel += 1;
            CurrentExp = 0;
            OnLevelUp?.Invoke(CurrentLevel);
        }
        OnGainExp?.Invoke(CurrentExp, MaxExp);
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

    [PunRPC]
    public void GetDamaged(float Damage, int InstigatorID)
    {
        if(IsDeath)
        {
            return;
        }    

        float CurHP = IngamePlayerStat.GetTotalStat(StatName.HP);
        if(CurHP == 0)
        {
            return;
        }

        CurHP -= (int)Damage;
        if(CurHP <= 0)
        {
            CurHP = 0;
            PhotonView InstigatorView = PhotonView.Find(InstigatorID);
            OnDeath?.Invoke(this, InstigatorView ? InstigatorView.gameObject.GetComponent<Character>() : null);
        }
        IngamePlayerStat.SetStat(StatName.HP, CurHP);
        OnGetDamage?.Invoke(this, (int)Damage);
    }

    public IEnumerator StartCooldown()
    {
        IsCoolingDownSkill = true;
        if(photonView.IsMine && Role == CharacterRole.CHARACTER_ROLE_PLAYER)
        {
            float MaxCooldownTime = CharacterData.CharacterSkill.CooldownTime;
            float CooldownTime = MaxCooldownTime;
            Ingame IngameUI = UIManager.Instance.GetUIByType<Ingame>();
            IngameUI.CooldownImg.fillAmount = 1;
            while (true)
            {
                yield return null;
                CooldownTime -= Time.deltaTime;
                IngameUI.CooldownImg.fillAmount -= Time.deltaTime / MaxCooldownTime;
                if (CooldownTime <= 0)
                {
                    break;
                }
            }
        }
        else
        {
            yield return new WaitForSeconds(CharacterData.CharacterSkill.CooldownTime);
        }
        IsCoolingDownSkill = false;
    }

    public bool IsSameTeam(GameObject Other)
    {
        return tag == Other.tag;
    }
}
