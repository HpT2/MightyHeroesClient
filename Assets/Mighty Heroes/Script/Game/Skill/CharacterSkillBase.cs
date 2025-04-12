using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ESkillType
{
    EquipmentSkill,
    CharacterSkill,
}

[CreateAssetMenu(fileName = "CharacterSkillBase", menuName = "Skill/CharacterSkillBase")]
public class CharacterSkillBase : ScriptableObject
{
    public ESkillType SkillType;
    public float CooldownTime;
    private Character Owner;

    public virtual void ActivateSkill(Character Owner)
    {
        this.Owner = Owner;
        Animator animator = Owner.gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            animator.Play("CharacterSkill", 0, 0);
            Owner.StartCoroutine(OnAnimationStartPlaying());   
        }

        Owner.StartCoroutine(Owner.StartCooldown());
    }

    void OnAnimationFinish()
    {
        Animator animator = Owner.gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            AnimNotify AnimFinishedNotify = animator.GetBehaviour<AnimNotify>();
            AnimFinishedNotify.OnAnimationFinished -= OnAnimationFinish;
        }
    }

    private IEnumerator OnAnimationStartPlaying()
    {
        yield return null;

        Animator animator = Owner.gameObject.GetComponent<Animator>();
        if (animator != null)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            AnimNotify[] Notifies = animator.GetBehaviours<AnimNotify>();
            foreach (var Notify in Notifies)
            {
                if (Notify.StateHash == stateInfo.fullPathHash)
                {
                    Notify.OnAnimationFinished += OnAnimationFinish;
                    break;
                }
            }
        }
    }
}
