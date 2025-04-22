using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class AIBehaviorBase : Character
{
    public AIBehaviorInfoBase AIBehaviorInfo;
    public bool ShouldStartAI;
    Coroutine AIUpdateHandle;
    public NavMeshAgent NavMeshAgent;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        
        NavMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        NavMeshAgent.speed = Controller.MoveSpeed;
        NavMeshAgent.stoppingDistance = AIBehaviorInfo.AttackRange;

        AIBehaviorInfo.IsRunningAI = true;
        if(PhotonNetwork.IsMasterClient || ShouldStartAI)
        {
            AIUpdateHandle = StartCoroutine(UpdateAI());
        }
    }

    IEnumerator UpdateAI()
    {
        while(AIBehaviorInfo.IsRunningAI)
        {
            if(!StopMoving)
            {
                AIBehaviorInfo.UpdateState(Controller, gameObject);
                AIBehaviorInfo.DoAction(Controller, gameObject);
            }

            yield return new WaitForSeconds(AIBehaviorInfo.UpdateRate);
        }
    }

    public override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        base.OnPhotonSerializeView(stream, info);

        //if (stream.IsWriting)
        //{
        //    stream.SendNext(Rigidbody.velocity);
        //}
        //else
        //{
        //    Rigidbody.velocity = (Vector3)stream.ReceiveNext();
        //}
    }

    protected override void Update()
    {
        base.Update();

        if(StopMoving)
        {
            NavMeshAgent.isStopped = true;
        }
        else
        {
            NavMeshAgent.isStopped = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, AIBehaviorInfo.SenseRadius);
    }
}
