using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AN_StopMovement : StateMachineBehaviour
{
    public float StartTime;
    public float EndTime = 0.9f;
    bool Stopped = false;
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(stateInfo.normalizedTime >= StartTime && stateInfo.normalizedTime <= EndTime && !Stopped)
        {
            Character Owner = animator.gameObject.GetComponent<Character>();
            if (Owner && Owner.photonView.IsMine)
            {
                Owner.StopMoving = true;
                Owner.Rigidbody.constraints = RigidbodyConstraints.FreezePosition;
            }
            Stopped = true;
        }
        else if(stateInfo.normalizedTime >= EndTime && Stopped)
        {
            Character Owner = animator.gameObject.GetComponent<Character>();
            if (Owner && Owner.photonView.IsMine)
            {
                Owner.StopMoving = false;
                Owner.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                Owner.Rigidbody.velocity = Owner.Controller.LastUpdateVelocity;
            }

            Stopped = false;
        }
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        Character Owner = animator.gameObject.GetComponent<Character>();
        if (Owner && Owner.photonView.IsMine)
        {
            Owner.StopMoving = false;
            Owner.Rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            Owner.Rigidbody.velocity = Owner.Controller.LastUpdateVelocity;
        }

        base.OnStateExit(animator, stateInfo, layerIndex);
    }
}