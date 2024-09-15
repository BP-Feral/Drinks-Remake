using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelDataSO", menuName = "Game/Level Data")]
public class LevelDataSO : ScriptableObject
{
    public string[] levelJsonPaths;
}
