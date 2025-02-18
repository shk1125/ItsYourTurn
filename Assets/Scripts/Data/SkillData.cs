using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillData
{
    public string skillName; //스킬 이름
    public double skillPower; //스킬 위력
    public double skillCoefficient; //스킬 계수
    public int skillTurn; //스킬 턴 : 재사용에 필요한 턴
    public SkillType skillType; //스킬 타입
    public string skillSpriteLocation; //스킬 이미지 위치
    public int skillDurationTurn; //스킬 지속 턴
}


public enum SkillType
{
    Attack,
    Defence,
    Heal,
    Buff,
    Debuff
}