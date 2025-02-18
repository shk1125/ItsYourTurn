using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using static GameManager;
using TMPro;
using UnityEditor.U2D.Aseprite;

public class BattleManager : MonoBehaviour //��Ʋ�Ŵ��� Ŭ����
{
	public Vector3[] characterPositionArray; //ĳ���� ������Ʈ ��ġ ��ǥ : �ν����Ϳ��� ���
	public static BattleManager instance = null; //BattleManager �ν��Ͻ�
	public SpriteRenderer battleSceneBackgroundSpriteRenderer; //���� �� ��� �̹����� SpriteRenderer
	public GameObject characterPrefab; //ĳ���� �⺻ Prefab : ���� �� ��������Ʈ�� �����͸� ���� ����
	public GameObject playerCharacterHighLight; //�÷��̾� ĳ���� �ٴڿ� ��ġ�ϴ� ���̶���Ʈ : �ν����Ϳ��� ���. ���� ���� �÷��̾� ĳ���� Ȥ�� Ÿ���õ� �÷��̾� ĳ����
	public GameObject enemyCharacterHighLight; //�� ĳ���� �ٴڿ� ��ġ�ϴ� ���̶���Ʈ : �ν����Ϳ��� ���. Ÿ���õ� �� ĳ���� Ȥ�� ���� ���� �� ĳ����


	#region Data Dictionary Variables
	private Dictionary<int, CharacterData> characterDataDictionary; //ĳ���� ������ ��ųʸ�
	private Dictionary<int, ItemData> itemDataDictionary; //������ ������ ��ųʸ�
	private Dictionary<int, SkillData> skillDataDictionary; //��ų ������ ��ųʸ�
 	private Dictionary<int, Sprite> skillSpriteDictionary; //��ų ��������Ʈ ��ųʸ� : ��������Ʈ�� Resources ������ ����Ǿ� �ִµ� ���� �ٲ� ������ �Ź� ȣ���ϸ� �޸� �����̱� ������ ��ųʸ��� �����ص�
	#endregion

	private List<CharacterManager> characterManagerList; //ĳ���� �Ŵ��� ��ũ��Ʈ ����Ʈ
	private Queue<CharacterManager> turnQueue; //�� ���� ť : ĳ���� �Ŵ��� ��ũ��Ʈ�� ť�� ��ҷ� ���


	private CharacterManager currentPlayerCharacterManager; //���� ���� �÷��̾� ĳ���� �Ŵ��� ��ũ��Ʈ Ȥ�� ���� ���� Ÿ������ �÷��̾� �Ŵ��� ��ũ��Ʈ
	private CharacterManager currentEnemyCharacterManager; //���� ���� �� ĳ���� �Ŵ��� ��ũ��Ʈ Ȥ�� ���� �÷��̾ Ÿ������ �� �Ŵ��� ��ũ��Ʈ
	private SkillData skillData; //��ų ������ ����ü : �� ĳ���� AI �Ŵ��� �޼ҵ忡�� ��ų Ÿ���� ������ Ȯ���ϱ� ������ ���� ������ �����

	#region Player Character Panel Objects and Variables

	[Header("Player Character Panel Objects and Variables")]
	public GameObject playerCharacterPanel; //�÷��̾� ĳ���� �г� : �÷��̾� ĳ������ �Ͽ��� ĳ������ ������ ����� �� �ִ� �ൿ�� ǥ�õ�
	public TextMeshProUGUI characterNameText; //ĳ���� �̸� �ؽ�Ʈ
	public TextMeshProUGUI characterStatText; //ĳ���� ���� �ؽ�Ʈ
	public Image defaultAttackButtonImage; //�⺻ ���� ��ư �̹���
	public Image defaultActiveSkill01ButtonImage; //1�� ��ų ��ư �̹���
	public Image defaultActiveSkill02ButtonImage; //2�� ��ų ��ư �̹���
	public Button defaultActiveSkill01Button; //1�� ��ų ��ư
	public Button defaultActiveSkill02Button; //2�� ��ų ��ư

	#endregion

	private void Awake()
	{
		if (instance == null) //�ν��Ͻ��� ���� ��
		{
			instance = this; //��Ʋ�Ŵ��� �ν��Ͻ� ���
		}
		else
		{
			if (instance != this) //�ν��Ͻ��� �̹� �ִµ� �ڽ��� �ƴ� ��
				Destroy(this.gameObject); //�ν��Ͻ� ����
		}

		Application.targetFrameRate = 60; //Ÿ�� �������� 60

		GenerateBattle(); //���� �˰��� ����
		GenerateTurn(); //�� �˰��� ����
	}


	private void Update()
	{
		if (Input.GetMouseButton(0)) //���콺 ���� Ŭ��
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //���� ���콺 Ŭ�� ��ġ ����
			RaycastHit2D mouseHit = Physics2D.Raycast(mousePosition, Vector2.zero, 0f); //���콺 Ŭ�� ��ġ�� ����ĳ��Ʈ
			if (mouseHit.collider != null) //����ĳ��Ʈ�� �ݶ��̴��� ����� ���
			{
				if (!mouseHit.collider.gameObject.GetComponent<CharacterManager>().GetCharacterData().isPlayerCharacter && //����ĳ��Ʈ ������Ʈ�� �÷��̾� ĳ���Ͱ� �ƴ� ���
					turnQueue.Peek().GetCharacterData().isPlayerCharacter) //���� ���� �÷��̾� ĳ������ ���
				{
					currentEnemyCharacterManager = mouseHit.collider.gameObject.GetComponent<CharacterManager>(); //�� ĳ���� Ÿ���� ����
					enemyCharacterHighLight.transform.position = mouseHit.collider.transform.position; //�� ĳ���� ���̶���Ʈ�� Ÿ������ ������ �̵�
				}
			}
		}
	}


	private void GenerateBattle() //���� �˰��� ���� �޼ҵ�
	{
		battleSceneBackgroundSpriteRenderer.sprite = Resources.Load<Sprite>(BattleDataHolder.battleSceneBackgroundSpriteLocation); //���� �� ��� �̹��� Resources �������� ȣ�� �� ���� :
																																   //BattleDataHolder Ŭ������ ��������Ʈ ��ġ ���� �̿�


		characterDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, CharacterData>> //ĳ���� ������ ��ųʸ� ����
			(Resources.Load<TextAsset>("Data/CharacterData").text);
		itemDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, ItemData>> //������ ������ ��ųʸ� ����
			(Resources.Load<TextAsset>("Data/ItemData").text);
		skillDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, SkillData>> //��ų ������ ��ųʸ� ����
			(Resources.Load<TextAsset>("Data/SkillData").text);

		skillSpriteDictionary = new Dictionary<int, Sprite>(); //��ų ��������Ʈ ��ųʸ� �ʱ�ȭ

		foreach (KeyValuePair<int, SkillData> items in skillDataDictionary) //��ų ������ ��ųʸ� �ݺ���
		{
			skillSpriteDictionary.Add(items.Key, Resources.Load<Sprite>(items.Value.skillSpriteLocation)); //��ų ��������Ʈ ��ųʸ��� ��ų ID�� Key��, ��ų ��������Ʈ ��ġ ������ Value�� ����
		}



		GameObject character; //ĳ���� ������Ʈ ����
		CharacterManager characterManager; //ĳ���� �Ŵ��� ����

		characterManagerList = new List<CharacterManager>(); //ĳ���� �Ŵ��� ����Ʈ �ʱ�ȭ
		for (int i = 0; i < BattleDataHolder.characterSaveDataArray.Length; i++) //BattleDataHolder Ŭ������ ĳ���� ���� ������ �迭 �ݺ���
		{
			character = Instantiate(characterPrefab); //ĳ���� Prefab ����
			character.transform.position = characterPositionArray[i]; //������ ĳ���� position ����
			characterManager = character.transform.GetComponent<CharacterManager>(); //ĳ���� �Ŵ��� ����
			characterManager.SetCharacterData(characterDataDictionary[BattleDataHolder.characterSaveDataArray[i].characterID]); //ĳ���� ������ ���
			characterManager.SetCharacterSaveData(BattleDataHolder.characterSaveDataArray[i]); //ĳ���� ���� ������ ��� : ĳ���Ͱ� ���� �ſ� �����ϴ� ������ ü���̳� �������� ���� ������
			characterManagerList.Add(characterManager); //ĳ���� �Ŵ��� ����Ʈ�� ĳ���� �Ŵ��� �߰�
		}

		int enemyNum; //�� ���� �ʱ�ȭ
		enemyNum = Random.Range(1, 4); //���� 1~3 �� �߿� �������� ���ڰ� ������. Ư�� �� Ÿ���� ���ڰ� �����Ǿ�� �� ��� �����Ϳ� ���ǹ��� �߰��� ����
		int enemyID; //�� ID �ʱ�ȭ
		for (int i = 0; i < enemyNum; i++) //������ �� ���� �ݺ���
		{
			enemyID = BattleDataHolder.enemyCharacterIDArray[Random.Range(0, BattleDataHolder.enemyCharacterIDArray.Length)]; //Ÿ�Ͽ� ��ϵ� ID �迭���� �������� ID�� �̾Ƽ� ����
			character = Instantiate(characterPrefab); //ĳ���� ������Ʈ ����
			character.transform.position = characterPositionArray[i + 3]; //�� position�� �÷��̾� position �迭���� 3,4,5���̱� ������ i�� 3�� ���ؼ� ���� 
			characterManager = character.transform.GetComponent<CharacterManager>(); //ĳ���� �Ŵ��� ����
			characterManager.SetCharacterData(characterDataDictionary[BattleDataHolder.enemyCharacterIDArray[enemyID]]); //ĳ���� ������ ���
			characterManagerList.Add(characterManager); //ĳ���� �Ŵ��� ����Ʈ�� ĳ���� �Ŵ��� �߰�
		}

	}


	#region Turn Methods

	private void GenerateTurn() //�� ���� ť ���� �޼ҵ�
	{
		turnQueue = new Queue<CharacterManager>(); //�� ���� ť �ʱ�ȭ

		CharacterManager characterManager; //ĳ���� �Ŵ��� ����

		characterManager = characterManagerList[0]; //ĳ���� �Ŵ��� ����Ʈ 0������ ĳ���� �Ŵ��� ����

		while (true) //break�� ȣ����� ������ ���� �ݺ�
		{
			for (int i = 1; i < characterManagerList.Count; i++) //ĳ���� �Ŵ��� ����Ʈ �ݺ���
			{
				if (characterManager.GetTurnSpeed() <= characterManagerList[i].GetTurnSpeed()) //���� ĳ���� �Ŵ����� �� �ӵ����� ĳ���� �Ŵ��� ����Ʈ ����� �� �ӵ����� ���� ��� Ȥ�� �� �ӵ��� ���� ���
				{
					characterManager = characterManagerList[i]; //ĳ���� �Ŵ��� ����Ʈ ��ҷ� ĳ���� �Ŵ��� ���� : �� �� �ӵ��� �����ų� ���� ������� ť�� ������� ����
				}
			}
			turnQueue.Enqueue(characterManager); //�� ���� ť�� ĳ���� �Ŵ��� �߰�
			characterManager.AddTurnSpeed(-characterManager.GetTurnSpeed()); //ĳ���� �Ŵ����� �� �ӵ� ���� :
																			 //ť�� �������� �� �������� �� �ӵ��� ���� ĳ���� �Ŵ����� ť�� ������ ���� �� �ӵ��� ȸ���� ������ �ļ����� �̷����� ��

			if (turnQueue.Count == 10) //�� ���� ť�� 10���� ĳ���� �Ŵ����� �߰����� ��
			{
				break; //�ݺ��� Ż��
			}
		}
		NextTurn(); //���� �� �޼ҵ� ȣ��
	}

	private void AddTurn() //�� �߰� �޼ҵ�
	{
		CharacterManager characterManager; //ĳ���� �Ŵ��� ����
		characterManager = characterManagerList[0]; //ĳ���� �Ŵ��� ����Ʈ 0������ ĳ���� �Ŵ��� ����
		for (int i = 1; i < characterManagerList.Count; i++) //ĳ���� �Ŵ��� ����Ʈ �ݺ���
		{
			if (characterManager.GetTurnSpeed() <= characterManagerList[i].GetTurnSpeed()) //���� ĳ���� �Ŵ����� �� �ӵ����� ĳ���� �Ŵ��� ����Ʈ ����� �� �ӵ����� ���� ��� Ȥ�� �� �ӵ��� ���� ���
            {
				characterManager = characterManagerList[i]; //ĳ���� �Ŵ��� ����Ʈ ��ҷ� ĳ���� �Ŵ��� ���� : �� �� �ӵ��� �����ų� ���� ������� ť�� ������� ����
            }
		}
		turnQueue.Enqueue(characterManager); //�� ���� ť�� ĳ���� �Ŵ��� �߰�
		characterManager.AddTurnSpeed(-characterManager.GetTurnSpeed()); //ĳ���� �Ŵ����� �� �ӵ� ���� :
																		 //ť�� �������� �� �������� �� �ӵ��� ���� ĳ���� �Ŵ����� ť�� ������ ���� �� �ӵ��� ȸ���� ������ �ļ����� �̷����� ��

		//�˰����� GenerateTurn �޼ҵ�� ������
	}

	private void RemoveTurn() //�� ���� �޼ҵ�
	{
		turnQueue.Peek().AddTurnSpeed(turnQueue.Peek().GetTurnSpeed()); //���� ���� ĳ���� �Ŵ����� �� �ӵ� ȸ�� : AddTurn �޼ҵ忡�� �� �ӵ��� ���� ������� ���� ť�� ����� ����
		turnQueue.Dequeue(); //���� ���� ĳ���� �Ŵ����� ť ����
		AddTurn(); //�� �߰� �޼ҵ� ȣ��
	}

	private void NextTurn() //���� �� �޼ҵ�
	{
		CharacterManager characterManager = turnQueue.Peek().GetComponent<CharacterManager>(); //�� ���� ť�� ù��° ��ҷ� ĳ���� �Ŵ��� ����
		switch (characterManager.GetCharacterData().isPlayerCharacter) //���� ���� �÷��̾� ĳ�������� �� ĳ�������� Ȯ��
		{
			case true: //�÷��̾� ĳ������ ���
				{
					currentPlayerCharacterManager = characterManager; //���� �÷��̾� ĳ���� �Ŵ��� ����
					playerCharacterHighLight.transform.position = currentPlayerCharacterManager.transform.position; //�÷��̾� ĳ���� ���̶���Ʈ ��ġ ����

					for(int i = 0; i < characterManagerList.Count; i++) //ĳ���� �Ŵ��� ����Ʈ �ݺ���
					{
						if (!characterManagerList[i].GetCharacterData().isPlayerCharacter) //�� ĳ������ ���
						{
							currentEnemyCharacterManager = characterManagerList[i]; //���� �� ĳ���� �Ŵ��� ����
							enemyCharacterHighLight.transform.position = currentEnemyCharacterManager.transform.position; //�� ĳ���� ���̶���Ʈ ��ġ ����
							break; //�ݺ��� Ż�� : �÷��̾��� ���� ���۵� �� ��Ƴ��� �� ���� 1��°�� Ÿ������ ������. ���콺 Ŭ������ Ÿ���� ���� ����
						}
					}
					SetPlayerCharacterPanel(); //�÷��̾� ĳ���� �г� ���� �޼ҵ� ȣ��
					break;
				}
			case false:
				{
					currentEnemyCharacterManager = characterManager; //���� �� ĳ���� �Ŵ��� ����
					enemyCharacterHighLight.transform.position = currentEnemyCharacterManager.transform.position; //�� ĳ���� ���̶���Ʈ ��ġ ����
					for (int i = 0; i < characterManagerList.Count; i++) //ĳ���� �Ŵ��� ����Ʈ �ݺ���
					{
						if (characterManagerList[i].GetCharacterData().isPlayerCharacter) //�÷��̾� ĳ������ ���
						{
							currentPlayerCharacterManager = characterManagerList[i]; //���� �÷��̾� ĳ���� �Ŵ��� ����
							playerCharacterHighLight.transform.position = currentPlayerCharacterManager.transform.position; //�÷��̾� ĳ���� ���̶���Ʈ ��ġ ����
							break; //�ݺ��� Ż�� : ���� ���� ���۵� �� ��Ƴ��� �÷��̾� ���� 1��°�� Ÿ������ ������ : DecideEnemyMovement �޼ҵ忡�� Ÿ���� ���� ����
						}
					}
					DecideEnemyMovement(); //�� ĳ���� AI �޼ҵ� ȣ��
					break;
				}
		}
	}



	#endregion

	#region Player Character Panel Methods


	private void SetPlayerCharacterPanel() //�÷��̾� ĳ���� �г� ���� �޼ҵ�
	{
		characterNameText.text = currentPlayerCharacterManager.GetCharacterData().characterName; //���� ĳ���� �̸� �ؽ�Ʈ ����
		characterStatText.text = $"ü�� : {currentPlayerCharacterManager.GetCurrentHealth()} / {currentPlayerCharacterManager.GetCharacterData().defaultMaxHealth}\n" + //���� ĳ���� ü�� �ؽ�Ʈ ����
			$"���ݷ� : {currentPlayerCharacterManager.GetCharacterData().defaultAttackPower} + {itemDataDictionary[currentPlayerCharacterManager.GetCharacterSaveData().weaponID].attackPower}\n" + //���� ĳ���� ���ݷ� �ؽ�Ʈ ����
			$"���� : {currentPlayerCharacterManager.GetCharacterData().defaultDefencePower} + {itemDataDictionary[currentPlayerCharacterManager.GetCharacterSaveData().armorID].defencePower}"; //���� ĳ���� ���� �ؽ�Ʈ ����
		defaultActiveSkill01ButtonImage.sprite = skillSpriteDictionary[currentPlayerCharacterManager.GetCharacterData().defaultActiveSkill01ID]; //1�� ��ų ��������Ʈ ����
		defaultActiveSkill02ButtonImage.sprite = skillSpriteDictionary[currentPlayerCharacterManager.GetCharacterData().defaultActiveSkill02ID]; //2�� ��ų ��������Ʈ ����

		if(!playerCharacterPanel.activeSelf) //�÷��̾� ĳ���� �г��� ��Ȱ��ȭ �Ǿ�������
		{
			playerCharacterPanel.SetActive(true); //�÷��̾� ĳ���� �г� Ȱ��ȭ
		}


		switch (currentPlayerCharacterManager.GetDefaultSkill01Turn()) //1�� ��ų�� ��� �� �� Ȯ��
		{
			case 0: //��� �� ���� 0�� ���
				{
					defaultActiveSkill01Button.interactable = true; //1�� ��ų ��� ����
					break;
				}
			default:
				{
					defaultActiveSkill01Button.interactable = false; //1�� ��ų ��� �Ұ���
					break;
				}
		}

		switch (currentPlayerCharacterManager.GetDefaultSkill02Turn()) //2�� ��ų�� ��� �� �� Ȯ��
		{
			case 0: //��� �� ���� 0�� ���
				{
					defaultActiveSkill02Button.interactable = true; //2�� ��ų ��� ����
					break;
				}
			default:
				{
					defaultActiveSkill02Button.interactable = false; //2�� ��ų ��� �Ұ���
					break;
				}
		}


	}

	public void OnClickDefaultAttack() //�⺻ ���� �޼ҵ�
	{


		OnClickEndTurn();
	}

	public void OnClickDefaultSkill01() //1�� ��ų �޼ҵ�
	{


		OnClickEndTurn();
	}

	public void OnClickDefaultSkill02() //2�� ��ų �޼ҵ�
	{


		OnClickEndTurn();
	}

	public void OnClickItem() //�Һ� ������ �޼ҵ�
	{
		//�Һ� �����۸� ������� �Ѵ�
	}

	public void OnClickEndTurn() //�� ���� �޼ҵ�
	{
		if (playerCharacterPanel.activeSelf) //�÷��̾� ĳ���� �г��� Ȱ��ȭ �Ǿ�������
		{
			playerCharacterPanel.SetActive(false); //�÷��̾� ĳ���� �г� ��Ȱ��ȭ : ���� ���� ���¿��� �÷��̾��� Ŭ���� ����
		}
		NextTurn(); //���� �� �޼ҵ� ȣ��
	}
	#endregion

	#region Enemy Methods

	private void DecideEnemyMovement() //�� AI �޼ҵ�
	{

		for (int i = 0; i < characterManagerList.Count; i++) //ĳ���� �Ŵ��� ����Ʈ �ݺ���
		{
			if(currentPlayerCharacterManager.GetCurrentHealth() / currentPlayerCharacterManager.GetCharacterData().defaultMaxHealth > //���� �÷��̾� ĳ������ �����ִ� ü�� ������
				characterManagerList[i].GetCurrentHealth() / characterManagerList[i].GetCharacterData().defaultMaxHealth && //ĳ���� �Ŵ��� ����Ʈ ����� �����ִ� ü�� �������� ����
				characterManagerList[i].GetCharacterData().isPlayerCharacter) // �÷��̾� ĳ���� �Ŵ������ : �� �� AI�� ���� ü�� ������ ���� ���� �÷��̾� ĳ���͸� �ֿ켱���� Ÿ������ ����
			{
				currentPlayerCharacterManager = characterManagerList[i]; //���� �÷��̾� ĳ���� �Ŵ����� ĳ���� �Ŵ��� ����Ʈ ��� ����
				playerCharacterHighLight.transform.position = currentPlayerCharacterManager.transform.position; //�÷��̾� ĳ���� ���̶���Ʈ ��ġ ����
			}
		}



		//�� AI�� �ൿ �˰����� �Ѳ����� �����Ѵ�.
		//�� AI�� ���� ��� ������ �ൿ�� �������� 1�� ��ų, 2�� ��ų, �⺻ ���� ������ ����� ���̴�.
		//�� AI�� ���� �ڽ��� ü�� ������ 50% �̻����� Ȯ���Ѵ�.
		//���� ü�� ������ 50% �̻��� ��� �ൿ �켱 ������ ���� > ����� > ���� > ȸ�� > �⺻ ���� ���̴�.
		//���� ü�� ������ 50% �̻��� �� ȸ�� ��ų�� ȸ������ ����Ͽ� ���� ü�� + ȸ������ �ִ� ü���� �ʰ����� ���� ���� ����Ѵ�.
		//���� ü�� ������ 50% �̸��� ��� �ൿ �켱 ������ ȸ�� > ��� > ����� > ���� > ���� > �⺻ ���� ���̴�.
		if (currentEnemyCharacterManager.GetCurrentHealth() >= (currentEnemyCharacterManager.GetCharacterData().defaultMaxHealth / 2))
		{
			switch (currentEnemyCharacterManager.GetDefaultSkill01Turn())
			{
				case 0:
					{
						skillData = skillDataDictionary[currentEnemyCharacterManager.GetCharacterData().defaultActiveSkill01ID];
						switch (skillData.skillType)
						{
							case SkillType.Buff:
								{
									//���� ��ų
									break;
								}
							default:
								{
									switch (skillData.skillType)
									{
										case SkillType.Debuff:
											{
												//����� ��ų
												break;
											}
										default:
											{
												switch (skillData.skillType)
												{
													case SkillType.Attack:
														{
															//���� ��ų
															break;
														}
													default:
														{
															switch (skillData.skillType)
															{
																case SkillType.Heal:
																	{
																		if (currentEnemyCharacterManager.GetCharacterData().defaultMaxHealth - currentEnemyCharacterManager.GetCurrentHealth() > skillData.skillPower * skillData.skillCoefficient)
																		{
																			//ȸ�� ��ų
																		}
																		else
																		{
																			//�⺻ ����
																		}
																		break;
																	}
																default:
																	{
																		//�⺻ ����
																		break;
																	}
															}
															break;
														}
												}
												break;
											}
									}
									break;
								}
						}


						break;
					}
				default:
					{
						switch (currentEnemyCharacterManager.GetDefaultSkill02Turn())
						{
							case 0:
								{
									skillData = skillDataDictionary[currentEnemyCharacterManager.GetCharacterData().defaultActiveSkill02ID];
									switch (skillData.skillType)
									{
										case SkillType.Buff:
											{
												//���� ��ų
												break;
											}
										default:
											{
												switch (skillData.skillType)
												{
													case SkillType.Debuff:
														{
															//����� ��ų
															break;
														}
													default:
														{
															switch (skillData.skillType)
															{
																case SkillType.Attack:
																	{
																		//���� ��ų
																		break;
																	}
																default:
																	{
																		switch (skillData.skillType)
																		{
																			case SkillType.Heal:
																				{
																					if (currentEnemyCharacterManager.GetCharacterData().defaultMaxHealth - currentEnemyCharacterManager.GetCurrentHealth() > skillData.skillPower * skillData.skillCoefficient)
																					{
																						//ȸ�� ��ų
																					}
																					else
																					{
																						//�⺻ ����
																					}
																					break;
																				}
																			default:
																				{
																					//�⺻ ����
																					break;
																				}
																		}
																		break;
																	}
															}
															break;
														}
												}
												break;
											}
									}

									break;
								}
							default:
								{
									//�⺻ ����
									break;
								}
						}

						break;
					}
			}
		}
		else
		{
			switch(currentEnemyCharacterManager.GetDefaultSkill01Turn())
			{
				case 0:
					{
						skillData = skillDataDictionary[currentEnemyCharacterManager.GetCharacterData().defaultActiveSkill01ID];

						switch(skillData.skillType)
						{
							case SkillType.Heal:
								{
									//ȸ�� ��ų
									break;
								}
								default:
								{
									switch(skillData.skillType)
									{
										case SkillType.Defence:
											{
												//��� ��ų
												break;
											}
										default:
											{
												switch(skillData.skillType)
												{
													case SkillType.Debuff:
														{
															//����� ��ų
															break;
														}
													default:
														{
															switch(skillData.skillType)
															{
																case SkillType.Buff:
																	{
																		//���� ��ų
																		break;
																	}
																default:
																	{
																		switch(skillData.skillType)
																		{
																			case SkillType.Attack:
																				{
																					//���� ��ų
																					break;
																				}
																			default:
																				{
																					//�⺻ ����
																					break;
																				}
																				
																		}
																		break;
																	}
															}
															break;
														}
												}
												break;
											}
									}
									break;
								}
						}
						break;
					}
				default:
					{
						switch (currentEnemyCharacterManager.GetDefaultSkill02Turn())
						{
							case 0:
								{
									skillData = skillDataDictionary[currentEnemyCharacterManager.GetCharacterData().defaultActiveSkill01ID];

									switch (skillData.skillType)
									{
										case SkillType.Heal:
											{
												//ȸ�� ��ų
												break;
											}
										default:
											{
												switch (skillData.skillType)
												{
													case SkillType.Defence:
														{
															//��� ��ų
															break;
														}
													default:
														{
															switch (skillData.skillType)
															{
																case SkillType.Debuff:
																	{
																		//����� ��ų
																		break;
																	}
																default:
																	{
																		switch (skillData.skillType)
																		{
																			case SkillType.Buff:
																				{
																					//���� ��ų
																					break;
																				}
																			default:
																				{
																					switch (skillData.skillType)
																					{
																						case SkillType.Attack:
																							{
																								//���� ��ų
																								break;
																							}
																						default:
																							{
																								//�⺻ ����
																								break;
																							}

																					}
																					break;
																				}
																		}
																		break;
																	}
															}
															break;
														}
												}
												break;
											}
									}
									break;
								}
							default:
								{
									//�⺻ ����
									break;
								}

						}



						break;
					}
			}
		}
	}

	#endregion
}
