using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct ItemData //아이템 데이터 구조체
{
	public readonly string itemName; //아이템 이름
	public readonly string itemDescription; //아이템 설명
	public readonly int itemPrice; //아이템 가격
	public readonly double attackPower; //아이템 공격력 : 무기가 아니면 0
	public readonly double defencePower; //아이템 방어력 : 갑옷이 아니면 0
	public readonly double dodgeProbability; //아이템 회피 가능성
	public readonly double turnSpeed; //아이템 턴 속도
	public readonly double criticalProbability; //아이템 치명타 가능성
	public readonly double health; //아이템 체력 : 먹는거가 아니면 0
	public readonly string itemSpriteLocation; //아이템 스프라이트 위치
	public readonly itemTag itemTag; //아이템 태그
}

public enum itemTag //아이템 태그
{
	weapon, //무기
	armor, //갑옷
	consumable //먹는거
}
