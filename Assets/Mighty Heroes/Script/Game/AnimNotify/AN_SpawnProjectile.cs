using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class AN_SpawnProjectile : AnimNotify
{
    public string ProjectilePath;
    public string SocketName;
    public bool SpawnAtTarget;
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
                GameObject Projectile;
                if(Spawner.GetComponent<Character>().IsLocalControl)
                {
                    GameObject Obj = Resources.Load(ProjectilePath) as GameObject;
                    Projectile = GameObject.Instantiate(Obj);
                }
                else
                {
                    Projectile = PhotonNetwork.Instantiate(ProjectilePath, Vector3.zero, Quaternion.identity);
                }

                if(SpawnAtTarget)
                {

                }
                else
                {
                    if(SocketName.Length > 0)
                    {
                        Transform transform = Spawner.transform.Find(SocketName);
                        Projectile.transform.position = transform.position;
                    }
                    else
                    {
                        Projectile.transform.position = Spawner.transform.position;
                    }

                    Projectile.transform.forward = Spawner.transform.forward;
                }

                Projectile.GetComponent<AttackComponent>().SetSpawner(Spawner);
            }
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        bHasSpawned = false;
    }
}
