using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SkillData
{
    public string skillName; //��ų �̸�
    public double skillPower; //��ų ����
    public double skillCoefficient; //��ų ���
    public int skillTurn; //��ų �� : ���뿡 �ʿ��� ��
    public SkillType skillType; //��ų Ÿ��
    public string skillSpriteLocation; //��ų �̹��� ��ġ
    public int skillDurationTurn; //��ų ���� ��
}


public enum SkillType
{
    Attack,
    Defence,
    Heal,
    Buff,
    Debuff
}