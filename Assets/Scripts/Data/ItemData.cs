using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemData //������ ������ ����ü
{
	public string itemName; //������ �̸�
	public string itemDescription; //������ ����
	public int itemPrice; //������ ����
	public double attackPower; //������ ���ݷ� : ���Ⱑ �ƴϸ� 0
	public double defencePower; //������ ���� : ������ �ƴϸ� 0
	public double dodgeProbability; //������ ȸ�� ���ɼ�
	public double turnSpeed; //������ �� �ӵ�
	public double criticalProbability; //������ ġ��Ÿ ���ɼ�
	public double health; //������ ü�� : �Դ°Ű� �ƴϸ� 0
	public string itemSpriteLocation; //������ ��������Ʈ ��ġ
	public ItemTag itemTag; //������ �±�
}

public enum ItemTag //������ �±�
{
	Weapon, //����
	Armor, //����
	Consumable //�Դ°�
}
