using System.IO;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public string[] levelFileNames;
    public GameObject slotPrefab;
    public GameObject[] drinkPrefabs;

    private int currentLevelIndex = 0;

    private void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    private void LoadLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelFileNames.Length)
        {
            Debug.LogError("Invalid level index.");
            return;
        }

        string jsonFileName = levelFileNames[levelIndex];
        string path = Path.Combine(Application.streamingAssetsPath, jsonFileName);

        if (!File.Exists(path))
        {
            Debug.LogError($"File not found: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        LevelData levelData = JsonUtility.FromJson<LevelData>(json);

        // Clear existing slots (if needed)
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
    }
    public void NextLevel()
    {
        currentLevelIndex = (currentLevelIndex + 1) % levelFileNames.Length;
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
