using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Bot : MonoBehaviour
{
    [Header("������ � �������")]
    public float budget = 5000f; // �������� ������ (���������� ��-�������� ��������)
    public float overallInfluence = 0f;

    [Header("UI ��������")]
    public TextMeshProUGUI budgetText;
    public TextMeshProUGUI overallInfluenceText;
    public TextMeshProUGUI botActionText;

    [Header("��������� �� ����")]
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
        Debug.LogWarning("������������ ������ �� ����.");
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
            budgetText.text = $"������ �� ����: {budget:F1} ��.";
        }
    }

    public void UpdateOverallInfluenceDisplay()
    {
        overallInfluence = Mathf.Clamp(overallInfluence, 0f, 100f);
        if (overallInfluenceText != null)
        {
            overallInfluenceText.text = $"������� �� ����: {overallInfluence:F1}%";
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
        ShowActionFeedback($"{action.name} � {region.regionName}");
    }

    private IEnumerator ShowActionFeedback(string message)
    {
        if (botActionText != null)
        {
            botActionText.text = $"�����: {message}";
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