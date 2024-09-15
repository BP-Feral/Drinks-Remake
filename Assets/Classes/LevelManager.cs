using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    public List<GameObject> slots = new List<GameObject>();
    public int score = 0;

    public LevelDataSO levelDataSO;
    public GameObject slotPrefab;
    public GameObject[] drinkPrefabs;
    public Text timerText;
    public Text difficultyText;
    public Text Txtlevel;
    public Text Txtscore;
    public GameObject gameOverPanelPrefab;


    private int currentLevelIndex = 0;
    private bool isLevelActive = true;
    private float levelTimer;
    private GameObject gameOverPanelInstance;

    private AudioManager audioManager;
    private void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
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

        // Clear existing slots and reset the list
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        slots.Clear();

        // Instantiate slots and drinks
        foreach (SlotData slotData in levelData.slots)
        {
            GameObject slotInstance = Instantiate(slotPrefab, transform);
            slotInstance.GetComponent<RectTransform>().anchoredPosition = slotData.position;

            slots.Add(slotInstance);

            if (!string.IsNullOrEmpty(slotData.drink))
            {
                GameObject drinkPrefab = GetDrinkPrefabByName(slotData.drink);
                if (drinkPrefab != null)
                {
                    GameObject drinkInstance = Instantiate(drinkPrefab, slotInstance.transform);
                }
            }
        }

        Debug.Log($"Loaded level with difficulty: {levelData.difficulty}");
        levelTimer = levelData.timer;
        isLevelActive = true;
        
        if (Txtlevel != null)
        {
            Txtlevel.text = $"Level: {levelIndex + 1}";
        }
        
        if (difficultyText != null)
        {
            difficultyText.text = $"Difficulty: {levelData.difficulty}";
        }
    }

    private void EndLevel()
    {
        if (isLevelActive)
        {
            isLevelActive = false;

            if (gameOverPanelPrefab != null)
            {
                if (gameOverPanelInstance != null)
                {
                    Destroy(gameOverPanelInstance);
                }

                gameOverPanelInstance = Instantiate(gameOverPanelPrefab, Vector3.zero, Quaternion.identity);
                RectTransform rt = gameOverPanelInstance.GetComponent<RectTransform>();
                rt.SetParent(FindObjectOfType<Canvas>().transform, false);
            }

            Debug.Log("Level ended.");
        }
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

    public void CheckForMatches()
    {
        for (int i = 0; i < slots.Count; i += 3)
        {
            if (i + 2 >= slots.Count) break;

            GameObject slot1 = slots[i];
            GameObject slot2 = slots[i + 1];
            GameObject slot3 = slots[i + 2];

            Draggable drink1 = slot1.GetComponentInChildren<Draggable>();
            Draggable drink2 = slot2.GetComponentInChildren<Draggable>();
            Draggable drink3 = slot3.GetComponentInChildren<Draggable>();

            // Check if all three slots contain the same type of drink
            if (drink1 != null && drink2 != null && drink3 != null)
            {
                if (drink1.name == drink2.name && drink2.name == drink3.name)
                {
                    Debug.Log("Match found!");

                    // Destroy the matched drinks
                    Destroy(drink1.gameObject);
                    Destroy(drink2.gameObject);
                    Destroy(drink3.gameObject);

                    if (audioManager != null)
                    {
                        audioManager.PlayMatchSound();
                    }
                    // Award points
                    score += 10; // TODO display score and level
                    if (Txtscore != null)
                    {
                        Txtscore.text = $"Score: {score}";
                    }
                    Debug.Log("Score: " + score);
                }
            }
        }

        // Delay checking for remaining drinks until the next frame
        StartCoroutine(DelayedCheckForRemainingDrinks());
    }

    // Coroutine to check for remaining drinks after a small delay
    private IEnumerator DelayedCheckForRemainingDrinks()
    {
        // Wait until the end of the frame to ensure all destroyed objects are fully removed
        yield return new WaitForEndOfFrame();

        Draggable[] remainingDrinks = FindObjectsOfType<Draggable>();

        if (remainingDrinks.Length == 0)
        {
            Debug.Log("No more drinks left! Moving to next level.");
            NextLevel();
        }
    }
}
