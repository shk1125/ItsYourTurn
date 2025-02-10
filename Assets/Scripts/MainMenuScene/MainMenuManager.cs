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
		if (Application.genuine == false && Application.genuineCheckAvailable == false) //무결성 검사
		{
			Debug.Log("무결성 검사 실패");
#if UNITY_EDITOR

			UnityEditor.EditorApplication.isPlaying = false; //실패했을 시 에디터는 종료
#endif

			Application.Quit(); //애플리케이션 종료
		}


		Application.targetFrameRate = 60; //타겟 프레임은 60
		DirectoryInfo dataFolder = new DirectoryInfo(Application.persistentDataPath + "/Data/"); //데이터 폴더 정보 저장
		if (!dataFolder.Exists) //데이터 폴더가 없을 경우
		{
			dataFolder.Create(); //데이터 폴더 생성
		}

		saveDataLocation = Application.persistentDataPath + "/Data/saveData.json";

#if !UNITY_ANDROID
		Screen.SetResolution(1920, 1080, true); //PC에서 해상도 Full HD로 고정
#endif
		GenerateLoadGamePanel(); //불러오기 패널 생성
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

		saveNameText.text = $"저장된 게임 : \n{saveDataList[saveNum].saveName}";
		saveTimeText.text = $"저장한 시각 : \n{saveDataList[saveNum].saveTime}";
		


		currentLoadGameScrollviewButton = loadGameScrollviewButton;
		currentLoadGameScrollviewButton.interactable = false;

		this.saveNum = saveNum;

	}

	public void OnClickStartSavedGame()
	{
		loadOrDeleteSavedGame = true;
		confirmText.text = "저장된 게임을 시작하시겠습니까?";
		loadGamePanel.SetActive(false);
		confirmPanel.SetActive(true);
	}

	public void OnClickDeleteSavedGame()
	{
		loadOrDeleteSavedGame = false;
		confirmText.text = "저장된 게임을 삭제하시겠습니까?\n삭제된 게임은 복구되지 않습니다.";
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

		UnityEditor.EditorApplication.isPlaying = false; //에디터일 경우 종료
#endif

		Application.Quit(); //애플리케이션 종료
	}


	#endregion

	#region LoadingMethods

	IEnumerator LoadScene(string scene) //Scene 변경 메소드
	{
		yield return null;
		AsyncOperation op = SceneManager.LoadSceneAsync(scene); //AsyncOperation 변수에 GameScene 연결
		op.allowSceneActivation = false; //로딩 이전에는 Scene을 로드하지 않도록 저장
		float timer = 0.0f; //시간 변수 저장
		while (!op.isDone) //로딩이 끝나지 않았을 경우
		{
			yield return null;
			timer += Time.deltaTime; //시간 변수 증가
			if (op.progress < 0.9f) //0.9f 정도에 로딩이 완료된 것으로 간주
			{
				loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, op.progress, timer); //로딩 정도와 타이머 변수의 보간을 로딩 바의 fillAmount로 표현
				if (loadingBarImage.fillAmount >= op.progress) //로딩 바의 fillAmount가 로딩 정도보다 높을 경우
				{
					timer = 0f; //시간 변수 초기화
				}
			}
			else
			{
				loadingBarImage.fillAmount = Mathf.Lerp(loadingBarImage.fillAmount, 1f, timer); //0.9f 시간 정도가 로딩이 완료된 것으로 간주되기 때문에 1f까지 수동으로 보간
				if (loadingBarImage.fillAmount == 1.0f) //로딩이 완료되었을 때
				{
					op.allowSceneActivation = true; //Scene을 로드하도록 허가
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

