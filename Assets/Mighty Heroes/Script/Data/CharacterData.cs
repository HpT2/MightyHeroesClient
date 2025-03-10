using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StatStruct
{
    public StatName Name;
    public float Value;
}

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData")]
public class CharacterData : ScriptableObject
{
    public List<StatStruct> BaseCharacterStatData;
}
