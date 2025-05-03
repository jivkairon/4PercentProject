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
    public bool isCity; // Дали е голям град

    [System.Serializable]
    public struct ActionData
    {
        public string name;
        public float baseCost;
        public float baseInfluence;
    }

    // Действия за политически кампании
    public ActionData[] politicalCampaigns = new ActionData[]
    {
        new ActionData { name = "Реч на площада", baseCost = 200, baseInfluence = 10 },
        new ActionData { name = "Плакати и билбордове", baseCost = 400, baseInfluence = 20 },
        new ActionData { name = "Телевизионна реклама", baseCost = 1000, baseInfluence = 40 }
    };

    // Социални инициативи
    public ActionData[] socialInitiatives = new ActionData[]
    {
        new ActionData { name = "Ремонт на училище", baseCost = 500, baseInfluence = 25 },
        new ActionData { name = "Безплатни здравни прегледи", baseCost = 800, baseInfluence = 30 },
        new ActionData { name = "Подкрепа за пенсионери", baseCost = 600, baseInfluence = 20 }
    };

    // Икономически мерки
    public ActionData[] economicMeasures = new ActionData[]
    {
        new ActionData { name = "Субсидии за земеделци", baseCost = 700, baseInfluence = 35 },
        new ActionData { name = "Намаляване на данъците", baseCost = 1200, baseInfluence = 50 },
        new ActionData { name = "Инфраструктурни проекти", baseCost = 1500, baseInfluence = 60 }
    };

    // Цена на действие според мандати
    public float GetActionCost(ActionData action)
    {
        if (mandates <= 0) return action.baseCost;
        return action.baseCost * (mandates / 240f);
    }
    public void ResetMarkerColor()
    {
        if (TryGetComponent<SpriteRenderer>(out var renderer))
        {
            renderer.color = Color.white;  // Или началния цвят на региона
        }
    }
    // Влияние на действие
    public float GetActionInfluence(ActionData action)
    {
        if (mandates <= 0) return action.baseInfluence;
        float baseValue = action.baseInfluence * (mandates / 240f);
        return isCity ? baseValue * 0.7f : baseValue * 1.3f;
    }

    // Процент влияние на играча
    public float GetPlayerInfluencePercentage()
    {
        if (mandates <= 0) return 0f;
        return (playerInfluence / mandates) * 100f;
    }

    // Процент влияние на бота
    public float GetBotInfluencePercentage()
    {
        if (mandates <= 0) return 0f;
        return (botInfluence / mandates) * 100f;
    }

    // Обновяване на визуализацията
    public void UpdateInfluenceDisplay()
    {
        playerInfluence = Mathf.Clamp(playerInfluence, 0f, mandates);
        botInfluence = Mathf.Clamp(botInfluence, 0f, mandates);

        if (playerInfluenceText != null)
        {
            playerInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {GetPlayerInfluencePercentage():F1}%";
        }
        if (botInfluenceText != null)
        {
            botInfluenceText.text = $"Влияние на бота: {GetBotInfluencePercentage():F1}%";
        }
    }

    // Обновяване на влиянието на играча
    public void UpdatePlayerInfluence(float amount)
    {
        playerInfluence += amount;
        UpdateInfluenceDisplay();
    }

    // Обновяване на влиянието на бота
    public void UpdateBotInfluence(float amount)
    {
        botInfluence += amount;
        UpdateInfluenceDisplay();
    }

    // Промяна на цвета на маркера
    public void SetMarkerColor(Color color)
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.color = color;
    }
}