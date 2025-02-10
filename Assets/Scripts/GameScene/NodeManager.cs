using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
	[SerializeField]private float encounterProbability;
	private float encounterCalculateNum;
	private int[] enemyCharacterIDArray;
	private string battleSceneBackgroundSpriteLocation;
	private int tileID;

	public void SetData(tileData tileData, string battleSceneBackgroundSpriteLocation, int tileID)
	{
		encounterProbability = (float)tileData.encounterProbability;
		enemyCharacterIDArray = tileData.characterIDArray;
		this.battleSceneBackgroundSpriteLocation = battleSceneBackgroundSpriteLocation;
		this.tileID = tileID;
	}


	public void CheckNode()
	{
		encounterCalculateNum = Random.Range(0.0f, 1.0f);
		
		if(encounterCalculateNum <= encounterProbability)
		{
			GameManager.instance.EncounterEnemy(enemyCharacterIDArray, battleSceneBackgroundSpriteLocation);
		}
	}

	public int GetTileID()
	{
		return tileID;
	}
}
