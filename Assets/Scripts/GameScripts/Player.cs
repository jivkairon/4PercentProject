using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Numerics;

public class Player : MonoBehaviour
{
    public static Player Instance;
    public float budget = 1000f; // Бюджет на играча
    public float overallInfluence = 0f; // Влияние на играча
    public TextMeshProUGUI budgetText; // Текст на бюджета
    public TextMeshProUGUI overallInfluenceText; // Текст на влиянието
    public Image colorSquare; // Снимка на цвят на партията

    private GameManager gameManager;

    void Start()
    {
        // Set the player's name and color
        if (budgetText != null)
        {
            budgetText.text = $"{PlayerDataManager.Instance.playerName} бюджет: {budget:F1} лв.";
        }
        if (overallInfluenceText != null)
        {
            overallInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {overallInfluence:F1}%";
        }
        if (colorSquare != null)
        {
            colorSquare.color = PlayerDataManager.Instance.playerColor;
        }

        overallInfluence = 0f;
        CalculateOverallInfluence();

        gameManager = FindObjectOfType<GameManager>();
        if (gameManager == null)
        {
            Debug.LogError("GameManager not found in the scene.");
        }
    }

    // Ъпдейт на текста на бюджета
    public void UpdateBudgetDisplay()
    {
        if (budgetText != null)
        {
            budgetText.text = $"Бюджет: {budget:F1} лв.";
        }
        else
        {
            Debug.LogError("Player budgetText is not assigned.");
        }
    }

    // Ъпдейт на текста на влиянието
    public void UpdateOverallInfluenceDisplay()
    {
        overallInfluence = Mathf.Clamp(overallInfluence, 0f, 100f);

        if (overallInfluenceText != null)
        {
            // Replace "Вашето" with the player's party name
            overallInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {overallInfluence:F1}%";
        }
        else
        {
            Debug.LogError("Player overallInfluenceText is not assigned in the inspector.");
        }
    }

    // Ъпдейт на текста на бюджета на играча
    public void UpdatePlayerBudgetDisplay()
    {
        if (budgetText != null)
        {
            budgetText.text = $"{PlayerDataManager.Instance.playerName} бюджет: {budget:F1} лв.";
        }
        else
        {
            Debug.LogError("Player budgetText is not assigned.");
        }   
    }

    // Метод за използване на пари и намаляване на бюджета
    public bool SpendMoney(float amount)
    {
        if (budget >= amount)
        {
            budget -= amount;
            UpdateBudgetDisplay();
            return true;
        }
        else
        {
            Debug.LogWarning("Недостатъчен бюджет.");
            return false;
        }
    }

    // Изчисление на влиянието
    public void CalculateOverallInfluence()
    {
        overallInfluence = 0f;
        RegionData[] regions = FindObjectsOfType<RegionData>();

        foreach (var region in regions)
        {
            overallInfluence += region.playerInfluence;
        }

        overallInfluence = Mathf.Clamp(overallInfluence, 0f, 100f);
        UpdateOverallInfluenceDisplay();
    }

    // При изпълнение на действие
    public void PerformAction(RegionData region, float cost, float influence)
    {
        if (SpendMoney(cost))
        {
            region.UpdatePlayerInfluence(influence); // Ъпдейт на влияние на играча върху регион

            CalculateOverallInfluence(); // Изчисляване на влияние на играча върху страната 

            gameManager.PlayerActionTaken(); // Съобщение към GameManager
        }
        else
        {
            Debug.LogWarning("Not enough budget to perform action!");
        }
    }

}