using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TargetingMethod
{
    NEAREST,
    FURTHEST,
    RANDOM,
    CUSTOM
}

public delegate GameObject GetTargetingEnemy_Delegate();

public class AN_FindTarget : StateMachineBehaviour
{
    public GetTargetingEnemy_Delegate GetTargetingEnemy;

    public TargetingMethod TargetingMethod;

    public float Range;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);
        Character Owner = animator.gameObject.GetComponent<Character>();
        if(TargetingMethod == TargetingMethod.CUSTOM)
        {
            Owner.TargetingEnemy = GetTargetingEnemy?.Invoke();
        }
        else
        {
            Collider[] Colliders = Physics.OverlapSphere(Owner.transform.position, Range, LayerMask.GetMask("Damagable"));

            List<GameObject> Targets = new List<GameObject>();
            for (int i = 0; i < Colliders.Length; i++)
            {
                Collider col = Colliders[i];
                if(Owner.gameObject == col.gameObject)
                {
                    continue;
                }

                Targets.Add(col.gameObject);
            }

            if(Targets.Count == 0)
            {
                return;
            }

            GameObject FinalTarget = null;
            if (TargetingMethod == TargetingMethod.RANDOM)
            {
                FinalTarget = Targets[Random.Range(0, Targets.Count)];
            }
            else if(TargetingMethod == TargetingMethod.NEAREST)
            {
                Targets.Sort((a, b) =>
                {
                    return Vector3.Distance(Owner.transform.position, a.transform.position).CompareTo(Vector3.Distance(Owner.transform.position, b.transform.position));
                });
                FinalTarget = Targets[0];
            }
            else
            {
                Targets.Sort((a, b) =>
                {
                    return Vector3.Distance(Owner.transform.position, b.transform.position).CompareTo(Vector3.Distance(Owner.transform.position, a.transform.position));
                });
                FinalTarget = Targets[0];
            }

            Owner.TargetingEnemy = FinalTarget;
            //UIManager.AddDebugMessage("AN_FindTarget: " + FinalTarget.name);
        }
    }
}
