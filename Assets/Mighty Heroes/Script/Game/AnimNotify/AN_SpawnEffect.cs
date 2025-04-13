using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AN_SpawnEffect : AnimNotify
{
    //Paticle system object to spawn
    public GameObject EffectToSpawn;
    public bool bShouldFindGround;
    public bool bAttachToOwner;
    public bool bAttachToEnemy;
    public float SpawnTime;
    public Vector3 Scale = Vector3.one;

    private bool bHasSpawned;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        bHasSpawned = false;
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if(stateInfo.normalizedTime >= SpawnTime && !bHasSpawned)
        {
            bHasSpawned = true;

            GameObject OwnerGO = animator.gameObject;
            GameObject EffectGO = GameObject.Instantiate(EffectToSpawn, OwnerGO.transform.position, Quaternion.identity);
            EffectGO.transform.localScale = Scale;

            if(bAttachToOwner)
            {
                EffectGO.transform.parent = OwnerGO.transform;
            }
            else if(bAttachToEnemy)
            {
                GameObject TargetEnemy = EffectGO.GetComponent<Character>().TargetingEnemy;
                EffectGO.transform.parent = TargetEnemy ? TargetEnemy.transform : null;
                EffectGO.transform.position = TargetEnemy ? TargetEnemy.transform.position : EffectGO.transform.position;
            }

            if(bShouldFindGround)
            {
                EffectGO.transform.position = Utils.FindGround(EffectGO);
            }
        }
    }
}
