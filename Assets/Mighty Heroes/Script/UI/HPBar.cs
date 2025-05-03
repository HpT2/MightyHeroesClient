using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HPBar : MonoBehaviour
{
    Character Owner;
    public Image HPBarImg;
    public Image HPEffectImg;
    public TextMeshProUGUI Label;
    public TextMeshProUGUI DamageText;

    Coroutine HPDecreaseRoutine;
    Coroutine DamageEffectRoutine;

    public Image ExpBar;

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Vector3.up);
    }

    public void Init(Character Character)
    {
        Owner = Character;
        Owner.IngamePlayerStat.OnStatChange += OnHPChange;
        Owner.OnGetDamage += OnDamaged;
        OnHPChange(StatName.HP);
        if(Owner.gameObject.tag == "Player")
        {
            Owner.OnLevelUp += OnLevelUp;
            OnLevelUp(1);
            Owner.OnGainExp += OnGainExp;
        }
        else
        {
            ExpBar.gameObject.SetActive(false);
            Label.gameObject.SetActive(false);
        }
    }

    void OnGainExp(float CurrentExp, float MaxExp)
    {
        ExpBar.fillAmount = CurrentExp / MaxExp;
    }

    public void OnLevelUp(int NewLevel)
    {
        Label.text = Owner.photonView.Owner.NickName + " - Lv." + NewLevel;
        Owner.IngamePlayerStat.LevelUp(NewLevel, Owner.BasePlayerStat);
    }

    void OnHPChange(StatName stat)
    {
        if(stat == StatName.HP)
        {
            if(HPDecreaseRoutine != null)
            {
                StopCoroutine(HPDecreaseRoutine);
            }
            float CurHP = Owner.IngamePlayerStat.GetTotalStat(stat);
            float MaxHP = Owner.BasePlayerStat.GetTotalStat(stat);

            float Percent = CurHP / MaxHP;
            if (Percent < HPBarImg.fillAmount)
            {
                HPDecreaseRoutine = StartCoroutine(HPDecreaseEffect(CurHP / MaxHP));
            }
            else
            {
                HPBarImg.fillAmount = HPEffectImg.fillAmount = Percent;
            }
        }
    }

    void OnDamaged(Character Char, float Damage)
    {
        if(DamageEffectRoutine != null)
        {
            StopCoroutine(DamageEffectRoutine);
        }
        DamageText.text = Damage.ToString();
        Color color = DamageText.color;
        color.a = 1.0f;
        DamageText.color = color;
        DamageEffectRoutine = StartCoroutine(DamageTextMove());
    }

    IEnumerator HPDecreaseEffect(float TargetPercent)
    {
        HPBarImg.fillAmount = TargetPercent;
        yield return new WaitForSeconds(0.25f);
        float CurrentPercent = HPEffectImg.fillAmount;
        float Speed = (CurrentPercent - TargetPercent);
        while (HPEffectImg.fillAmount > TargetPercent)
        {
            HPEffectImg.fillAmount -= Speed * Time.deltaTime;
            yield return null;
        }
        HPDecreaseRoutine = null;
    }

    IEnumerator DamageTextMove()
    {
        DamageText.transform.localPosition = new Vector3(0, 0.5f, 0);
        while(DamageText.transform.localPosition.y < 1.0f)
        {
            DamageText.transform.localPosition += Vector3.up * 4.0f * Time.deltaTime;
            yield return null;
        }

        DamageEffectRoutine = StartCoroutine(DamageTextFade());
        yield return null;
    }

    IEnumerator DamageTextFade()
    {
        yield return new WaitForSeconds(0.5f);
        while(DamageText.color.a > 0.0f)
        {
            Color color = DamageText.color;
            color.a -= 2 * Time.deltaTime;
            DamageText.color = color;
            yield return null;
        }
        DamageEffectRoutine = null;
        yield return null;
    }
}
