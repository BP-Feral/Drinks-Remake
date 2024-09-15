using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SlotData
{
    public int id;
    public string drink;
    public Vector2 position;
}

[System.Serializable]
public class LevelData
{
    public List<SlotData> slots;
    public string difficulty;
    public float timer;
}