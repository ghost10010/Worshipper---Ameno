using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Power
{
    public List<GameObject> spawnObjects;
    public int areaSize;
    public int amount;
    public int manaCost;
    public int foodCost;
    public int woodCost;
    public int ironCost;
    public string type;
    public int color;

    public int smaller = -1;
    public int bigger = -1;

    public string nameKey = "";
    public string effectKey = "";
}
