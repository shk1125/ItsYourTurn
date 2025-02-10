using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour //ĳ���� �Ŵ��� Ŭ����
{
    private CharacterData characterData; //ĳ���� ������ ����ü 
	private CharacterSaveData characterSaveData; //ĳ���� ���� ������ ����ü
	private int turnSpeed; //�� �ӵ�
	private double currentHealth; //���� ü��
	private int defaultSkill01Turn; //1�� ��ų ��� �� ��
	private int defaultSkill02Turn; //2�� ��ų ��� �� ��
	private List<SkillData> skillEffectList; //��ų ȿ�� ����Ʈ : ������ ����� ��ų ���� ȿ���� ������
	private List<int> skillDurationTurnList; //��ų ȿ�� ���� �� ����Ʈ : ���� �Ѿ ������ ������ ����Ǿ�� �ϱ� ������ ����Ʈ�� ���� �غ���. ���� ���� ������ ��ų ȿ���� �����

	public void SetCharacterData(CharacterData characterData) //ĳ���� ������ ��� �޼ҵ�
	{
		this.characterData = characterData; //ĳ���� ������ ���
		turnSpeed = characterData.defaultTurnSpeed; //�� �ӵ� �ʱ�ȭ
		defaultSkill01Turn = 0; //1�� ��ų ��� �� �� �ʱ�ȭ
		defaultSkill02Turn = 0; //2�� ��ų ��� �� �� �ʱ�ȭ
		skillEffectList = new List<SkillData>(); //��ų ȿ�� ����Ʈ �ʱ�ȭ
		skillDurationTurnList = new List<int>(); //��ų ȿ�� ���� �� ����Ʈ �ʱ�ȭ
	}

	public void SetCharacterSaveData(CharacterSaveData characterSaveData) //ĳ���� ���� ������ ��� �޼ҵ�
	{
		this.characterSaveData = characterSaveData; //ĳ���� ���� ������ ���
		currentHealth = characterSaveData.currentHealth; //���� ü�� ���� : ü���� 100%�� �ƴ� ���¿��� ���� �ſ� ���� �� �ֱ� ����
	}

	public void AddTurnSpeed(int turnSpeed) //�� �ӵ� ȸ�� �޼ҵ� : ĳ������ ���� ������ ���� ť�� �߰��� �� �� �ӵ��� �����ؾ� ��
	{
		this.turnSpeed += turnSpeed; //�� �ӵ� �߰�
	}

	public CharacterData GetCharacterData() //ĳ���� ������ Getter �޼ҵ�
	{ 
		return characterData; 
	}

	public CharacterSaveData GetCharacterSaveData() //ĳ���� ���� ������ Getter �޼ҵ�
	{
		return characterSaveData;
	}

	public int GetTurnSpeed() //�� �ӵ� Getter �޼ҵ�
	{
		return turnSpeed;
	}

	public void AddCurrentHealth(double power) //ü�� ȸ�� �޼ҵ�
	{
		currentHealth += power;
	}

	public double GetCurrentHealth() //���� ü�� Getter �޼ҵ�
	{
		return currentHealth;
	}

	public int GetDefaultSkill01Turn() //1�� ��ų Getter �޼ҵ�
	{
		return defaultSkill01Turn;
	}

	public void AddDefaultSkill01Turn(int skillTurn) //1�� ��ų ��� �� �߰� �޼ҵ�
	{
		defaultSkill01Turn += skillTurn;
	}

	public int GetDefaultSkill02Turn() //2�� ��ų Getter �޼ҵ�
	{
		return defaultSkill02Turn;
	}

	public void AddDefaultSkill02Turn(int skillTurn) //2�� ��ų ��� �� �߰� �޼ҵ�
	{
		defaultSkill02Turn += skillTurn;
	}

	public void AddSkillEffect(SkillData skillData) //��ų ȿ�� �߰� �޼ҵ�
	{
		skillEffectList.Add(skillData); //��ų ȿ�� ����Ʈ �߰�
		skillDurationTurnList.Add(skillData.skillDurationTurn); //��ų ȿ�� ���� �� ����Ʈ �߰�
	}

	public List<SkillData> GetSkillEffectList() //��ų ȿ�� ����Ʈ Getter �޼ҵ�
	{
		return skillEffectList;
	}

	public void CheckSkillEffect() //��ų ȿ�� ���� �� Ȯ�� �޼ҵ�
	{
		for(int i = 0; i < skillDurationTurnList.Count; i++) //��ų ȿ�� ���� �� ����Ʈ �ݺ���
		{
			skillDurationTurnList[i]--; //��ų ȿ�� ���� �� ����
			if (skillDurationTurnList[i]==0) //��ų ȿ�� ���� ���� 0�� �Ǹ�
			{
				skillDurationTurnList.RemoveAt(i); //��ų ȿ�� ���� �� ����Ʈ���� ��� ����
				skillEffectList.RemoveAt(i); //��ų ȿ�� ����Ʈ���� ��� ����
			}
		}
	}

}
