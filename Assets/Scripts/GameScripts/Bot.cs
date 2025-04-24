using UnityEngine;
using TMPro;

public class Bot : MonoBehaviour
{
    public float budget = 1000f; // Бюджет на бота
    public float overallInfluence = 0f; // Влияние на бота
    public TextMeshProUGUI budgetText; // Текст на бюджета
    public TextMeshProUGUI overallInfluenceText; // Текст на влиянието

    void Start()
    {
        overallInfluence = 0f;
        UpdateOverallInfluenceDisplay();
        CalculateOverallInfluence();
    }

    // Ъпдейт на текста на бюджета
    public void UpdateBudgetDisplay()
    {
        if (budgetText != null)
        {
            budgetText.text = $"Бюджет на бота: {budget:F1} лв.";
        }
        else
        {
            Debug.LogError("Bot budgetText is not assigned.");
        }
    }

    // Ъпдейт на текста на влиянието
    public void UpdateOverallInfluenceDisplay()
    {
        overallInfluence = Mathf.Clamp(overallInfluence, 0f, 100f);

        Debug.Log($"Updating Overall Influence Display: {overallInfluence:F1}%");

        if (overallInfluenceText != null)
        {
            overallInfluenceText.text = $"Влияние на бота: {overallInfluence:F1}%";
        }
        else
        {
            Debug.LogError("Bot overallInfluenceText is not assigned.");
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
            Debug.LogWarning("Недостатъчен бюджет на бота.");
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
            overallInfluence += region.botInfluence;
            Debug.Log($"Region {region.regionName} Bot Influence: {region.botInfluence}");
        }
        overallInfluence = Mathf.Clamp(overallInfluence, 0f, 100f);
        Debug.Log($"Calculated Bot Overall Influence: {overallInfluence}");
        UpdateOverallInfluenceDisplay();
    }

    // При изпълнение на действие
    public void PerformAction(RegionData region, float cost, float influence)
    {
        if (SpendMoney(cost))
        {
            region.UpdateBotInfluence(influence); // Ъпдейт на влияние на бота върху регион
            overallInfluence += influence; // Ъпдейт на влияние на бота върху страната 
            UpdateOverallInfluenceDisplay(); // Ъпдейт на текста за влияние
        }
    }
}