using UnityEngine;
using TMPro;
using System.IO;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;
    private string savePath;

    private void OnEnable()
    {
        DataManager.OnCoinChanged += UpdateCoinText;
    }

    private void OnDisable()
    {
        DataManager.OnCoinChanged -= UpdateCoinText;
    }

    private void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "SaveData.json");

        if (DataManager.Instance != null)
        {
            UpdateCoinText(DataManager.Instance.data.totalCoins);
        }
    }

    private void UpdateCoinText(int currentCoins)
    {
        if (coinText != null)
        {
            coinText.text = currentCoins.ToString();
        }

        if (DataManager.Instance != null)
        {
            DataManager.Instance.data.totalCoins = currentCoins;
        }

        try
        {
            SettingsMenuController.GameSaveData data = new SettingsMenuController.GameSaveData();

            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                data = JsonUtility.FromJson<SettingsMenuController.GameSaveData>(json);
            }

            data.totalCoins = currentCoins;

            string updatedJson = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, updatedJson);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"[CoinUI] JSON 파일 직접 저장 실패: {e.Message}");
        }
    }
}