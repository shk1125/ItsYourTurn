using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapData //�� ������ ����ü
{
    public string mapName; //�� �̸�
    public string tilemapLocation; //Ÿ�ϸ� ��ġ
    public string battleSceneBackgroundSpriteLocation;
    public Dictionary<int,tileData> tileDataDictionary;
    public string tileIDArrayLocation;
}

public struct tileData
{
    public TileName tileName;
    public double encounterProbability;
    public int[] characterIDArray;
}   

public enum TileName
{
    Plain,
    Forest,
    Mountain,
	Store
}

