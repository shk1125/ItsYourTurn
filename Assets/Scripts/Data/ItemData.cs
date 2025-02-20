using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ItemData //아이템 데이터 구조체
{
	public string itemName; //아이템 이름
	public string itemDescription; //아이템 설명
	public int itemPrice; //아이템 가격
	public double attackPower; //아이템 공격력 : 무기가 아니면 0
	public double defencePower; //아이템 방어력 : 갑옷이 아니면 0
	public double dodgeProbability; //아이템 회피 가능성
	public double turnSpeed; //아이템 턴 속도
	public double criticalProbability; //아이템 치명타 가능성
	public double health; //아이템 체력 : 먹는거가 아니면 0
	public string itemSpriteLocation; //아이템 스프라이트 위치
	public ItemTag itemTag; //아이템 태그
}

public enum ItemTag //아이템 태그
{
	Weapon, //무기
	Armor, //갑옷
	Consumable //먹는거
}
