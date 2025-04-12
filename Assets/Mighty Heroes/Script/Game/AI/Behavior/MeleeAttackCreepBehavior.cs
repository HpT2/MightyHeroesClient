using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MeleeAttackCreepBehavior", menuName = "AI/Melee Attack Creep Behavior ")]
public class MeleeAttackCreepBehavior : AIBehaviorInfoBase
{
    public override void UpdateState(PlayerControllerComponent Controller, GameObject Owner)
    {
        base.UpdateState(Controller, Owner);

        CurrentState = EAIBehaviorState.ENone;

        Collider[] Colliders = Physics.OverlapSphere(Owner.transform.position, SenseRadius);

        List<GameObject> TempPlayer = new List<GameObject>();
        for(int i = 0; i < Colliders.Length; i++)
        {
            Collider col = Colliders[i];
            if(col.CompareTag("Player"))
            {
                TempPlayer.Add(col.gameObject);
            }
        }

        if(TempPlayer.Count == 0)
        {
            CurrentState = EAIBehaviorState.EPatrol;
            Target = null;
            return;
        }

        if(Target == null || !TempPlayer.Contains(Target))
        {
            Target = TempPlayer[0];
        }

        if(Vector3.Distance(Target.transform.position, Owner.transform.position) < AttackRange)
        {
            CurrentState = EAIBehaviorState.EAttack;
        }
        else
        {
            CurrentState = EAIBehaviorState.EChase;
        }
    }

    public override void DoAction(PlayerControllerComponent Controller, GameObject Owner)
    {
        base.DoAction(Controller, Owner);

        switch (CurrentState)
        {
            case EAIBehaviorState.ENone:
            case EAIBehaviorState.EPatrol:
                Controller.MoveDirection = Vector3.zero;
                break;
            case EAIBehaviorState.EAttack:
                Controller.MoveDirection = Vector3.zero;
                Owner.GetComponent<Character>().OnMainSkillTrigger();
                break;
            case EAIBehaviorState.EChase:
                Controller.MoveDirection = Owner.transform.forward;
                Owner.GetComponent<AIBehaviorBase>().NavMeshAgent.SetDestination(Target.transform.position);
                break;
        }

    }
}
