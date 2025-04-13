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

    [HideInInspector]
    public GameObject Spawner;

    public OnGameObjectDestroy_Delegate OnGameObjectDestroy;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == Spawner)
        {
            return;
        }

        UIManager.AddDebugMessage(other.gameObject.name);

        if(Spawner.GetComponent<PhotonView>().IsMine)
        {
            //Deal damage
        }

        SpawnEffectAndDestroy();
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
