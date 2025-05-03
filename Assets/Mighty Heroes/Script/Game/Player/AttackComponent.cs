using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public delegate void OnGameObjectDestroy_Delegate();

public class AttackComponent : MonoBehaviour
{
    [SerializeField]
    private GameObject AttackSuccessEffect;

    [SerializeField]
    private Vector3 AttackSuccessEffectScale = Vector3.one;

    public GameObject Spawner;

    public OnGameObjectDestroy_Delegate OnGameObjectDestroy;

    public bool DestroyOnCollision;

    public float ContinuosDamageInterval = 0;

    Dictionary<Character, Coroutine> ContinouseDmgRoutine = new Dictionary<Character, Coroutine>();

    private void OnTriggerEnter(Collider other)
    {
        Character DamagedChar = other.GetComponent<Character>();
        if (other.gameObject == Spawner || !DamagedChar)
        {
            return;
        }

        if(Spawner.tag == "Creep" && other.gameObject.tag == "Creep")
        {
            return;
        }

        float HP = DamagedChar.IngamePlayerStat.GetTotalStat(StatName.HP);
        if (HP == 0)
        {
            return;
        }

        //Debug.Log(other.gameObject);

        //UIManager.AddDebugMessage(other.gameObject.name);

        //Play Damage Anim
        Animator AnimController = other.GetComponent<Animator>();
        if(AnimController /*&& Check same team */)
        {
            AnimController.Play("TakeDamage", 0, 0);
        }


        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            //Deal damage
            float AtkPoint = Spawner.GetComponent<Character>().IngamePlayerStat.GetTotalStat(StatName.ATK);
            float DefPoint = DamagedChar.IngamePlayerStat.GetTotalStat(StatName.DEF);
            int ID = Spawner.GetComponent<PhotonView>().ViewID;
            float LevelDiff = DamagedChar.CurrentLevel - Spawner.GetComponent<Character>().CurrentLevel;
            if(LevelDiff > 0)
            {
                AtkPoint += AtkPoint * (0.1f * LevelDiff);
            }
            float Dmg = AtkPoint * (AtkPoint / (AtkPoint + DefPoint));
            DamagedChar.MulticastGetDamaged(Dmg, ID);
        }

        if(ContinuosDamageInterval > 0)
        {
            ContinouseDmgRoutine.Add(DamagedChar, StartCoroutine(ContinousDamage(DamagedChar)));
            return;
        }

        if (DestroyOnCollision || other.gameObject.layer == LayerMask.NameToLayer("WorldStatic"))
        {
            SpawnEffectAndDestroy();
        }
    }

    IEnumerator ContinousDamage(Character DamagedChar)
    {
        while(true)
        {
            yield return new WaitForSeconds(ContinuosDamageInterval);
            float HP = DamagedChar.IngamePlayerStat.GetTotalStat(StatName.HP);
            if (HP == 0)
            {
                continue;
            }

            Animator AnimController = DamagedChar.GetComponent<Animator>();
            if (AnimController /*&& Check same team */)
            {
                AnimController.Play("TakeDamage", 0, 0);
            }


            if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
            {
                //Deal damage
                float AtkPoint = Spawner.GetComponent<Character>().IngamePlayerStat.GetTotalStat(StatName.ATK);
                float DefPoint = DamagedChar.IngamePlayerStat.GetTotalStat(StatName.DEF);
                float Dmg = AtkPoint * (AtkPoint / (AtkPoint + DefPoint));
                int ID = Spawner.GetComponent<PhotonView>().ViewID;
                DamagedChar.MulticastGetDamaged(Dmg, ID);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Character DamagedChar = other.GetComponent<Character>();
        if (other.gameObject == Spawner || !DamagedChar)
        {
            return;
        }

        if (Spawner.tag == "Creep" && other.gameObject.tag == "Creep")
        {
            return;
        }

        if(ContinuosDamageInterval > 0)
        {
            Coroutine DmgRoutine;
            if(ContinouseDmgRoutine.TryGetValue(DamagedChar, out DmgRoutine))
            {
                StopCoroutine(DmgRoutine);
                ContinouseDmgRoutine.Remove(DamagedChar);
            }
        }
    }

    public void SpawnEffectAndDestroy()
    {
        if (AttackSuccessEffect)
        {
            GameObject Effect = GameObject.Instantiate(AttackSuccessEffect);
            Effect.transform.position = transform.position;
            Effect.transform.localScale = AttackSuccessEffectScale;
        }

        OnGameObjectDestroy?.Invoke();
        Destroy(gameObject);
    }

    public void SetSpawner(GameObject Spawner)
    {
        this.Spawner = Spawner;
    }
}
