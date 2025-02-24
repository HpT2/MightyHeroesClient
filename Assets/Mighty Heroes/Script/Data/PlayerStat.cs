using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum StatName
{
    HP,
    DEF,
    ATK,
    MOVE_SPD,
    CRIT_RATE,
    CRIT_DMG,
}

public delegate void OnStatChange_Delegate(StatName Name);

public class PlayerStat
{
    public Dictionary<StatName, float> BaseStat = new Dictionary<StatName, float>
    {
        {StatName.HP, 0},
        {StatName.DEF, 0},
        {StatName.ATK, 0},
        {StatName.MOVE_SPD, 0},
        {StatName.CRIT_RATE, 0},
        {StatName.CRIT_DMG, 0}
    };

    public Dictionary<StatName, List<Dictionary<string, float>>> AdditionalStatByEquipments = new Dictionary<StatName, List<Dictionary<string, float>>>
    {
        {StatName.HP, new List<Dictionary<string, float>>() },
        {StatName.DEF, new List<Dictionary<string, float>>() },
        {StatName.ATK, new List<Dictionary<string, float>>() },
        {StatName.MOVE_SPD, new List<Dictionary<string, float>>() },
        {StatName.CRIT_RATE, new List<Dictionary<string, float>>() },
        {StatName.CRIT_DMG, new List<Dictionary<string, float>>() },

    };
    public Dictionary<StatName, List<Dictionary<string, float>>> TemporaryAdditionalStat = new Dictionary<StatName, List<Dictionary<string, float>>>
    {
        {StatName.HP, new List<Dictionary<string, float>>() },
        {StatName.DEF, new List<Dictionary<string, float>>() },
        {StatName.ATK, new List<Dictionary<string, float>>() },
        {StatName.MOVE_SPD, new List<Dictionary<string, float>>() },
        {StatName.CRIT_RATE, new List<Dictionary<string, float>>() },
        {StatName.CRIT_DMG, new List<Dictionary<string, float>>() },
    };

    public OnStatChange_Delegate OnStatChange;

    public void SetBaseStat(StatName Name, float Value)
    {
        if (BaseStat.ContainsKey(Name))
        {
            BaseStat[Name] = Value;
        }
        else
        {
            BaseStat.Add(Name, Value);
        }

        OnStatChange?.Invoke(Name);
    }

    public float GetBaseStat(StatName Name)
    {
        if(BaseStat.ContainsKey(Name))
        {
            return BaseStat[Name];
        }

        return 0;
    }

    public float GetTotalStat(StatName Name)
    {
        if (BaseStat.ContainsKey(Name))
        {
            float ReturnedValue = BaseStat[Name];

            List<Dictionary<string, float>> EquipmentStat = AdditionalStatByEquipments[Name];
            foreach (var Dict in EquipmentStat)
            {
                foreach(var KVP in Dict)
                {
                    ReturnedValue += KVP.Value;
                }
            }

            List<Dictionary<string, float>> TemporaryStat = TemporaryAdditionalStat[Name];
            foreach (var Dict in TemporaryStat)
            {
                foreach (var KVP in Dict)
                {
                    ReturnedValue += KVP.Value;
                }
            }

            return ReturnedValue;
        }

        return 0;
    }

    public void AddAditionalStat(StatName Name, string AdditionalStatName, float Value, bool IsTemp = false)
    {
        Dictionary<StatName, List<Dictionary<string, float>>> AdditionalStatDict;
        if (IsTemp)
        {
            AdditionalStatDict = TemporaryAdditionalStat;
        }
        else
        {
            AdditionalStatDict = AdditionalStatByEquipments;
        }

        List<Dictionary<string, float>> AdditionalStatList = AdditionalStatDict[Name];
        foreach(var Dict in AdditionalStatList)
        {
            if(Dict.ContainsKey(AdditionalStatName))
            {
                Dict[AdditionalStatName] = Value;
                OnStatChange?.Invoke(Name);
                return;
            }
        }
        AdditionalStatList.Add(new Dictionary<string, float> { { AdditionalStatName, Value } });

        OnStatChange?.Invoke(Name);
    }
}
