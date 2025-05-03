using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ANS_AttackBox : StateMachineBehaviour
{
    public GameObject AttackBoxObj;
    public float StartTime;
    public float EndTime;
    public string SocketPath;

    private GameObject SpawnedAttackBox;


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);
        if(stateInfo.normalizedTime >= StartTime && stateInfo.normalizedTime < EndTime && !SpawnedAttackBox)
        {
            GameObject Spawner = animator.gameObject;
            Transform AttachSocket = Spawner.transform.Find(SocketPath);
            if (!AttachSocket)
            {
                AttachSocket = Spawner.transform;
            }
            SpawnedAttackBox = GameObject.Instantiate(AttackBoxObj, AttachSocket);
            SpawnedAttackBox.GetComponent<AttackComponent>().SetSpawner(Spawner);
        }
        else if(stateInfo.normalizedTime >= EndTime && SpawnedAttackBox)
        {
            GameObject.Destroy(SpawnedAttackBox);
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        GameObject.Destroy(SpawnedAttackBox);
        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}