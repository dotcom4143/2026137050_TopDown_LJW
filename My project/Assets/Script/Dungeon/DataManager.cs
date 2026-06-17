using UnityEngine;
using System.IO;
using System;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;

    public GameData data = new GameData();
    private string filePath;

    public static event Action<int> OnCoinChanged;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        filePath = Application.persistentDataPath + "/saveData.json";
        LoadData();
    }

    public void AddCoin(int amount)
    {
        data.totalCoins += amount;
        OnCoinChanged?.Invoke(data.totalCoins);
    }

    public void SaveData()
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(filePath, json);
    }

    public void LoadData()
    {
        if (File.Exists(filePath))
        {
            string json = File.ReadAllText(filePath);
            data = JsonUtility.FromJson<GameData>(json);
            OnCoinChanged?.Invoke(data.totalCoins);
        }
    }
}

[System.Serializable]
public class GameData
{
    public int totalCoins = 0;
    public float maxSurvivalTime = 0f;
}