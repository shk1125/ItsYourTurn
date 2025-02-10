using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CharacterData //캐릭터 데이터 구조체
{
    public string characterName; //캐릭터 이름
    public bool isPlayerCharacter; //플레이어 캐릭터 여부 : false면 적 캐릭터
    public double defaultMaxHealth; //기본 최대 체력
    public double defaultAttackPower; //기본 공격력
    public double defaultDefencePower; //기본 방어력
    public double defaultDodgeProbability; //기본 회피 가능성
    public int defaultTurnSpeed; //기본 턴 속도
    public double defaultCriticalProbability; //기본 치명타 가능성
    public int defaultAttackID; //기본 공격 ID
    public int defaultActiveSkill01ID; //1번 스킬 ID
	public int defaultActiveSkill02ID; //2번 스킬 ID
    public string characterSpriteLocation; //캐릭터 스프라이트 위치

}
