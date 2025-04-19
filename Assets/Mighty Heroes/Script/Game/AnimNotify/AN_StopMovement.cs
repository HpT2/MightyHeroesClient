using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AN_StopMovement : StateMachineBehaviour
{
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        Character Owner = animator.gameObject.GetComponent<Character>();
        if(Owner && Owner.photonView.IsMine)
        {
            Owner.StopMoving = true;
            Owner.Rigidbody.isKinematic = true;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Character Owner = animator.gameObject.GetComponent<Character>();
        if (Owner && Owner.photonView.IsMine)
        {
            Owner.StopMoving = false;
            Owner.Rigidbody.isKinematic = false;
            Owner.Rigidbody.velocity = Owner.Controller.LastUpdateVelocity;
        }

        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}