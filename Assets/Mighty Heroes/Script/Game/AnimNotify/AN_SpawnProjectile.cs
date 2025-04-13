using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AN_SpawnProjectile : AnimNotify
{
    public string ProjectilePath;
    public string SocketName;
    public bool SpawnAtTarget;
    public bool FindGround;
    public float SpawnTime;
    private bool bHasSpawned = false;

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if(stateInfo.normalizedTime >= SpawnTime && !bHasSpawned)
        {
            bHasSpawned = true;
            PhotonView photonView = animator.gameObject.GetComponent<PhotonView>();
            if(photonView.IsMine)
            {
                GameObject Spawner = animator.gameObject;
                Vector3 SpawnPosition = Vector3.zero;
                Vector3 SpawnDirection = Vector3.zero;

                if(SpawnAtTarget)
                {
                    SpawnPosition = Spawner.GetComponent<Character>().TargetingEnemy.transform.position;
                    if(FindGround)
                    {
                        SpawnPosition = Utils.FindGround(Spawner.GetComponent<Character>().TargetingEnemy);
                    }
                }
                else
                {
                    if(SocketName.Length > 0)
                    {
                        Transform transform = Spawner.transform.Find(SocketName);
                        SpawnPosition = transform.position;
                    }
                    else
                    {
                        SpawnPosition = Spawner.transform.position;
                    }

                    SpawnDirection = Spawner.transform.forward;
                }

                Character SpawnerCharacter = Spawner.GetComponent<Character>();
                SpawnerCharacter.MulticastSpawnProjectile(ProjectilePath, SpawnPosition, SpawnDirection);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        bHasSpawned = false;
    }
}
