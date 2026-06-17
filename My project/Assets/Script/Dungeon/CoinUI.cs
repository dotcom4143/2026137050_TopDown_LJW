using UnityEngine;
using TMPro;

public class CoinUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI coinText;

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
    }
}