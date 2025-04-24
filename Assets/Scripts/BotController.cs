using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class BotController : MonoBehaviour
{
    public float botMoney = 5000f; // �������� ������
    public float botDecisionDelay = 2f; // �������� ����� ��������

    private RegionData[] allRegions;
    private float timer;


    public TextMeshProUGUI botActionText; // ���������� � Inspector

    private IEnumerator ShowBotAction(string message)
    {
        botActionText.text = message;
        yield return new WaitForSeconds(3f);
        botActionText.text = "";
    }

    private void Start()
    {
        // ������ ������ ������� � �������
        allRegions = FindObjectsOfType<RegionData>();
        timer = botDecisionDelay;
    }

    private void Update()
    {
        // ����� ����� ��� �� ����� 'botDecisionDelay' �������
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            MakeBotDecision();
            timer = botDecisionDelay;
        }
    }

    private void MakeBotDecision()
    {
        // 1. ����� �� ������ ������
        RegionData targetRegion = ChooseTargetRegion();

        // 2. ����� �� ��������
        RegionData.ActionData chosenAction = ChooseAction(targetRegion);

        // 3. ��������� �� ����������
        if (botMoney >= chosenAction.baseCost)
        {
            botMoney -= chosenAction.baseCost;
            targetRegion.UpdateBotInfluence(chosenAction.baseInfluence);
            StartCoroutine(ShowBotAction($"�����: {chosenAction.name} � {targetRegion.regionName}"));
        }
        /*  if (botMoney >= chosenAction.baseCost)
          {
              botMoney -= chosenAction.baseCost;
              targetRegion.UpdateBotInfluence(chosenAction.baseInfluence);
              Debug.Log($"����� �����: {chosenAction.name} � {targetRegion.regionName}");
          }
        */
    }

    private RegionData ChooseTargetRegion()
    {
        // ������������ ������� �:
        // - ���-����� ������� �� ����
        // - ���-������ ������� �� ������ (�� �����������)
        return allRegions
            .OrderBy(r => r.botInfluence - r.playerInfluence * 0.5f)
            .First();
    }

    private RegionData.ActionData ChooseAction(RegionData target)
    {
        // ���� �������� ������ ���: RegionData.ActionData
        if (target.GetPlayerInfluencePercentage() > 60)
        {
            return target.economicMeasures[1];
        }
        else if (botMoney < 1000)
        {
            return target.politicalCampaigns[0];
        }
        else
        {
            int randomCategory = Random.Range(0, 3);
            switch (randomCategory)
            {
                case 0: return target.politicalCampaigns[Random.Range(0, 3)];
                case 1: return target.socialInitiatives[Random.Range(0, 3)];
                default: return target.economicMeasures[Random.Range(0, 3)];
            }
        }
    }



}