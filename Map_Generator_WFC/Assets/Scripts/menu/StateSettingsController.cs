using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StateSettingsController 
{
    public static Vector2Int stMapSize = new Vector2Int(20,20);  // размер карты

    public static int stWeightGrass = 60; //вес травы
    public static int stWeightGrassTree = 15; //вес деревьев
    public static int stWeightWater = 20; //вес воды
    public static int stWeightGrassCorner=10; //вес угла
    public static int stWeightGrassInnerCorner=10; //вес внутреннего угла
    public static int stWeightGrassSide=10; //вес берега
   
}
