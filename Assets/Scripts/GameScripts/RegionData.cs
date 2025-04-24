using UnityEngine;
using TMPro;

public class RegionData : MonoBehaviour
{
    public string regionName;
    public int mandates;
    public float playerInfluence;
    public float botInfluence;

    public TextMeshProUGUI playerInfluenceText;
    public TextMeshProUGUI botInfluenceText;

    // Базови цени и влияния на действия
    [System.Serializable]
    public struct ActionData
    {
        public string name;
        public float baseCost;
        public float baseInfluence;
    }

    public ActionData[] politicalCampaigns = new ActionData[]
    {
        new ActionData { name = "Реч на площада", baseCost = 200, baseInfluence = 10 },
        new ActionData { name = "Плакати и билбордове", baseCost = 400, baseInfluence = 20 },
        new ActionData { name = "Телевизионна реклама", baseCost = 1000, baseInfluence = 40 }
    };

    public ActionData[] socialInitiatives = new ActionData[]
    {
        new ActionData { name = "Ремонт на училище", baseCost = 500, baseInfluence = 25 },
        new ActionData { name = "Безплатни здравни прегледи", baseCost = 800, baseInfluence = 30 },
        new ActionData { name = "Подкрепа за пенсионери", baseCost = 600, baseInfluence = 20 }
    };

    public ActionData[] economicMeasures = new ActionData[]
    {
        new ActionData { name = "Субсидии за земеделци", baseCost = 700, baseInfluence = 35 },
        new ActionData { name = "Намаляване на данъците", baseCost = 1200, baseInfluence = 50 },
        new ActionData { name = "Инфраструктурни проекти", baseCost = 1500, baseInfluence = 60 }
    };

    // Цена на действие според брой мандати
    public float GetActionCost(ActionData action)
    {
        return action.baseCost * (mandates / 240f); 
    }

    // Влияние на действие според брой мандати
    public float GetActionInfluence(ActionData action)
    {
        return action.baseInfluence * (mandates / 240f); 
    }

    // Изчисляване на влияние върху регион като процент от мандати на регион
    public float GetPlayerInfluencePercentage()
    {
        return (playerInfluence / mandates) * 100f;
    }

    public float GetBotInfluencePercentage()
    {
        return (botInfluence / mandates) * 100f;
    }

    // Ъпдейт на текст на влияние
    public void UpdateInfluenceDisplay()
    {
        playerInfluence = Mathf.Clamp(playerInfluence, 0f, mandates);
        botInfluence = Mathf.Clamp(botInfluence, 0f, mandates);

        // Изчисляване на проценти на влияние за регион
        float playerInfluencePercent = GetPlayerInfluencePercentage();
        float botInfluencePercent = GetBotInfluencePercentage();

        // Ъпдейт на текста
        if (playerInfluenceText != null)
        {
            playerInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {playerInfluencePercent:F1}%";
        }
        if (botInfluenceText != null)
        {
            botInfluenceText.text = $"Влияние на бота: {botInfluencePercent:F1}%";
        }
    }

    // Ъпдейт на влиянието в регион
    public void UpdatePlayerInfluence(float amount)
    {
        playerInfluence += amount;
        playerInfluence = Mathf.Clamp(playerInfluence, 0f, 100f);
        UpdateInfluenceDisplay();

        Debug.Log($"Updated {regionName} player influence: {playerInfluence}");
    }

    public void UpdateBotInfluence(float amount)
    {
        botInfluence += amount;
        botInfluence = Mathf.Clamp(botInfluence, 0f, 100f);
        UpdateInfluenceDisplay();
    }

}