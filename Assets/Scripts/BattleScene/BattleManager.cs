using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using Newtonsoft.Json;
using static GameManager;
using TMPro;
using UnityEditor.U2D.Aseprite;

public class BattleManager : MonoBehaviour //배틀매니저 클래스
{
	public Vector3[] characterPositionArray; //캐릭터 오브젝트 배치 좌표 : 인스펙터에서 등록
	public static BattleManager instance = null; //BattleManager 인스턴스
	public SpriteRenderer battleSceneBackgroundSpriteRenderer; //전투 신 배경 이미지용 SpriteRenderer
	public GameObject characterPrefab; //캐릭터 기본 Prefab : 생성 후 스프라이트와 데이터를 따로 설정
	public GameObject playerCharacterHighLight; //플레이어 캐릭터 바닥에 위치하는 하이라이트 : 인스펙터에서 등록. 현재 턴인 플레이어 캐릭터 혹은 타겟팅된 플레이어 캐릭터
	public GameObject enemyCharacterHighLight; //적 캐릭터 바닥에 위치하는 하이라이트 : 인스펙터에서 등록. 타겟팅된 적 캐릭터 혹은 현재 턴인 적 캐릭터


	#region Data Dictionary Variables
	private Dictionary<int, CharacterData> characterDataDictionary; //캐릭터 데이터 딕셔너리
	private Dictionary<int, ItemData> itemDataDictionary; //아이템 데이터 딕셔너리
	private Dictionary<int, SkillData> skillDataDictionary; //스킬 데이터 딕셔너리
 	private Dictionary<int, Sprite> skillSpriteDictionary; //스킬 스프라이트 딕셔너리 : 스프라이트는 Resources 폴더에 저장되어 있는데 턴이 바뀔 때마다 매번 호출하면 메모리 낭비이기 때문에 딕셔너리에 저장해둠
	#endregion

	private List<CharacterManager> characterManagerList; //캐릭터 매니저 스크립트 리스트
	private Queue<CharacterManager> turnQueue; //턴 관리 큐 : 캐릭터 매니저 스크립트를 큐의 요소로 등록


	private CharacterManager currentPlayerCharacterManager; //현재 턴인 플레이어 캐릭터 매니저 스크립트 혹은 현재 적이 타겟팅한 플레이어 매니저 스크립트
	private CharacterManager currentEnemyCharacterManager; //현재 턴인 적 캐릭터 매니저 스크립트 혹은 현재 플레이어가 타겟팅한 적 매니저 스크립트
	private SkillData skillData; //스킬 데이터 구조체 : 적 캐릭터 AI 매니저 메소드에서 스킬 타입을 여러번 확인하기 때문에 전역 변수로 등록함

	#region Player Character Panel Objects and Variables

	[Header("Player Character Panel Objects and Variables")]
	public GameObject playerCharacterPanel; //플레이어 캐릭터 패널 : 플레이어 캐릭터의 턴에서 캐릭터의 정보와 사용할 수 있는 행동이 표시됨
	public TextMeshProUGUI characterNameText; //캐릭터 이름 텍스트
	public TextMeshProUGUI characterStatText; //캐릭터 상태 텍스트
	public Image defaultAttackButtonImage; //기본 공격 버튼 이미지
	public Image defaultActiveSkill01ButtonImage; //1번 스킬 버튼 이미지
	public Image defaultActiveSkill02ButtonImage; //2번 스킬 버튼 이미지
	public Button defaultActiveSkill01Button; //1번 스킬 버튼
	public Button defaultActiveSkill02Button; //2번 스킬 버튼

	#endregion

	private void Awake()
	{
		if (instance == null) //인스턴스가 없을 때
		{
			instance = this; //배틀매니저 인스턴스 등록
		}
		else
		{
			if (instance != this) //인스턴스가 이미 있는데 자신이 아닐 때
				Destroy(this.gameObject); //인스턴스 삭제
		}

		Application.targetFrameRate = 60; //타겟 프레임은 60

		GenerateBattle(); //전투 알고리즘 생성
		GenerateTurn(); //턴 알고리즘 생성
	}


	private void Update()
	{
		if (Input.GetMouseButton(0)) //마우스 왼쪽 클릭
		{
			Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition); //현재 마우스 클릭 위치 저장
			RaycastHit2D mouseHit = Physics2D.Raycast(mousePosition, Vector2.zero, 0f); //마우스 클릭 위치로 레이캐스트
			if (mouseHit.collider != null) //레이캐스트가 콜라이더에 닿았을 경우
			{
				if (!mouseHit.collider.gameObject.GetComponent<CharacterManager>().GetCharacterData().isPlayerCharacter && //레이캐스트 오브젝트가 플레이어 캐릭터가 아닐 경우
					turnQueue.Peek().GetCharacterData().isPlayerCharacter) //현재 턴이 플레이어 캐릭터인 경우
				{
					currentEnemyCharacterManager = mouseHit.collider.gameObject.GetComponent<CharacterManager>(); //적 캐릭터 타겟팅 지정
					enemyCharacterHighLight.transform.position = mouseHit.collider.transform.position; //적 캐릭터 하이라이트를 타겟팅한 적에게 이동
				}
			}
		}
	}


	private void GenerateBattle() //전투 알고리즘 생성 메소드
	{
		battleSceneBackgroundSpriteRenderer.sprite = Resources.Load<Sprite>(BattleDataHolder.battleSceneBackgroundSpriteLocation); //전투 신 배경 이미지 Resources 폴더에서 호출 후 저장 :
																																   //BattleDataHolder 클래스의 스프라이트 위치 변수 이용


		characterDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, CharacterData>> //캐릭터 데이터 딕셔너리 저장
			(Resources.Load<TextAsset>("Data/CharacterData").text);
		itemDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, ItemData>> //아이템 데이터 딕셔너리 저장
			(Resources.Load<TextAsset>("Data/ItemData").text);
		skillDataDictionary = JsonConvert.DeserializeObject<Dictionary<int, SkillData>> //스킬 데이터 딕셔너리 저장
			(Resources.Load<TextAsset>("Data/SkillData").text);

		skillSpriteDictionary = new Dictionary<int, Sprite>(); //스킬 스프라이트 딕셔너리 초기화

		foreach (KeyValuePair<int, SkillData> items in skillDataDictionary) //스킬 데이터 딕셔너리 반복문
		{
			skillSpriteDictionary.Add(items.Key, Resources.Load<Sprite>(items.Value.skillSpriteLocation)); //스킬 스프라이트 딕셔너리에 스킬 ID를 Key로, 스킬 스프라이트 위치 변수를 Value로 저장
		}



		GameObject character; //캐릭터 오브젝트 선언
		CharacterManager characterManager; //캐릭터 매니저 선언

		characterManagerList = new List<CharacterManager>(); //캐릭터 매니저 리스트 초기화
		for (int i = 0; i < BattleDataHolder.characterSaveDataArray.Length; i++) //BattleDataHolder 클래스의 캐릭터 저장 데이터 배열 반복문
		{
			character = Instantiate(characterPrefab); //캐릭터 Prefab 생성
			character.transform.position = characterPositionArray[i]; //생성된 캐릭터 position 지정
			characterManager = character.transform.GetComponent<CharacterManager>(); //캐릭터 매니저 저장
			characterManager.SetCharacterData(characterDataDictionary[BattleDataHolder.characterSaveDataArray[i].characterID]); //캐릭터 데이터 등록
			characterManager.SetCharacterSaveData(BattleDataHolder.characterSaveDataArray[i]); //캐릭터 저장 데이터 등록 : 캐릭터가 전투 신에 진입하는 시점의 체력이나 아이템은 따로 존재함
			characterManagerList.Add(characterManager); //캐릭터 매니저 리스트에 캐릭터 매니저 추가
		}

		int enemyNum; //적 숫자 초기화
		enemyNum = Random.Range(1, 4); //적은 1~3 명 중에 무작위로 숫자가 정해짐. 특정 적 타입은 숫자가 고정되어야 할 경우 데이터와 조건문을 추가할 것임
		int enemyID; //적 ID 초기화
		for (int i = 0; i < enemyNum; i++) //정해진 적 숫자 반복문
		{
			enemyID = BattleDataHolder.enemyCharacterIDArray[Random.Range(0, BattleDataHolder.enemyCharacterIDArray.Length)]; //타일에 등록된 ID 배열에서 무작위로 ID를 뽑아서 저장
			character = Instantiate(characterPrefab); //캐릭터 오브젝트 생성
			character.transform.position = characterPositionArray[i + 3]; //적 position은 플레이어 position 배열에서 3,4,5번이기 때문에 i에 3을 더해서 지정 
			characterManager = character.transform.GetComponent<CharacterManager>(); //캐릭터 매니저 저장
			characterManager.SetCharacterData(characterDataDictionary[BattleDataHolder.enemyCharacterIDArray[enemyID]]); //캐릭터 데이터 등록
			characterManagerList.Add(characterManager); //캐릭터 매니저 리스트에 캐릭터 매니저 추가
		}

	}


	#region Turn Methods

	private void GenerateTurn() //턴 관리 큐 생성 메소드
	{
		turnQueue = new Queue<CharacterManager>(); //턴 관리 큐 초기화

		CharacterManager characterManager; //캐릭터 매니저 선언

		characterManager = characterManagerList[0]; //캐릭터 매니저 리스트 0번으로 캐릭터 매니저 저장

		while (true) //break가 호출되지 않으면 무한 반복
		{
			for (int i = 1; i < characterManagerList.Count; i++) //캐릭터 매니저 리스트 반복문
			{
				if (characterManager.GetTurnSpeed() <= characterManagerList[i].GetTurnSpeed()) //현재 캐릭터 매니저의 턴 속도보다 캐릭터 매니저 리스트 요소의 턴 속도보다 느릴 경우 혹은 턴 속도가 같을 경우
				{
					characterManager = characterManagerList[i]; //캐릭터 매니저 리스트 요소로 캐릭터 매니저 저장 : 즉 턴 속도가 빠르거나 같은 순서대로 큐가 만들어질 것임
				}
			}
			turnQueue.Enqueue(characterManager); //턴 관리 큐에 캐릭터 매니저 추가
			characterManager.AddTurnSpeed(-characterManager.GetTurnSpeed()); //캐릭터 매니저의 턴 속도 감소 :
																			 //큐에 들어왔으니 그 다음으로 턴 속도가 빠른 캐릭터 매니저가 큐에 들어오기 위해 턴 속도가 회복될 때까진 후순위로 미뤄져야 함

			if (turnQueue.Count == 10) //턴 관리 큐에 10개의 캐릭터 매니저가 추가됐을 때
			{
				break; //반복문 탈출
			}
		}
		NextTurn(); //다음 턴 메소드 호출
	}

	private void AddTurn() //턴 추가 메소드
	{
		CharacterManager characterManager; //캐릭터 매니저 선언
		characterManager = characterManagerList[0]; //캐릭터 매니저 리스트 0번으로 캐릭터 매니저 저장
		for (int i = 1; i < characterManagerList.Count; i++) //캐릭터 매니저 리스트 반복문
		{
			if (characterManager.GetTurnSpeed() <= characterManagerList[i].GetTurnSpeed()) //현재 캐릭터 매니저의 턴 속도보다 캐릭터 매니저 리스트 요소의 턴 속도보다 느릴 경우 혹은 턴 속도가 같을 경우
            {
				characterManager = characterManagerList[i]; //캐릭터 매니저 리스트 요소로 캐릭터 매니저 저장 : 즉 턴 속도가 빠르거나 같은 순서대로 큐가 만들어질 것임
            }
		}
		turnQueue.Enqueue(characterManager); //턴 관리 큐에 캐릭터 매니저 추가
		characterManager.AddTurnSpeed(-characterManager.GetTurnSpeed()); //캐릭터 매니저의 턴 속도 감소 :
																		 //큐에 들어왔으니 그 다음으로 턴 속도가 빠른 캐릭터 매니저가 큐에 들어오기 위해 턴 속도가 회복될 때까진 후순위로 미뤄져야 함

		//알고리즘은 GenerateTurn 메소드와 동일함
	}

	private void RemoveTurn() //턴 삭제 메소드
	{
		turnQueue.Peek().AddTurnSpeed(turnQueue.Peek().GetTurnSpeed()); //현재 턴인 캐릭터 매니저의 턴 속도 회복 : AddTurn 메소드에서 턴 속도가 빠른 순서대로 다음 큐를 만들기 위해
		turnQueue.Dequeue(); //현재 턴인 캐릭터 매니저의 큐 제거
		AddTurn(); //턴 추가 메소드 호출
	}

	private void NextTurn() //다음 턴 메소드
	{
		CharacterManager characterManager = turnQueue.Peek().GetComponent<CharacterManager>(); //턴 관리 큐의 첫번째 요소로 캐릭터 매니저 저장
		switch (characterManager.GetCharacterData().isPlayerCharacter) //현재 턴이 플레이어 캐릭터인지 적 캐릭터인지 확인
		{
			case true: //플레이어 캐릭터인 경우
				{
					currentPlayerCharacterManager = characterManager; //현재 플레이어 캐릭터 매니저 저장
					playerCharacterHighLight.transform.position = currentPlayerCharacterManager.transform.position; //플레이어 캐릭터 하이라이트 위치 지정

					for(int i = 0; i < characterManagerList.Count; i++) //캐릭터 매니저 리스트 반복문
					{
						if (!characterManagerList[i].GetCharacterData().isPlayerCharacter) //적 캐릭터인 경우
						{
							currentEnemyCharacterManager = characterManagerList[i]; //현재 적 캐릭터 매니저 저장
							enemyCharacterHighLight.transform.position = currentEnemyCharacterManager.transform.position; //적 캐릭터 하이라이트 위치 지정
							break; //반복문 탈출 : 플레이어의 턴이 시작될 때 살아남은 적 중의 1번째로 타겟팅을 지정함. 마우스 클릭으로 타겟팅 변경 가능
						}
					}
					SetPlayerCharacterPanel(); //플레이어 캐릭터 패널 설정 메소드 호출
					break;
				}
			case false:
				{
					currentEnemyCharacterManager = characterManager; //현재 적 캐릭터 매니저 저장
					enemyCharacterHighLight.transform.position = currentEnemyCharacterManager.transform.position; //적 캐릭터 하이라이트 위치 지정
					for (int i = 0; i < characterManagerList.Count; i++) //캐릭터 매니저 리스트 반복문
					{
						if (characterManagerList[i].GetCharacterData().isPlayerCharacter) //플레이어 캐릭터인 경우
						{
							currentPlayerCharacterManager = characterManagerList[i]; //현재 플레이어 캐릭터 매니저 저장
							playerCharacterHighLight.transform.position = currentPlayerCharacterManager.transform.position; //플레이어 캐릭터 하이라이트 위치 지정
							break; //반복문 탈출 : 적의 턴이 시작될 때 살아남은 플레이어 중의 1번째로 타겟팅을 지정함 : DecideEnemyMovement 메소드에서 타겟팅 변경 가능
						}
					}
					DecideEnemyMovement(); //적 캐릭터 AI 메소드 호출
					break;
				}
		}
	}



	#endregion

	#region Player Character Panel Methods


	private void SetPlayerCharacterPanel() //플레이어 캐릭터 패널 설정 메소드
	{
		characterNameText.text = currentPlayerCharacterManager.GetCharacterData().characterName; //현재 캐릭터 이름 텍스트 저장
		characterStatText.text = $"체력 : {currentPlayerCharacterManager.GetCurrentHealth()} / {currentPlayerCharacterManager.GetCharacterData().defaultMaxHealth}\n" + //현재 캐릭터 체력 텍스트 저장
			$"공격력 : {currentPlayerCharacterManager.GetCharacterData().defaultAttackPower} + {itemDataDictionary[currentPlayerCharacterManager.GetCharacterSaveData().weaponID].attackPower}\n" + //현재 캐릭터 공격력 텍스트 저장
			$"방어력 : {currentPlayerCharacterManager.GetCharacterData().defaultDefencePower} + {itemDataDictionary[currentPlayerCharacterManager.GetCharacterSaveData().armorID].defencePower}"; //현재 캐릭터 방어력 텍스트 저장
		defaultActiveSkill01ButtonImage.sprite = skillSpriteDictionary[currentPlayerCharacterManager.GetCharacterData().defaultActiveSkill01ID]; //1번 스킬 스프라이트 저장
		defaultActiveSkill02ButtonImage.sprite = skillSpriteDictionary[currentPlayerCharacterManager.GetCharacterData().defaultActiveSkill02ID]; //2번 스킬 스프라이트 저장

		if(!playerCharacterPanel.activeSelf) //플레이어 캐릭터 패널이 비활성화 되어있으면
		{
			playerCharacterPanel.SetActive(true); //플레이어 캐릭터 패널 활성화
		}


		switch (currentPlayerCharacterManager.GetDefaultSkill01Turn()) //1번 스킬의 대기 턴 수 확인
		{
			case 0: //대기 턴 수가 0일 경우
				{
					defaultActiveSkill01Button.interactable = true; //1번 스킬 사용 가능
					break;
				}
			default:
				{
					defaultActiveSkill01Button.interactable = false; //1번 스킬 사용 불가능
					break;
				}
		}

		switch (currentPlayerCharacterManager.GetDefaultSkill02Turn()) //2번 스킬의 대기 턴 수 확인
		{
			case 0: //대기 턴 수가 0일 경우
				{
					defaultActiveSkill02Button.interactable = true; //2번 스킬 사용 가능
					break;
				}
			default:
				{
					defaultActiveSkill02Button.interactable = false; //2번 스킬 사용 불가능
					break;
				}
		}


	}

	public void OnClickDefaultAttack() //기본 공격 메소드
	{


		OnClickEndTurn();
	}

	public void OnClickDefaultSkill01() //1번 스킬 메소드
	{


		OnClickEndTurn();
	}

	public void OnClickDefaultSkill02() //2번 스킬 메소드
	{


		OnClickEndTurn();
	}

	public void OnClickItem() //소비 아이템 메소드
	{
		//소비 아이템만 보여줘야 한다
	}

	public void OnClickEndTurn() //턴 종료 메소드
	{
		if (playerCharacterPanel.activeSelf) //플레이어 캐릭터 패널이 활성화 되어있으면
		{
			playerCharacterPanel.SetActive(false); //플레이어 캐릭터 패널 비활성화 : 적의 턴인 상태에서 플레이어의 클릭을 방지
		}
		NextTurn(); //다음 턴 메소드 호출
	}
	#endregion

	#region Enemy Methods

	private void DecideEnemyMovement() //적 AI 메소드
	{

		for (int i = 0; i < characterManagerList.Count; i++) //캐릭터 매니저 리스트 반복문
		{
			if(currentPlayerCharacterManager.GetCurrentHealth() / currentPlayerCharacterManager.GetCharacterData().defaultMaxHealth > //현재 플레이어 캐릭터의 남아있는 체력 비율이
				characterManagerList[i].GetCurrentHealth() / characterManagerList[i].GetCharacterData().defaultMaxHealth && //캐릭터 매니저 리스트 요소의 남아있는 체력 비율보다 높고
				characterManagerList[i].GetCharacterData().isPlayerCharacter) // 플레이어 캐릭터 매니저라면 : 즉 적 AI는 현재 체력 비율이 가장 낮은 플레이어 캐릭터를 최우선으로 타겟팅할 것임
			{
				currentPlayerCharacterManager = characterManagerList[i]; //현재 플레이어 캐릭터 매니저에 캐릭터 매니저 리스트 요소 저장
				playerCharacterHighLight.transform.position = currentPlayerCharacterManager.transform.position; //플레이어 캐릭터 하이라이트 위치 지정
			}
		}



		//적 AI의 행동 알고리즘은 한꺼번에 서술한다.
		//적 AI는 현재 사용 가능한 행동을 기준으로 1번 스킬, 2번 스킬, 기본 공격 순서로 사용할 것이다.
		//적 AI는 현재 자신의 체력 비율이 50% 이상인지 확인한다.
		//현재 체력 비율이 50% 이상일 경우 행동 우선 순위는 버프 > 디버프 > 공격 > 회복 > 기본 공격 순이다.
		//현재 체력 비율이 50% 이상일 때 회복 스킬은 회복량을 계산하여 현재 체력 + 회복량이 최대 체력을 초과하지 않을 때만 사용한다.
		//현재 체력 비율이 50% 미만일 경우 행동 우선 순위는 회복 > 방어 > 디버프 > 버프 > 공격 > 기본 공격 순이다.
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
									//버프 스킬
									break;
								}
							default:
								{
									switch (skillData.skillType)
									{
										case SkillType.Debuff:
											{
												//디버프 스킬
												break;
											}
										default:
											{
												switch (skillData.skillType)
												{
													case SkillType.Attack:
														{
															//공격 스킬
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
																			//회복 스킬
																		}
																		else
																		{
																			//기본 공격
																		}
																		break;
																	}
																default:
																	{
																		//기본 공격
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
												//버프 스킬
												break;
											}
										default:
											{
												switch (skillData.skillType)
												{
													case SkillType.Debuff:
														{
															//디버프 스킬
															break;
														}
													default:
														{
															switch (skillData.skillType)
															{
																case SkillType.Attack:
																	{
																		//공격 스킬
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
																						//회복 스킬
																					}
																					else
																					{
																						//기본 공격
																					}
																					break;
																				}
																			default:
																				{
																					//기본 공격
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
									//기본 공격
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
									//회복 스킬
									break;
								}
								default:
								{
									switch(skillData.skillType)
									{
										case SkillType.Defence:
											{
												//방어 스킬
												break;
											}
										default:
											{
												switch(skillData.skillType)
												{
													case SkillType.Debuff:
														{
															//디버프 스킬
															break;
														}
													default:
														{
															switch(skillData.skillType)
															{
																case SkillType.Buff:
																	{
																		//버프 스킬
																		break;
																	}
																default:
																	{
																		switch(skillData.skillType)
																		{
																			case SkillType.Attack:
																				{
																					//공격 스킬
																					break;
																				}
																			default:
																				{
																					//기본 공격
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
												//회복 스킬
												break;
											}
										default:
											{
												switch (skillData.skillType)
												{
													case SkillType.Defence:
														{
															//방어 스킬
															break;
														}
													default:
														{
															switch (skillData.skillType)
															{
																case SkillType.Debuff:
																	{
																		//디버프 스킬
																		break;
																	}
																default:
																	{
																		switch (skillData.skillType)
																		{
																			case SkillType.Buff:
																				{
																					//버프 스킬
																					break;
																				}
																			default:
																				{
																					switch (skillData.skillType)
																					{
																						case SkillType.Attack:
																							{
																								//공격 스킬
																								break;
																							}
																						default:
																							{
																								//기본 공격
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
									//기본 공격
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
