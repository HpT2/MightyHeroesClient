using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnAnimationFinished_Delegate();
public delegate void OnAnimationStart_Delegate();
public class AnimNotify : StateMachineBehaviour
{
    public OnAnimationFinished_Delegate OnAnimationFinished;
    public OnAnimationStart_Delegate OnAnimationStart;
    public int StateHash;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        StateHash = stateInfo.fullPathHash;
        OnAnimationStart?.Invoke();
    }

    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateUpdate(animator, stateInfo, layerIndex);

        if(stateInfo.normalizedTime >= 1)
        {
            OnAnimationFinished?.Invoke();
            UIManager.AddDebugMessage(animator.gameObject.name);
        }
    }
}