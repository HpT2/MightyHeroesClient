using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RangeAttackCreepBehavior", menuName = "AI/Range Attack Creep Behavior ")]
public class RangeAttackCreepBehavior : AIBehaviorInfoBase
{
    public override void UpdateState(PlayerControllerComponent Controller, GameObject Owner)
    {
        base.UpdateState(Controller, Owner);

        CurrentState = EAIBehaviorState.ENone;

        Collider[] Colliders = Physics.OverlapSphere(Owner.transform.position, SenseRadius, LayerMask.GetMask("Damagable"));

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
            return;
        }

        GameObject FinalTarget = null;
        if (TargetingMethod == TargetingMethod.RANDOM)
        {
            FinalTarget = TempPlayer[Random.Range(0, TempPlayer.Count)];
        }
        else if (TargetingMethod == TargetingMethod.NEAREST)
        {
            TempPlayer.Sort((a, b) =>
            {
                return Vector3.Distance(Owner.transform.position, a.transform.position).CompareTo(Vector3.Distance(Owner.transform.position, b.transform.position));
            });
            FinalTarget = TempPlayer[0];
        }
        else
        {
            TempPlayer.Sort((a, b) =>
            {
                return Vector3.Distance(Owner.transform.position, b.transform.position).CompareTo(Vector3.Distance(Owner.transform.position, a.transform.position));
            });
            FinalTarget = TempPlayer[0];
        }

        Character OwnerChar = Owner.GetComponent<Character>();
        if(OwnerChar.TargetingEnemy == null || !TempPlayer.Contains(Target))
        {
            OwnerChar.TargetingEnemy = TempPlayer[0];
        }

        if (Vector3.Distance(OwnerChar.TargetingEnemy.transform.position, Owner.transform.position) < AttackRange)
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

        Character OwnerChar = Owner.GetComponent<Character>();
        OwnerChar.AnimController.SetBool("IsMoving", false);
        Owner.GetComponent<AIBehaviorBase>().NavMeshAgent.SetDestination(Owner.transform.position);
        switch (CurrentState)
        {
            case EAIBehaviorState.ENone:
            case EAIBehaviorState.EPatrol:
                Controller.MoveDirection = Vector3.zero;
                break;
            case EAIBehaviorState.EAttack:
                Controller.MoveDirection = Vector3.zero;

                Vector3 A = Owner.transform.forward;
                A.y = 0;
                Vector3 B = (OwnerChar.TargetingEnemy.transform.position - Owner.transform.position).normalized;
                B.y = 0;
                float angle = Vector3.Angle(A, B);

                if (Mathf.Abs(angle) < 2.5)
                {
                    OwnerChar.OnMainSkillTrigger();
                }
                else
                {
                    Controller.MoveDirection = (OwnerChar.TargetingEnemy.transform.position - Owner.transform.position).normalized;
                }
                break;
            case EAIBehaviorState.EChase:
                Controller.MoveDirection = Owner.transform.forward;
                OwnerChar.AnimController.SetBool("IsMoving", true);
                Owner.GetComponent<AIBehaviorBase>().NavMeshAgent.SetDestination(OwnerChar.TargetingEnemy.transform.position);
                break;
        }

    }
}
