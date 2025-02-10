using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct ItemData //������ ������ ����ü
{
	public readonly string itemName; //������ �̸�
	public readonly string itemDescription; //������ ����
	public readonly int itemPrice; //������ ����
	public readonly double attackPower; //������ ���ݷ� : ���Ⱑ �ƴϸ� 0
	public readonly double defencePower; //������ ���� : ������ �ƴϸ� 0
	public readonly double dodgeProbability; //������ ȸ�� ���ɼ�
	public readonly double turnSpeed; //������ �� �ӵ�
	public readonly double criticalProbability; //������ ġ��Ÿ ���ɼ�
	public readonly double health; //������ ü�� : �Դ°Ű� �ƴϸ� 0
	public readonly string itemSpriteLocation; //������ ��������Ʈ ��ġ
	public readonly itemTag itemTag; //������ �±�
}

public enum itemTag //������ �±�
{
	weapon, //����
	armor, //����
	consumable //�Դ°�
}
