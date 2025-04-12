using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EAIBehaviorState
{
    ENone,
    EPatrol,
    EChase,
    EAttack,
    ERunAway
}

[CreateAssetMenu(fileName = "AIBehaviorInfoBase", menuName = "AI/AI Behavoir Info Base")]
public class AIBehaviorInfoBase : ScriptableObject
{
    public float UpdateRate = 0.2f;

    [HideInInspector]
    public bool IsAlive;

    [HideInInspector]
    public bool IsRunningAI;

    [HideInInspector]
    public EAIBehaviorState CurrentState = EAIBehaviorState.ENone;

    [HideInInspector]
    public GameObject Target;

    public float SenseRadius;

    public float AttackRange;

    public float AcceptedChaseRadius;

    public virtual void UpdateState(PlayerControllerComponent Controller, GameObject Owner)
    {
        //Do something with AI
    }

    public virtual void DoAction(PlayerControllerComponent Controller, GameObject Owner)
    {
        //Do action
    }
}

