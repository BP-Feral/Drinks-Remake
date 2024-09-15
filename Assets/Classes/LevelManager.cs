using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public LevelDataSO levelDataSO;
    public GameObject slotPrefab;
    public GameObject[] drinkPrefabs;

    public Text timerText;
    public Text difficultyText;

    private int currentLevelIndex = 0;
    private float levelTimer;
    private bool isLevelActive = false;

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    private void Update()
    {
        if (isLevelActive)
        {
            levelTimer -= Time.deltaTime;
            if (levelTimer <= 0)
            {
                levelTimer = 0;
                EndLevel();
            }

            // Update timer UI
            if (timerText != null)
            {
                timerText.text = $"Time: {Mathf.Round(levelTimer)}";
            }
        }
    }

    public void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelDataSO.levelJsonPaths.Length)
        {
            Debug.LogError("Invalid level index.");
            return;
        }

        string resourcePath = levelDataSO.levelJsonPaths[levelIndex];
        TextAsset levelJson = Resources.Load<TextAsset>(resourcePath);

        if (levelJson == null)
        {
            Debug.LogError($"Failed to load JSON file from path: {resourcePath}");
            return;
        }

        LevelData levelData = JsonUtility.FromJson<LevelData>(levelJson.text);

        // Clear existing slots
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Instantiate slots and drinks
        foreach (SlotData slotData in levelData.slots)
        {
            GameObject slotInstance = Instantiate(slotPrefab, transform);
            RectTransform slotRectTransform = slotInstance.GetComponent<RectTransform>();
            slotRectTransform.anchoredPosition = slotData.position;

            if (!string.IsNullOrEmpty(slotData.drink))
            {
                GameObject drinkPrefab = GetDrinkPrefabByName(slotData.drink);
                if (drinkPrefab != null)
                {
                    GameObject drinkInstance = Instantiate(drinkPrefab, slotInstance.transform);
                }
                else
                {
                    Debug.LogWarning($"Drink prefab for {slotData.drink} not found.");
                }
            }
        }

        Debug.Log($"Loaded level with difficulty: {levelData.difficulty}");
        levelTimer = levelData.timer;
        isLevelActive = true;

        if (difficultyText != null)
        {
            difficultyText.text = $"Difficulty: {levelData.difficulty}";
        }
    }

    private void EndLevel()
    {
        isLevelActive = false;
        Debug.Log("Level ended.");

        // TODO LOST GAME
    }

    public void NextLevel()
    {
        currentLevelIndex = (currentLevelIndex + 1) % levelDataSO.levelJsonPaths.Length;
        LoadLevel(currentLevelIndex);
    }

    private GameObject GetDrinkPrefabByName(string name)
    {
        foreach (GameObject drinkPrefab in drinkPrefabs)
        {
            if (drinkPrefab.name == name)
            {
                return drinkPrefab;
            }
        }
        return null;
    }
}
