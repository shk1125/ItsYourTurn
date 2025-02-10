using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterData //ĳ���� ������ ����ü
{
    public string characterName; //ĳ���� �̸�
    public bool isPlayerCharacter; //�÷��̾� ĳ���� ���� : false�� �� ĳ����
    public double defaultMaxHealth; //�⺻ �ִ� ü��
    public double defaultAttackPower; //�⺻ ���ݷ�
    public double defaultDefencePower; //�⺻ ����
    public double defaultDodgeProbability; //�⺻ ȸ�� ���ɼ�
    public int defaultTurnSpeed; //�⺻ �� �ӵ�
    public double defaultCriticalProbability; //�⺻ ġ��Ÿ ���ɼ�
    public int defaultAttackID; //�⺻ ���� ID
    public int defaultActiveSkill01ID; //1�� ��ų ID
	public int defaultActiveSkill02ID; //2�� ��ų ID
    public string characterSpriteLocation; //ĳ���� ��������Ʈ ��ġ

}
