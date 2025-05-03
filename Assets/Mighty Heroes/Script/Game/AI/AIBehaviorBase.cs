using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class AIBehaviorBase : Character
{
    public AIBehaviorInfoBase AIBehaviorInfo;
    public AIBehaviorInfoBase AIBehaviorInfoInstance;

    public bool ShouldStartAI;
    Coroutine AIUpdateHandle;
    public NavMeshAgent NavMeshAgent;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        Role = CharacterRole.CHARACTER_ROLE_CREEP;

        NavMeshAgent = gameObject.AddComponent<NavMeshAgent>();
        NavMeshAgent.speed = Controller.MoveSpeed;
        NavMeshAgent.stoppingDistance = AIBehaviorInfo.AttackRange;

        AIBehaviorInfoInstance = ScriptableObject.CreateInstance("AIBehaviorInfoBase") as AIBehaviorInfoBase;
        AIBehaviorInfoInstance.IsRunningAI = true;
        if(PhotonNetwork.IsMasterClient || ShouldStartAI)
        {
            AIUpdateHandle = StartCoroutine(UpdateAI());
        }

        OnDeath += OnAIDeath;
    }

    IEnumerator UpdateAI()
    {
        while(AIBehaviorInfoInstance.IsRunningAI)
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

        if (stream.IsWriting)
        {
            stream.SendNext(AnimController.GetBool("IsMoving"));
        }
        else
        {
            AnimController.SetBool("IsMoving", (bool)stream.ReceiveNext());
        }
    }

    protected override void Update()
    {
        base.Update();

        if(StopMoving || IsDeath)
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

    void OnAIDeath(Character DeathAI, Character Instigator)
    {
        AIBehaviorInfoInstance.IsRunningAI = false;
    }
}
