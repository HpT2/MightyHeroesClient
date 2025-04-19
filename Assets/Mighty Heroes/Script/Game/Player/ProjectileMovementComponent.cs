using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ProjectileMovementComponent : MonoBehaviour
{
    public float Speed;
    public float MaxRange;
    public bool PreserveParticle;
    public GameObject ParticleSystemGO;

    private Vector3 SpawnedLocation;

    private void Start()
    {
        SpawnedLocation = transform.position;
        AttackComponent AtkComp = GetComponent<AttackComponent>();
        AtkComp.OnGameObjectDestroy += OnGameObjectDestroy;
        GetComponent<Rigidbody>().velocity = transform.forward * Speed;
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(SpawnedLocation, transform.position) >= MaxRange)
        {
            GetComponent<AttackComponent>().SpawnEffectAndDestroy();
        }
    }

    void OnGameObjectDestroy()
    {
        if (PreserveParticle && ParticleSystemGO)
        {
            ParticleSystemGO.transform.parent = null;
            if(ParticleSystemGO.GetComponent<ParticleSystem>().main.loop)
            {
                ParticleSystemGO.GetComponent<ParticleSystem>().Stop();
            }
        }
    }
}
