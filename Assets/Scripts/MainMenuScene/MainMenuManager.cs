using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Newtonsoft.Json;

public class MainMenuManager : MonoBehaviour
{

	#region MainMenuPanel Objects and Variables
	public GameObject mainMenuPanel;
	#endregion

	#region LoadGamePanel Objects and Variables
	[Header("LoadGame Objects and Variables")]
	public GameObject loadGamePanel;
	public GameObject loadGameScrollviewButtonPrefab;
	public Transform loadGameScrollviewContentTransform;
	public TextMeshProUGUI saveNameText;
	public TextMeshProUGUI saveTimeText;
	public GameObject confirmPanel;
	public TextMeshProUGUI confirmText;
	public Button loadGameButton;

	private Button currentLoadGameScrollviewButton;
	private List<SaveData> saveDataList;
	private string saveDataLocation;
	private bool loadOrDeleteSavedGame;
	private int saveNum;
	#endregion

	#region HowToPlayPanel Objects and Variables
	[Header("HowToPlay Objects and Variables")]

	public GameObject howToPlayPanel;
	public Image howToPlayImage;
	public GameObject nextPageButton;
	public GameObject previousPageButton;

	[SerializeField]private Sprite[] howToPlaySpriteArray;
	private int currentHowToPlaySpriteNum;

	#endregion


	#region LoadingPanel Objects and Variables

	public GameObject loadingPanel;
	public Image loadingBarImage;

	#endregion

	private void Awake()
	{
		if (Application.genuine == false && Application.genuineCheckAvailable == false) //���Ἲ �˻�
		{
			Debug.Log("���Ἲ �˻� ����");
#if UNITY_EDITOR

			UnityEditor.EditorApplication.isPlaying = false; //�������� �� �����ʹ� ����
#endif

			Application.Quit(); //���ø����̼� ����
		}


		Application.targetFrameRate = 60; //Ÿ�� �������� 60
		DirectoryInfo dataFolder = new DirectoryInfo(Application.persistentDataPath + "/Data/"); //������ ���� ���� ����
		if (!dataFolder.Exists) //������ ������ ���� ���
		{
			dataFolder.Create(); //������ ���� ����
		}

		saveDataLocation = Application.persistentDataPath + "/Data/saveData.json";

#if !UNITY_ANDROID
		Screen.SetResolution(1920, 1080, true); //PC���� �ػ� Full HD�� ����
#endif
		GenerateLoadGamePanel(); //�ҷ����� �г� ����
		GenerateHowToPlayPanel();
	}


	#region NewGameMethods

	public void OnClickNewGame()
	{
		SaveDataHolder.saveData = new SaveData();
		mainMenuPanel.SetActive(false);
		loadingPanel.SetActive(true);
		StartCoroutine(LoadScene("GameScene"));
	}


	#endregion

	#region LoadGameMethods

	public void OnClickLoadGame()
	{
		mainMenuPanel.SetActive(false);
		loadGamePanel.SetActive(true);
	}

	public void OnClickCloseLoadGame()
	{
		loadGamePanel.SetActive(false);
		mainMenuPanel.SetActive(true);
	}

	public void OnClickLoadGameScrollviewButton(int saveNum)
	{
		if (currentLoadGameScrollviewButton == null)
		{
			loadGameScrollviewContentTransform.GetChild(0).GetComponent<Button>().interactable = false;
		}
		else
		{
			currentLoadGameScrollviewButton.interactable = true;
		}

		Button loadGameScrollviewButton = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();

		saveNameText.text = $"����� ���� : \n{saveDataList[saveNum].saveName}";
		saveTimeText.text = $"������ �ð� : \n{saveDataList[saveNum].saveTime}";
		


		currentLoadGameScrollviewButton = loadGameScrollviewButton;
		currentLoadGameScrollviewButton.interactable = false;

		this.saveNum = saveNum;

	}

	public void OnClickStartSavedGame()
	{
		loadOrDeleteSavedGame = true;
		confirmText.text = "����� ������ �����Ͻðڽ��ϱ�?";
		loadGamePanel.SetActive(false);
		confirmPanel.SetActive(true);
	}

	public void OnClickDeleteSavedGame()
	{
		loadOrDeleteSavedGame = false;
		confirmText.text = "����� ������ �����Ͻðڽ��ϱ�?\n������ ������ �������� �ʽ��ϴ�.";
		loadGamePanel.SetActive(false);
		confirmPanel.SetActive(true);
	}

	public void OnClickConfirm()
	{
		switch(loadOrDeleteSavedGame)
		{
			case true:
				{
					loadGamePanel.SetActive(false);
					loadingPanel.SetActive(true);
					SaveDataHolder.saveData = saveDataList[saveNum];
					StartCoroutine(LoadScene("GameScene"));
					break;
				}
			case false:
				{
					Destroy(loadGameScrollviewContentTransform.GetChild(saveNum).gameObject);
					System.GC.Collect();
					saveDataList.RemoveAt(saveNum);
					File.WriteAllText(saveDataLocation, JsonConvert.SerializeObject(saveDataList));
					OnClickLoadGameScrollviewButton(saveNum);
					break;
				}
		}


		confirmPanel.SetActive(false);
	}

	public void OnClickDecline()
	{
		confirmPanel.SetActive(false);
		loadGamePanel.SetActive(true);
	}

	private void GenerateLoadGamePanel()
	{
		if (File.Exists(saveDataLocation))
		{
			string saveDataJson = File.ReadAllText(saveDataLocation);
			saveDataList = JsonConvert.DeserializeObject<List<SaveData>>(saveDataJson);
			Button loadGameScrollviewButton;

			for (int i = 0; i < saveDataList.Count; i++)
			{
				loadGameScrollviewButton = Instantiate(loadGameScrollviewButtonPrefab, loadGameScrollviewContentTransform).GetComponent<Button>();
				loadGameScrollviewButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = saveDataList[i].saveName;
				loadGameScrollviewButton.onClick.AddListener(() => OnClickLoadGameScrollviewButton(i));
			}

			OnClickLoadGameScrollviewButton(0);
		}
		else
		{
			loadGameButton.interactable = false;
		}
	}


	#endregion

	#region HowToPlayMethods

	public void OnClickHowToPlay()
	{
		mainMenuPanel.SetActive(false);
		howToPlayPanel.SetActive(true);
	}

	public void OnClickCloseHowToPlay()
	{
		howToPlayPanel.SetActive(false);
		mainMenuPanel.SetActive(true);
	}

	public void OnClickNextPage()
	{
		if (!previousPageButton.activeSelf)
		{
			previousPageButton.SetActive(true);
		}

		howToPlayImage.sprite = howToPlaySpriteArray[++currentHowToPlaySpriteNum];
		if(currentHowToPlaySpriteNum == howToPlaySpriteArray.Length - 1)
		{
			nextPageButton.SetActive(false);
		}

	}

	public void OnClickPreviousPage()
	{
		if (!nextPageButton.activeSelf)
		{
			nextPageButton.SetActive(true);
		}
		howToPlayImage.sprite = howToPlaySpriteArray[--currentHowToPlaySpriteNum];
		if(currentHowToPlaySpriteNum == 0)
		{
			previousPageButton.SetActive(false);
		}


	}


	private void GenerateHowToPlayPanel()
	{
		//howToPlaySpriteArray = Resources.LoadAll<Sprite>("MainMenuScene/HowToPlaySpriteArray");
		howToPlayImage.sprite = howToPlaySpriteArray[0];
		currentHowToPlaySpriteNum = 0;
	}

	#endregion

	#region QuitGameMethods

	public void OnClickQuitGame()
	{
#if UNITY_EDITOR

		UnityEditor.EditorApplication.isPlaying = false; //�������� ��� ����
#endif

		Application.Quit(); //���ø����̼� ����
	}


	#endregion

	#region LoadingMethods

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


}


public static class SaveDataHolder
{
	public static SaveData saveData;
}

