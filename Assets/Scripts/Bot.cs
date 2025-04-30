using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UIElements;

public class Bot : MonoBehaviour
{
    [Header("Бюджет и влияние")]
    public float budget = 5000f; // Обединен бюджет (използваме по-голямата стойност)
    public float overallInfluence = 0f;

    [Header("UI Елементи")]
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI overallInfluenceText;
    public TextMeshProUGUI botActionText;

    [Header("Настройки на бота")]
    public float decisionDelay = 2f;

    private RegionData[] allRegions;
    private float timer;

    void Start()
    {
        InitializeBot();

        // Проверка за UI елементите
        if (botActionText == null)
        {
            Debug.LogError("botActionText не е зададен в инспектора!", this);
        }

        // Изчисти текста при стартиране
        if (botActionText != null)
        {
            botActionText.text = "";
        }
    }

    void Update()
    {
        if (GameManager.Instance.isPlayerTurn) return;
        HandleBotAI();
    }

    public bool SpendMoney(float amount)
    {
        if (budget >= amount)
        {
            budget -= amount;
            UpdateBudgetDisplay();
            return true;
        }
        Debug.LogWarning("Недостатъчен бюджет на бота.");
        return false;
    }

    private void InitializeBot()
    {
        allRegions = FindObjectsOfType<RegionData>();
        timer = decisionDelay;
        UpdateAllDisplays();
    }

    private void UpdateAllDisplays()
    {
        UpdateBudgetDisplay();
        UpdateOverallInfluenceDisplay();
    }

    public void UpdateBudgetDisplay()
    {
        if (budgetText != null)
        {
            budgetText.text = $"Бюджет на бота: {budget:F1} лв.";
        }
    }

    public void UpdateOverallInfluenceDisplay()
    {
        overallInfluence = Mathf.Clamp(overallInfluence, 0f, 100f);
        if (overallInfluenceText != null)
        {
            overallInfluenceText.text = $"Влияние на бота: {overallInfluence:F1}%";
        }
    }

    private void HandleBotAI()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            MakeBotDecision();
            timer = decisionDelay;
        }
    }

    private void MakeBotDecision()
    {
        RegionData targetRegion = ChooseTargetRegion();
        RegionData.ActionData chosenAction = ChooseAction(targetRegion);

        if (CanAffordAction(chosenAction.baseCost))
        {
            ExecuteAction(targetRegion, chosenAction);
        }
        else
        {
            // Ако ботът не може да си позволи действието, предава хода обратно на играча
            GameManager.Instance.StartPlayerTurn();
        }
    }

    private bool CanAffordAction(float cost)
    {
        return budget >= cost;
    }

    private void ExecuteAction(RegionData region, RegionData.ActionData action)
    {
        budget -= action.baseCost;
        region.UpdateBotInfluence(action.baseInfluence);
        overallInfluence += action.baseInfluence;

        UpdateAllDisplays();

        // Показваме действието на бота
        string actionMessage = $"{action.name} в {region.regionName}";
        Debug.Log($"Ботът изпълнява: {actionMessage}");

        // ПОПРАВЕНО: Правилно стартиране на корутината
        StartCoroutine(ShowActionFeedback(actionMessage));

        // След малко закъснение, предаваме хода обратно на играча
        StartCoroutine(EndBotTurnAfterDelay(2.0f));
    }

    private IEnumerator ShowActionFeedback(string message)
    {
        if (botActionText != null)
        {
            Debug.Log($"Показване на текст за бота: {message}");
            botActionText.text = $"Ботът: {message}";
            yield return new WaitForSeconds(3f);
            botActionText.text = "";
        }
        else
        {
            Debug.LogError("botActionText е null в ShowActionFeedback!");
            yield return null;
        }
    }

    private IEnumerator EndBotTurnAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Проверка дали GameManager съществува
   /*     if (GameManager.Instance != null)
        {
            GameManager.Instance.EndBotTurn();
        }
        else
        {
            Debug.LogError("GameManager.Instance е null в EndBotTurnAfterDelay!");
        }
   */
    }

    private RegionData ChooseTargetRegion()
    {
        // Проверяваме дали имаме региони
        if (allRegions == null || allRegions.Length == 0)
        {
            Debug.LogError("Няма намерени региони за бота!");
            allRegions = FindObjectsOfType<RegionData>();

            if (allRegions.Length == 0)
            {
                // Връщаме null, ако наистина няма региони
                return null;
            }
        }

        return allRegions
            .OrderBy(r => r.botInfluence - r.playerInfluence * 0.5f)
            .First();
    }

    private RegionData.ActionData ChooseAction(RegionData target)
    {
      /*  if (target == null)
        {
            Debug.LogError("Опит за избор на действие с null регион!");
            return null;
        }
      */
        if (target.GetPlayerInfluencePercentage() > 60)
        {
            return target.economicMeasures[1];
        }

        if (budget < 1000)
        {
            return target.politicalCampaigns[0];
        }

        int randomCategory = Random.Range(0, 3);
        return randomCategory switch
        {
            0 => target.politicalCampaigns[Random.Range(0, target.politicalCampaigns.Length)],
            1 => target.socialInitiatives[Random.Range(0, target.socialInitiatives.Length)],
            _ => target.economicMeasures[Random.Range(0, target.economicMeasures.Length)]
        };
    }

    public void CalculateOverallInfluence()
    {
        overallInfluence = 0f;

        // Защита срещу null
        if (allRegions == null || allRegions.Length == 0)
        {
            allRegions = FindObjectsOfType<RegionData>();
        }

        foreach (var region in allRegions)
        {
            overallInfluence += region.botInfluence;
        }

        overallInfluence = Mathf.Clamp(overallInfluence, 0f, 100f);
        UpdateOverallInfluenceDisplay();
    }

    // Ъпдейт на текста на бюджета на бота
    public void UpdateBotBudgetDisplay()
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
}
/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Bot : MonoBehaviour
{
    [Header("Бюджет и влияние")]
    public float budget = 5000f; // Обединен бюджет (използваме по-голямата стойност)
    public float overallInfluence = 0f;

    [Header("UI Елементи")]
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI overallInfluenceText;
    public TextMeshProUGUI botActionText;

    [Header("Настройки на бота")]
    public float decisionDelay = 2f;

    private RegionData[] allRegions;
    private float timer;

    void Start()
    {
        InitializeBot();
    }

    void Update()
    {
        if (GameManager.Instance.isPlayerTurn) return;
        HandleBotAI();
    }

    public bool SpendMoney(float amount)
    {
        if (budget >= amount)
        {
            budget -= amount;
            UpdateBudgetDisplay();
            return true;
        }
        Debug.LogWarning("Недостатъчен бюджет на бота.");
        return false;
    }

    private void InitializeBot()
    {
        allRegions = FindObjectsOfType<RegionData>();
        timer = decisionDelay;
        UpdateAllDisplays();
    }

    private void UpdateAllDisplays()
    {
        UpdateBudgetDisplay();
        UpdateOverallInfluenceDisplay();
    }

    public void UpdateBudgetDisplay()
    {
        if (budgetText != null)
        {
            budgetText.text = $"Бюджет на бота: {budget:F1} лв.";
        }
    }

    public void UpdateOverallInfluenceDisplay()
    {
        overallInfluence = Mathf.Clamp(overallInfluence, 0f, 100f);
        if (overallInfluenceText != null)
        {
            overallInfluenceText.text = $"Влияние на бота: {overallInfluence:F1}%";
        }
    }

    private void HandleBotAI()
    {
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            MakeBotDecision();
            timer = decisionDelay;
        }
    }

    private void MakeBotDecision()
    {
        RegionData targetRegion = ChooseTargetRegion();
        RegionData.ActionData chosenAction = ChooseAction(targetRegion);

        if (CanAffordAction(chosenAction.baseCost))
        {
            ExecuteAction(targetRegion, chosenAction);
        }
    }

    private bool CanAffordAction(float cost)
    {
        return budget >= cost;
    }

    private void ExecuteAction(RegionData region, RegionData.ActionData action)
    {
        budget -= action.baseCost;
        region.UpdateBotInfluence(action.baseInfluence);
        overallInfluence += action.baseInfluence;

        UpdateAllDisplays();
        ShowActionFeedback($"{action.name} в {region.regionName}");
    }

    private IEnumerator ShowActionFeedback(string message)
    {
        if (botActionText != null)
        {
            botActionText.text = $"Ботът: {message}";
            yield return new WaitForSeconds(3f);
            botActionText.text = "";
        }
    }

    private RegionData ChooseTargetRegion()
    {
        return allRegions
            .OrderBy(r => r.botInfluence - r.playerInfluence * 0.5f)
            .First();
    }

    private RegionData.ActionData ChooseAction(RegionData target)
    {
        if (target.GetPlayerInfluencePercentage() > 60)
        {
            return target.economicMeasures[1];
        }

        if (budget < 1000)
        {
            return target.politicalCampaigns[0];
        }

        int randomCategory = Random.Range(0, 3);
        return randomCategory switch
        {
            0 => target.politicalCampaigns[Random.Range(0, 3)],
            1 => target.socialInitiatives[Random.Range(0, 3)],
            _ => target.economicMeasures[Random.Range(0, 3)]
        };
    }

    public void CalculateOverallInfluence()
    {
        overallInfluence = 0f;
        foreach (var region in allRegions)
        {
            overallInfluence += region.botInfluence;
        }
        overallInfluence = Mathf.Clamp(overallInfluence, 0f, 100f);
        UpdateOverallInfluenceDisplay();
    }
}
*/