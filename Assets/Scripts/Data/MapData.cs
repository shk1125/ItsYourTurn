using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MapData //맵 데이터 구조체
{
    public string mapName; //맵 이름
    public string tilemapLocation; //타일맵 위치
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

