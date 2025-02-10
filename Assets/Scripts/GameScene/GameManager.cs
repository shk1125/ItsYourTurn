using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using Newtonsoft.Json;
using UnityEditor.U2D.Aseprite;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using static UnityEditor.Progress;


public class GameManager : MonoBehaviour
{
	public static GameManager instance = null; //���ӸŴ��� �ν��Ͻ�
	public Transform gridTransform;
	public Transform nodesTransform;
	public GameObject nodePrefab;
	public GameObject playerPrefab;


	#region Loading Panel Objects and Variables

	[Header("Loading Panel Objects and Variables")]
	public GameObject loadingPanel;
	public Image loadingBarImage;

	#endregion

	[Space(10)]

	#region Inventory Panel Objects and Variables
	[Header("Inventory Panel Objects and Variables")]
	public GameObject inventoryPanel;
	public Image characterPortraitImage;
	public Image weaponImage;
	public Image armorImage;
	public TextMeshProUGUI characterNameText;
	public TextMeshProUGUI characterStatText;
	public TextMeshProUGUI inventoryMoneyText;
	public Button nextButton;
	public Button previousButton;
	public Transform inventoryScrollviewContentTransform;
	public GameObject inventoryItemButtonPrefab;

	

	private int currentCharacterNum;
	private double weaponAttackPower;
	private double armorDefencePower;
	private float doubleClickInterval = 0.4f;

	#endregion

	[Space(10)]

	#region Item Info Objects and Variables
	[Header("Item Info Objects and Variables")]
	public GameObject itemInfo;
	public TextMeshProUGUI inventoryItemNameText;
	public TextMeshProUGUI inventoryItemTagText;
	public TextMeshProUGUI inventoryItemDescriptionText;
	public TextMeshProUGUI inventoryItemStatText;
	public TextMeshProUGUI inventoryItemPriceText;

	[SerializeField]
	private Vector2 itemInfoPosition;

	#endregion


	[Space(10)]

	#region Store Panel Objects and Variables
	[Header("Store Panel Objects and Variables")]
	public GameObject storePanel;
	public GameObject storeItemButtonPrefab;
	public Transform storeScrollviewContentTransform;
	public Button buyButton;
	public TextMeshProUGUI storeMoneyText;
	public TextMeshProUGUI storeItemNameText;
	public TextMeshProUGUI storeItemTagText;
	public TextMeshProUGUI storeItemDescriptionText;
	public TextMeshProUGUI storeItemStatText;
	public TextMeshProUGUI storeItemPriceText;

	[SerializeField]
	private int storeItemButtonCount;

	private List<Button> storeItemButtonList;
	private Dictionary<int, StoreData> storeDataDictionary;
	private StoreData storeData;
	private int storeItemID;

	#endregion

	private Dictionary<int, CharacterData> characterDataDictionary;
	private Dictionary<int, ItemData> itemDataDictionary;
	private Dictionary<int, Sprite> itemSpriteDictionary;
	private Dictionary<int, Sprite> characterSpriteDictionary;

	private Transform playerTransform;
	private Dictionary<int, MapData> mapDataDictionary;
	private SpriteRenderer playerSpriteRenderer;
	private Dictionary<Vector3, NodeManager> nodeManagerDictionary;

	

	private void Awake()
	{
		if (instance == null)
		{
			instance = this; //���ӸŴ��� �ν��Ͻ� ���
		}
		else
		{
			if (instance != this)
				Destroy(this.gameObject); //�̹� �ν��Ͻ��� ���� ��� ����
		}


		Application.targetFrameRate = 60; //Ÿ�� �������� 60



		GenerateMap();
		GenerateStorePanel();
		GenerateInventoryPanel();
		
	}

	private void Start()
	{
		
	}

	private void Update()
	{
		switch (Input.inputString)
		{
			case "W":
			case "w":
				{
					MovePlayer(0, 1);
					break;
				}
			case "A":
			case "a":
				{
					MovePlayer(-1, 0);
					break;
				}
			case "S":
			case "s":
				{
					MovePlayer(0, -1);
					break;
				}
			case "D":
			case "d":
				{
					MovePlayer(1, 0);
					break;
				}
			case "I":
			case "i":
				{
					switch (inventoryPanel.activeSelf)
					{
						case true:
							{
								if (itemInfo.activeSelf)
								{
									itemInfo.SetActive(false);
								}
								OnClickCloseInventoryPanel();
								break;
							}
						case false:
							{
								OpenInventoryPanel();
								break;
							}
					}
					break;
				}
			case "E":
			case "e":
				{
					SetStoreUI();
					OpenStorePanel();
					break;
				}
		}

		if(inventoryPanel.activeSelf)
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			RaycastHit2D mouseHit = Physics2D.Raycast(mousePosition, Vector2.zero, 0f);
			if (mouseHit.collider != null && mouseHit.transform.tag == "Item")
			{
				switch(mouseHit.transform.tag)
				{
					case "Item":
						{
							if(!itemInfo.activeSelf)
							{
								SetItemInfo(SaveDataHolder.saveData.itemIDList[mouseHit.transform.GetSiblingIndex()]);
								itemInfo.SetActive(true);
							}

							break;
						}
					default:
						{
							itemInfo.SetActive(false);
							break;
						}
				}
				
			}

			if(itemInfo.activeSelf)
			{
				itemInfo.transform.position = mousePosition + itemInfoPosition;
			}
		}

	}

	private void FixedUpdate()
	{

	}


	#region Player Methods

	private void MovePlayer(int x, int y)
	{
		if(inventoryPanel.activeSelf || storePanel.activeSelf)
		{
			return;
		}

		
		if (y == 0)
		{
			if (x < 0)
			{
				if (!playerSpriteRenderer.flipX)
				{
					playerSpriteRenderer.flipX = true;
				}
			}
			else
			{
				if (playerSpriteRenderer.flipX)
				{
					playerSpriteRenderer.flipX = false;
				}
			}
		}

		if(x > 0 && playerTransform.position.x == 8.5f)
		{
			return;
		}
		else if(x < 0 && playerTransform.position.x == -8.5f)
		{
			return;
		}

		if(y > 0 && playerTransform.position.y == 4.5f)
		{
			return;
		}
		else if(y < 0 && playerTransform.position.y == -4.5f)
		{
			return;
		}


		playerTransform.Translate(x, y, 0);
		nodeManagerDictionary[playerTransform.position].CheckNode();

	}

	#endregion


	#region Map Methods

	public void EncounterEnemy(int[] enemyCharacterIDArray, string battleSceneBackgroundSpriteLocation)
	{

		BattleDataHolder.enemyCharacterIDArray = enemyCharacterIDArray;
		BattleDataHolder.characterSaveDataArray = SaveDataHolder.saveData.characterSaveDataArray;
		BattleDataHolder.itemIDList = SaveDataHolder.saveData.itemIDList;
		BattleDataHolder.battleSceneBackgroundSpriteLocation = battleSceneBackgroundSpriteLocation;

		loadingPanel.SetActive(true);

		StartCoroutine(LoadScene("BattleScene"));
	}

	private void GenerateMap()
	{
		mapDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, MapData>>
			(Resources.Load<TextAsset>("Data/MapData").text);
		

		Instantiate(Resources.Load<GameObject>(mapDataDictionary[SaveDataHolder.saveData.mapID].tilemapLocation))
		   .transform.SetParent(gridTransform);



		GameObject node;
		NodeManager nodeManager = null;
		nodeManagerDictionary = new Dictionary<Vector3, NodeManager>();
		string[] tileIDStringArray = Resources.Load<TextAsset>(mapDataDictionary[SaveDataHolder.saveData.mapID].tileIDArrayLocation).text.Split(',');
		int tileIDStringArrayNum = 0;
		int tileID = 0;
		for (int j = 0; j < 10; j++)
		{
			for (int k = 0; k < 18; k++)
			{
				node = Instantiate(nodePrefab);
				node.transform.position = new Vector2(-8.5f + k, 4.5f - j);
				node.transform.SetParent(nodesTransform);
				nodeManager = node.GetComponent<NodeManager>();
				tileID = int.Parse(tileIDStringArray[tileIDStringArrayNum]);
				nodeManager.SetData(mapDataDictionary[SaveDataHolder.saveData.mapID].tileDataDictionary[tileID],
						mapDataDictionary[SaveDataHolder.saveData.mapID].battleSceneBackgroundSpriteLocation, tileID);
				tileIDStringArrayNum++;
				nodeManagerDictionary.Add(node.transform.position, nodeManager);
			}
		}

		


		playerTransform = Instantiate(playerPrefab).transform;
		playerTransform.position = SaveDataHolder.saveData.playerPosition;
		playerSpriteRenderer = playerTransform.GetComponent<SpriteRenderer>();

	}



	#endregion


	#region Loading Methods

	IEnumerator LoadScene(string scene) //Scene ���� �޼ҵ�
	{
		yield return null;
		AsyncOperation op = SceneManager.LoadSceneAsync(scene); //AsyncOperation ������ GameScene ����
		op.allowSceneActivation = false; //�ε� �������� Scene�� �ε����� �ʵ��� ����
		float timer = 0.0f; //�ð� ���� ����
		while (!op.isDone) //�ε��� ������ �ʾ��� ���
		{
			yield return null;
			timer += Time.deltaTime; //�ð� ���� ����
			if (op.progress < 0.9f) //0.9f ������ �ε��� �Ϸ�� ������ ����
			{
				loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, op.progress, timer); //�ε� ������ Ÿ�̸� ������ ������ �ε� ���� fillAmount�� ǥ��
				if (loadingBarImage.fillAmount >= op.progress) //�ε� ���� fillAmount�� �ε� �������� ���� ���
				{
					timer = 0f; //�ð� ���� �ʱ�ȭ
				}
			}
			else
			{
				loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, 1f, timer); //0.9f �ð� ������ �ε��� �Ϸ�� ������ ���ֵǱ� ������ 1f���� �������� ����
				if (loadingBarImage.fillAmount == 1.0f) //�ε��� �Ϸ�Ǿ��� ��
				{
					op.allowSceneActivation = true; //Scene�� �ε��ϵ��� �㰡
					yield break;
				}
			}
		}
	}




	#endregion


	#region Inventory Panel Methods

	private void GenerateInventoryPanel()
	{
		characterDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, CharacterData>>
			(Resources.Load<TextAsset>("Data/CharacterData").text);
		characterSpriteDictionary = new Dictionary<int, Sprite>();
		foreach (KeyValuePair<int, CharacterData> items in characterDataDictionary)
		{
			characterSpriteDictionary.Add(items.Key, Resources.Load<Sprite>(items.Value.characterSpriteLocation));
		}

		itemDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, ItemData>>
			(Resources.Load<TextAsset>("Data/ItemData").text);
		itemSpriteDictionary = new Dictionary<int, Sprite>();
		foreach (KeyValuePair<int, ItemData> items in itemDataDictionary)
		{
			itemSpriteDictionary.Add(items.Key, Resources.Load<Sprite>(items.Value.itemSpriteLocation));
		}


		if (SaveDataHolder.saveData.characterSaveDataArray.Length < 2)
		{
			nextButton.interactable = false;
		}


		Button itemButton;
		for (int i = 0; i < SaveDataHolder.saveData.itemIDList.Count; i++)
		{
			itemButton = Instantiate(inventoryItemButtonPrefab, inventoryScrollviewContentTransform).GetComponent<Button>();
			itemButton.onClick.AddListener(() => OnClickInventoryItemCoroutine(i, SaveDataHolder.saveData.itemIDList[i]));
		}


		currentCharacterNum = 0;

		inventoryMoneyText.text = $"���� �� : {SaveDataHolder.saveData.money} ���";
		ChangeCharacter(currentCharacterNum);
	}

	public void OnClickNextCharacter()
	{
		currentCharacterNum++;
		ChangeCharacter(currentCharacterNum);
		if(currentCharacterNum == SaveDataHolder.saveData.characterSaveDataArray.Length-1)
		{
			nextButton.interactable = false;
		}
		if(!previousButton.interactable)
		{
			previousButton.interactable = true;
		}
	}

	public void OnClickPreviousCharacter()
	{
		currentCharacterNum--;
		ChangeCharacter(currentCharacterNum);
		if(currentCharacterNum == 0)
		{
			previousButton.interactable= false;
		}
		if(!nextButton.interactable)
		{
			nextButton.interactable = true;
		}
	}


	private IEnumerator OnClickInventoryItemCoroutine(int itemNum, int itemID)
	{
		yield return new WaitForEndOfFrame();

		float clickTime = 0f;
		while (clickTime < doubleClickInterval)
		{
			if (Input.GetMouseButtonDown(0))
			{
				OnClickInventoryItem(itemNum, itemID);
				yield break;
			}
			clickTime += Time.deltaTime;
			yield return null;
		}
	}


	private void SetItemInfo(int itemID)
	{
		inventoryItemNameText.text = itemDataDictionary[itemID].itemName;
		inventoryItemDescriptionText.text = itemDataDictionary[itemID].itemDescription;
		inventoryItemPriceText.text = $"{itemDataDictionary[itemID].itemPrice} ���";

		switch(itemDataDictionary[itemID].itemTag)
		{
			case itemTag.consumable:
				{
					inventoryItemTagText.text = "�Դ°�";
					inventoryItemStatText.text = $"ü�� : {itemDataDictionary[itemID].health}";
					break;
				}
			case itemTag.weapon:
				{
					inventoryItemTagText.text = "����";
					inventoryItemStatText.text = $"���ݷ� : {itemDataDictionary[itemID].attackPower}";
					break;
				}
			case itemTag.armor:
				{
					inventoryItemTagText.text = "����";
					inventoryItemStatText.text = $"���� : {itemDataDictionary[itemID].defencePower}";
					break;
				}
		}
	}

	private void OnClickInventoryItem(int itemNum, int itemID)
	{
		switch (itemDataDictionary[itemID].itemTag)
		{
			case itemTag.weapon:
				{
					SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].weaponID = itemID;
			        weaponImage.sprite = itemSpriteDictionary[itemID];
					weaponAttackPower = itemDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].weaponID].attackPower;
					break;
				}
			case itemTag.armor:
				{
					SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].armorID = itemID;
					armorImage.sprite = itemSpriteDictionary[itemID];
					armorDefencePower = itemDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].armorID].defencePower;
					break;
				}
			case itemTag.consumable:
				{
					SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].currentHealth += itemDataDictionary[itemID].health;
					if(SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].currentHealth > characterDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].characterID].defaultMaxHealth)
					{
						SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].currentHealth = characterDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].characterID].defaultMaxHealth;

					}
					break;
				}

		}
		characterStatText.text = $"ü�� : {SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].currentHealth}/" +
			$"{characterDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].characterID].defaultMaxHealth}\n" +
			$"���ݷ� : {characterDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].characterID].defaultAttackPower} + {weaponAttackPower}" +
			$"���� : {characterDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].characterID].defaultDefencePower} + {armorDefencePower}";


		Destroy(inventoryScrollviewContentTransform.GetChild(itemNum));
		SaveDataHolder.saveData.itemIDList.Remove(itemNum);
		System.GC.Collect();

	}


	private void ChangeCharacter(int currentCharacterNum)
	{

		characterPortraitImage.sprite = characterSpriteDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].characterID];

		if (itemDataDictionary.ContainsKey(SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].weaponID))
		{
			weaponImage.sprite = itemSpriteDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].weaponID];
			weaponAttackPower = itemDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].weaponID].attackPower;
		}
		else
		{
			weaponAttackPower = 0;
		}
		if (itemDataDictionary.ContainsKey(SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].armorID))
		{
			armorImage.sprite = itemSpriteDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].armorID];
			armorDefencePower = itemDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].armorID].defencePower;
		}
		else
		{
			armorDefencePower = 0;
		}

		characterNameText.text = characterDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].characterID].characterName;
		characterStatText.text = $"ü�� : {SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].currentHealth}/" +
			$"{characterDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].characterID].defaultMaxHealth}\n" +
			$"���ݷ� : {characterDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].characterID].defaultAttackPower} + {weaponAttackPower}" +
			$"���� : {characterDataDictionary[SaveDataHolder.saveData.characterSaveDataArray[currentCharacterNum].characterID].defaultDefencePower} + {armorDefencePower}";

	}

	private void OpenInventoryPanel()
	{
		inventoryPanel.SetActive(true);
	}

	public void OnClickCloseInventoryPanel()
	{
		inventoryPanel.SetActive(false);
	}


	#endregion


	#region Store Methods

	public void GenerateStorePanel()
	{
		storeDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, StoreData>>
			(Resources.Load<TextAsset>("Data/StoreData").text);
		storeItemButtonList = new List<Button>();
		for (int i = 0; i < storeItemButtonCount; i++)
		{
			Instantiate(storeItemButtonPrefab, storeScrollviewContentTransform);
		}
		storeMoneyText.text = $"���� �� : {SaveDataHolder.saveData.money} ���";
	}

	public void OnClickStoreItem(int itemID)
	{
		buyButton.onClick.RemoveAllListeners();
		buyButton.onClick.AddListener(() => OnClickBuyItem(itemID));

		storeItemNameText.text = itemDataDictionary[itemID].itemName;
		switch(itemDataDictionary[itemID].itemTag)
		{
			case itemTag.weapon:
				{
					storeItemTagText.text = "����";
					storeItemStatText.text = $"���ݷ� : {itemDataDictionary[itemID].attackPower}";
					break;
				}
			case itemTag.armor:
				{
					storeItemTagText.text = "����";
					storeItemStatText.text = $"���� : {itemDataDictionary[itemID].attackPower}";
					break;
				}
			case itemTag.consumable:
				{
					storeItemTagText.text = "�Դ°�";
					storeItemStatText.text = $"ȸ���� : {itemDataDictionary[itemID].attackPower}";
					break;
				}
		}
		storeItemDescriptionText.text = itemDataDictionary[itemID].itemDescription;
		storeItemPriceText.text = $"���� : {itemDataDictionary[itemID].itemPrice} ���";

		if(SaveDataHolder.saveData.money < itemDataDictionary[itemID].itemPrice)
		{
			buyButton.interactable = false;
		}
		else
		{
			buyButton.interactable = true;
		}
	}

	public void OnClickBuyItem(int itemID)
	{
		SaveDataHolder.saveData.money -= itemDataDictionary[itemID].itemPrice;
		storeMoneyText.text = $"���� �� : {SaveDataHolder.saveData.money} ���";

		SaveDataHolder.saveData.itemIDList.Add(itemID);


		Button itemButton;
		itemButton = Instantiate(inventoryItemButtonPrefab, inventoryScrollviewContentTransform).GetComponent<Button>();
		itemButton.onClick.AddListener(() => OnClickInventoryItemCoroutine(itemButton.transform.GetSiblingIndex(), itemID));


		if (SaveDataHolder.saveData.money < itemDataDictionary[itemID].itemPrice)
		{
			buyButton.interactable = false;
		}
		else
		{
			buyButton.interactable = true;
		}
	}

	private void SetStoreUI()
	{
		for (int i = 0; i < storeItemButtonList.Count; i++)
		{
			storeItemButtonList[i].gameObject.SetActive(false);
			storeItemButtonList[i].onClick.RemoveAllListeners();
		}

		storeData = storeDataDictionary[nodeManagerDictionary[playerTransform.position].GetTileID()];

		for (int i = 0; i < storeData.itemIDArray.Length; i++)
		{
			storeItemButtonList[i].onClick.AddListener(() => OnClickStoreItem(storeData.itemIDArray[i]));
			storeItemButtonList[i].GetComponent<Image>().sprite = itemSpriteDictionary[storeData.itemIDArray[i]];
			storeItemButtonList[i].gameObject.SetActive(true);
		}
	}

	private void OpenStorePanel()
	{
		storePanel.SetActive(true);
	}

	public void OnClickCloseStorePanel()
	{
		storePanel.SetActive(false);

		storeItemNameText.text = null;
	    storeItemTagText.text = null;
	    storeItemDescriptionText.text = null;
	    storeItemStatText.text = null;
	    storeItemPriceText.text = null;
}

	#endregion

	public static class BattleDataHolder
	{
		public static int[] enemyCharacterIDArray;
		public static CharacterSaveData[] characterSaveDataArray;
		public static List<int> itemIDList;
		public static string battleSceneBackgroundSpriteLocation;
	}
}
