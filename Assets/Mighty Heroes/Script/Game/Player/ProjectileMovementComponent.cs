using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        GetComponent<AttackComponent>().OnGameObjectDestroy += OnGameObjectDestroy;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.forward * Speed * Time.deltaTime;

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
            ParticleSystemGO.GetComponent<ParticleSystem>().Stop();
        }
    }
}
