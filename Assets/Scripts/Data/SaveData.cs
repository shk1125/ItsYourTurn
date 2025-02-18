using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;

public class SaveData //저장 데이터 클래스
{
    public string saveName; //저장 데이터 이름
    public DateTime saveTime; //저장한 시각
    public int mapID; //맵 ID

    public Vector2 playerPosition; //플레이어 좌표

    public CharacterSaveData[] characterSaveDataArray; //플레이어 캐릭터 저장 데이터 배열
    public List<int> itemIDList; //아이템 ID 리스트
    public int money; //가진 돈


    public SaveData() //새 게임으로 시작할 때 생성자
    {
        saveName = string.Empty;
        saveTime = DateTime.Now;
        mapID = 100;
       
        playerPosition = new Vector2(-0.5f, 0.5f);
		characterSaveDataArray = new CharacterSaveData[3];

        

		Dictionary<int, CharacterData> characterDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, CharacterData>>
			(Resources.Load<TextAsset>("Data/CharacterData").text);

        characterSaveDataArray[0].characterID = 100;
        characterSaveDataArray[0].currentHealth = characterDataDictionary[characterSaveDataArray[0].characterID].defaultMaxHealth;
        characterSaveDataArray[0].armorID = 0;
        characterSaveDataArray[0].weaponID = 0;

        
		characterSaveDataArray[1].characterID = 101;
		characterSaveDataArray[1].currentHealth = characterDataDictionary[characterSaveDataArray[1].characterID].defaultMaxHealth;
		characterSaveDataArray[1].armorID = 0;
		characterSaveDataArray[1].weaponID = 0;

		characterSaveDataArray[2].characterID = 102;
		characterSaveDataArray[2].currentHealth = characterDataDictionary[characterSaveDataArray[2].characterID].defaultMaxHealth;
		characterSaveDataArray[2].armorID = 0;
		characterSaveDataArray[2].weaponID = 0;
        


        itemIDList = new List<int>();

        money = 5000;
	}
}

public struct CharacterSaveData //캐릭터 저장 데이터
{
    public int characterID; //캐릭터 ID
    public double currentHealth; //현재 체력
    public int weaponID; //무기 ID
	public int armorID; //갑옷 ID
}




