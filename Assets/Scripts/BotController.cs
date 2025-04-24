using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class BotController : MonoBehaviour
{
    public float botMoney = 5000f; // Примерен бюджет
    public float botDecisionDelay = 2f; // Забавяне между ходовете

    private RegionData[] allRegions;
    private float timer;


    public TextMeshProUGUI botActionText; // Референция в Inspector

    private IEnumerator ShowBotAction(string message)
    {
        botActionText.text = message;
        yield return new WaitForSeconds(3f);
        botActionText.text = "";
    }

    private void Start()
    {
        // Намира всички региони в сцената
        allRegions = FindObjectsOfType<RegionData>();
        timer = botDecisionDelay;
    }

    private void Update()
    {
        // Ботът прави ход на всеки 'botDecisionDelay' секунди
        timer -= Time.deltaTime;
        if (timer <= 0)
        {
            MakeBotDecision();
            timer = botDecisionDelay;
        }
    }

    private void MakeBotDecision()
    {
        // 1. Избор на целеви регион
        RegionData targetRegion = ChooseTargetRegion();

        // 2. Избор на действие
        RegionData.ActionData chosenAction = ChooseAction(targetRegion);

        // 3. Прилагане на действието
        if (botMoney >= chosenAction.baseCost)
        {
            botMoney -= chosenAction.baseCost;
            targetRegion.UpdateBotInfluence(chosenAction.baseInfluence);
            StartCoroutine(ShowBotAction($"Ботът: {chosenAction.name} в {targetRegion.regionName}"));
        }
        /*  if (botMoney >= chosenAction.baseCost)
          {
              botMoney -= chosenAction.baseCost;
              targetRegion.UpdateBotInfluence(chosenAction.baseInfluence);
              Debug.Log($"Ботът избра: {chosenAction.name} в {targetRegion.regionName}");
          }
        */
    }

    private RegionData ChooseTargetRegion()
    {
        // Приоритизира региони с:
        // - Най-ниско влияние на бота
        // - Най-високо влияние на играча (за контраатака)
        return allRegions
            .OrderBy(r => r.botInfluence - r.playerInfluence * 0.5f)
            .First();
    }

    private RegionData.ActionData ChooseAction(RegionData target)
    {
        // Явно указваме пълния път: RegionData.ActionData
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