using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AN_Die : AnimNotify
{
    public float DieTime;
    bool HasDie = false;
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= DieTime && !HasDie)
        {
            HasDie = true;
            Character Owner = animator.gameObject.GetComponent<Character>();
            if (Owner && (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected))
            {
                Owner.MulticastGetDamaged(Owner.IngamePlayerStat.GetBaseStat(StatName.HP), -1);
            }
        }
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}
