using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;

public class SaveData //���� ������ Ŭ����
{
    public string saveName; //���� ������ �̸�
    public DateTime saveTime; //������ �ð�
    public int mapID; //�� ID

    public Vector2 playerPosition; //�÷��̾� ��ǥ

    public CharacterSaveData[] characterSaveDataArray; //�÷��̾� ĳ���� ���� ������ �迭
    public List<int> itemIDList; //������ ID ����Ʈ
    public int money; //���� ��


    public SaveData() //�� �������� ������ �� ������
    {
        saveName = string.Empty;
        saveTime = DateTime.Now;
        mapID = 100;
       
        playerPosition = new Vector2(-0.5f, 0.5f);
		characterSaveDataArray = new CharacterSaveData[1];

        

		Dictionary<int, CharacterData> characterDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, CharacterData>>
			(Resources.Load<TextAsset>("Data/CharacterData").text);

        characterSaveDataArray[0].characterID = 100;
        characterSaveDataArray[0].currentHealth = characterDataDictionary[characterSaveDataArray[0].characterID].defaultMaxHealth;
        characterSaveDataArray[0].armorID = 0;
        characterSaveDataArray[0].weaponID = 0;

        /*
		characterSaveDataArray[1].characterID = 101;
		characterSaveDataArray[1].currentHealth = CharacterDataDictionary[characterSaveDataArray[1].characterID].defaultMaxHealth;
		characterSaveDataArray[1].ArmorID = 0;
		characterSaveDataArray[1].weaponID = 0;

		characterSaveDataArray[2].characterID = 102;
		characterSaveDataArray[2].currentHealth = CharacterDataDictionary[characterSaveDataArray[2].characterID].defaultMaxHealth;
		characterSaveDataArray[2].ArmorID = 0;
		characterSaveDataArray[2].weaponID = 0;
        */


        itemIDList = new List<int>();

        money = 0;
	}
}

public struct CharacterSaveData //ĳ���� ���� ������
{
    public int characterID; //ĳ���� ID
    public double currentHealth; //���� ü��
    public int weaponID; //���� ID
	public int armorID; //���� ID
}




