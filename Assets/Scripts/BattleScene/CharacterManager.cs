using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterManager : MonoBehaviour //캐릭터 매니저 클래스
{
    private CharacterData characterData; //캐릭터 데이터 구조체 
	private CharacterSaveData characterSaveData; //캐릭터 저장 데이터 구조체
	private int turnSpeed; //턴 속도
	private double currentHealth; //현재 체력
	private int defaultSkill01Turn; //1번 스킬 대기 턴 수
	private int defaultSkill02Turn; //2번 스킬 대기 턴 수
	private List<SkillData> skillEffectList; //스킬 효과 리스트 : 버프나 디버프 스킬 등의 효과를 저장함
	private List<int> skillDurationTurnList; //스킬 효과 지속 턴 리스트 : 턴이 넘어갈 때마다 변수가 변경되어야 하기 때문에 리스트를 따로 준비함. 지속 턴이 끝나면 스킬 효과가 사라짐

	public void SetCharacterData(CharacterData characterData) //캐릭터 데이터 등록 메소드
	{
		this.characterData = characterData; //캐릭터 데이터 등록
		turnSpeed = characterData.defaultTurnSpeed; //턴 속도 초기화
		defaultSkill01Turn = 0; //1번 스킬 대기 턴 수 초기화
		defaultSkill02Turn = 0; //2번 스킬 대기 턴 수 초기화
		skillEffectList = new List<SkillData>(); //스킬 효과 리스트 초기화
		skillDurationTurnList = new List<int>(); //스킬 효과 지속 턴 리스트 초기화
	}

	public void SetCharacterSaveData(CharacterSaveData characterSaveData) //캐릭터 저장 데이터 등록 메소드
	{
		this.characterSaveData = characterSaveData; //캐릭터 저장 데이터 등록
		currentHealth = characterSaveData.currentHealth; //현재 체력 저장 : 체력이 100%가 아닌 상태에서 전투 신에 들어올 수 있기 때문
	}

	public void AddTurnSpeed(int turnSpeed) //턴 속도 회복 메소드 : 캐릭터의 턴이 끝나면 다음 큐를 추가할 때 턴 속도를 대조해야 함
	{
		this.turnSpeed += turnSpeed; //턴 속도 추가
	}

	public CharacterData GetCharacterData() //캐릭터 데이터 Getter 메소드
	{ 
		return characterData; 
	}

	public CharacterSaveData GetCharacterSaveData() //캐릭터 저장 데이터 Getter 메소드
	{
		return characterSaveData;
	}

	public int GetTurnSpeed() //턴 속도 Getter 메소드
	{
		return turnSpeed;
	}

	public void AddCurrentHealth(double power) //체력 회복 메소드
	{
		currentHealth += power;
	}

	public double GetCurrentHealth() //현재 체력 Getter 메소드
	{
		return currentHealth;
	}

	public int GetDefaultSkill01Turn() //1번 스킬 Getter 메소드
	{
		return defaultSkill01Turn;
	}

	public void AddDefaultSkill01Turn(int skillTurn) //1번 스킬 대기 턴 추가 메소드
	{
		defaultSkill01Turn += skillTurn;
	}

	public int GetDefaultSkill02Turn() //2번 스킬 Getter 메소드
	{
		return defaultSkill02Turn;
	}

	public void AddDefaultSkill02Turn(int skillTurn) //2번 스킬 대기 턴 추가 메소드
	{
		defaultSkill02Turn += skillTurn;
	}

	public void AddSkillEffect(SkillData skillData) //스킬 효과 추가 메소드
	{
		skillEffectList.Add(skillData); //스킬 효과 리스트 추가
		skillDurationTurnList.Add(skillData.skillDurationTurn); //스킬 효과 지속 턴 리스트 추가
	}

	public List<SkillData> GetSkillEffectList() //스킬 효과 리스트 Getter 메소드
	{
		return skillEffectList;
	}

	public void CheckSkillEffect() //스킬 효과 지속 턴 확인 메소드
	{
		for(int i = 0; i < skillDurationTurnList.Count; i++) //스킬 효과 지속 턴 리스트 반복문
		{
			skillDurationTurnList[i]--; //스킬 효과 지속 턴 감소
			if (skillDurationTurnList[i]==0) //스킬 효과 지속 턴이 0이 되면
			{
				skillDurationTurnList.RemoveAt(i); //스킬 효과 지속 턴 리스트에서 요소 제거
				skillEffectList.RemoveAt(i); //스킬 효과 리스트에서 요소 제거
			}
		}
	}

}
