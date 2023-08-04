using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "MapData_SO",menuName = "Map/MapData")]
public class MapData_SO : ScriptableObject
{
    [SceneName]
    public string sceneName;

    [Header("场景大小")] 
    public int gridWidth;
    public int gridHeight;
    [Header("左下原点")] 
    public int originX;
    public int originY;

    public List<TileProperty> tileProperties;
}
