using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class SettingsMenuController : MonoBehaviour
{
    [System.Serializable]
    public class GameSaveData
    {
        public int totalCoins = 0;
        public int damageLevel = 0;
        public int playerHealthLevel = 0;
        public int moveSpeedLevel = 0;
    }

    [Header("위젯 연결 - 상단 코인 텍스트")]
    [SerializeField] private TextMeshProUGUI totalCoinText;

    [Header("강화 항목 1 - 평타 딜 증가")]
    [SerializeField] private Button dmgUpgradeButton;
    [SerializeField] private TextMeshProUGUI dmgLevelText;
    [SerializeField] private TextMeshProUGUI dmgCostText;
    [SerializeField] private int dmgBaseCost = 10;
    [SerializeField] private int dmgMaxLevel = 5;

    [Header("강화 항목 2 - 체력 증가")]
    [SerializeField] private Button playerHealthUpgradeButton;
    [SerializeField] private TextMeshProUGUI playerHealthLevelText;
    [SerializeField] private TextMeshProUGUI playerHealthCostText;
    [SerializeField] private int playerHealthBaseCost = 10;
    [SerializeField] private int playerHealthMaxLevel = 5;

    [Header("강화 항목 3 - 이동 속도 증가")]
    [SerializeField] private Button moveSpeedUpgradeButton;
    [SerializeField] private TextMeshProUGUI moveSpeedLevelText;
    [SerializeField] private TextMeshProUGUI moveSpeedCostText;
    [SerializeField] private int moveSpeedBaseCost = 10;
    [SerializeField] private int moveSpeedMaxLevel = 5;

    [Header("닫기 버튼")]
    [SerializeField] private Button closeButton;

    private GameSaveData saveData = new GameSaveData();
    private string savePath;

    private void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");
        
        dmgUpgradeButton.onClick.AddListener(() => TryUpgrade(ref saveData.damageLevel, dmgMaxLevel, dmgBaseCost, UpdateDmgUI));
        playerHealthUpgradeButton.onClick.AddListener(() => TryUpgrade(ref saveData.playerHealthLevel, playerHealthMaxLevel, playerHealthBaseCost, UpdatePlayerHealthUI));
        moveSpeedUpgradeButton.onClick.AddListener(() => TryUpgrade(ref saveData.moveSpeedLevel, moveSpeedMaxLevel, moveSpeedBaseCost, UpdateMoveSpeedUI));
        
        closeButton.onClick.AddListener(ClosePanel);

        LoadDataFromJSON();
    }

    public void OpenPanel()
    {
        gameObject.SetActive(true);
        LoadDataFromJSON();
        UpdateAllUI();
    }

    private void ClosePanel()
    {
        gameObject.SetActive(false);
    }

    private void TryUpgrade(ref int currentLevel, int maxLevel, int baseCost, System.Action uiUpdateAction)
    {
        if (currentLevel >= maxLevel) return;

        int cost = CalculateCost(baseCost, currentLevel);

        if (saveData.totalCoins >= cost)
        {
            saveData.totalCoins -= cost;
            currentLevel++;
            
            SaveDataToJSON();
            UpdateCoinUI();
            uiUpdateAction.Invoke();
        }
    }

    private int CalculateCost(int baseCost, int currentLevel)
    {
        return baseCost * (currentLevel + 1);
    }

    private void UpdateAllUI()
    {
        UpdateCoinUI();
        UpdateDmgUI();
        UpdatePlayerHealthUI();
        UpdateMoveSpeedUI();
    }

    private void UpdateCoinUI()
    {
        if (totalCoinText != null) totalCoinText.text = $"GOLD: {saveData.totalCoins}";
    }

    private void UpdateDmgUI()
    {
        dmgLevelText.text = saveData.damageLevel >= dmgMaxLevel ? "LV. MAX" : $"LV. {saveData.damageLevel}";
        dmgCostText.text = saveData.damageLevel >= dmgMaxLevel ? "-" : $"{CalculateCost(dmgBaseCost, saveData.damageLevel)} G";
        dmgUpgradeButton.interactable = saveData.damageLevel < dmgMaxLevel;
    }

    private void UpdatePlayerHealthUI()
    {
        playerHealthLevelText.text = saveData.playerHealthLevel >= playerHealthMaxLevel ? "LV. MAX" : $"LV. {saveData.playerHealthLevel}";
        playerHealthCostText.text = saveData.playerHealthLevel >= playerHealthMaxLevel ? "-" : $"{CalculateCost(playerHealthBaseCost, saveData.playerHealthLevel)} G";
        playerHealthUpgradeButton.interactable = saveData.playerHealthLevel < playerHealthMaxLevel;
    }

    private void UpdateMoveSpeedUI()
    {
        moveSpeedLevelText.text = saveData.moveSpeedLevel >= moveSpeedMaxLevel ? "LV. MAX" : $"LV. {saveData.moveSpeedLevel}";
        moveSpeedCostText.text = saveData.moveSpeedLevel >= moveSpeedMaxLevel ? "-" : $"{CalculateCost(moveSpeedBaseCost, saveData.moveSpeedLevel)} G";
        moveSpeedUpgradeButton.interactable = saveData.moveSpeedLevel < moveSpeedMaxLevel;
    }

    private void LoadDataFromJSON()
    {
        if (File.Exists(savePath))
        {
            try
            {
                string json = File.ReadAllText(savePath);
                saveData = JsonUtility.FromJson<GameSaveData>(json);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.Message);
                saveData = new GameSaveData();
            }
        }
        else
        {
            saveData = new GameSaveData();
        }
    }

    private void SaveDataToJSON()
    {
        try
        {
            string json = JsonUtility.ToJson(saveData, true);
            File.WriteAllText(savePath, json);
        }
        catch (System.Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}