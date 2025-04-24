using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class ActionPanel : MonoBehaviour
{
    public TMP_Dropdown categoryDropdown; // Списък за категориите
    public TMP_Dropdown actionDropdown;  // Списък за действията
    public TextMeshProUGUI actionDescriptionText; // Описание на действие - текст
    public Button performActionButton; // Бутон - готово

    public TextMeshProUGUI regionNameText; // Текст - име на регион
    public TextMeshProUGUI playerInfluenceText; // Текст - влияние на играча
    public TextMeshProUGUI botInfluenceText;    // Текст - влияние на бота

    private RegionData selectedRegion; // Избран регион
    private Player player; // Играч
    private List<RegionData.ActionData> currentActions = new List<RegionData.ActionData>();

    void Start()
    {
        player = FindObjectOfType<Player>();
        // Проверка дали играча е избран
        if (player == null)
        {
            Debug.LogError("Player not found in the scene.");
        }

        // Списък - категории
        if (categoryDropdown != null)
        {
            categoryDropdown.ClearOptions();
            categoryDropdown.AddOptions(new List<string> { "Политически Кампании", "Социални Инициативи", "Икономически Мерки" });
            categoryDropdown.onValueChanged.AddListener(OnCategorySelected);
        }
        else
        {
            Debug.LogError("Category Dropdown is not assigned.");
        }

        // Списък - действия
        if (actionDropdown != null)
        {
            actionDropdown.ClearOptions();
            actionDropdown.onValueChanged.AddListener(OnActionSelected);
        }
        else
        {
            Debug.LogError("Action Dropdown is not assigned.");
        }

        // Бутон - готово
        if (performActionButton != null)
        {
            performActionButton.onClick.AddListener(OnPerformAction);
        }
        else
        {
            Debug.LogError("Perform Action Button is not assigned.");
        }

        // Скрит панел в началото
        gameObject.SetActive(false);
    }

    public void SetRegion(RegionData region)
    {
        selectedRegion = region;

        // Име на регион и влияния
        if (regionNameText != null)
        {
            regionNameText.text = region.regionName;
        }
        if (playerInfluenceText != null)
        {
            playerInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {region.playerInfluence:F1}%";
        }
        if (botInfluenceText != null)
        {
            botInfluenceText.text = $"Влияние на бота: {region.botInfluence:F1}%";
        }

        gameObject.SetActive(true);
        OnCategorySelected(0); // По подразбиране - списък

        // Връщане на списък за действия по подразбиране
        if (actionDropdown != null)
        {
            actionDropdown.value = 0; 
        }
    }

    private void OnCategorySelected(int index)
    {
        if (selectedRegion == null) return;

        // Изчистване на сегашните действия
        currentActions.Clear();

        // Списък за действия според избрана категория
        switch (index)
        {
            case 0:
                currentActions.AddRange(selectedRegion.politicalCampaigns);
                break;
            case 1:
                currentActions.AddRange(selectedRegion.socialInitiatives);
                break;
            case 2:
                currentActions.AddRange(selectedRegion.economicMeasures);
                break;
        }

        // Нулиране на списък за действия
        if (actionDropdown != null)
        {
            actionDropdown.ClearOptions();
            List<string> actionNames = new List<string>();
            foreach (var action in currentActions)
            {
                actionNames.Add($"{action.name} (Цена: {selectedRegion.GetActionCost(action):F1} лв.)");
            }
            actionDropdown.AddOptions(actionNames);
        }

        // Смяна на описание на действие
        OnActionSelected(0);
    }

   private void OnActionSelected(int index)
{
    if (selectedRegion == null || index < 0 || index >= currentActions.Count || currentActions.Count == 0) return;

    // Обновяване на описание на действие
    var action = currentActions[index];
    if (actionDescriptionText != null)
    {
        actionDescriptionText.text = $"{action.name}\nЦена: {selectedRegion.GetActionCost(action):F1} лв.\nВлияние: {selectedRegion.GetActionInfluence(action):F1}%";
    }

    // Обновяване на влиянието според регион
    if (playerInfluenceText != null)
    {
        playerInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {selectedRegion.GetPlayerInfluencePercentage():F1}%";
    }
    if (botInfluenceText != null)
    {
        botInfluenceText.text = $"Влияние на бота: {selectedRegion.GetBotInfluencePercentage():F1}%";
    }
}

    private void OnPerformAction()
    {
        if (selectedRegion == null || actionDropdown.value < 0 || actionDropdown.value >= currentActions.Count || player == null) return;

        // Взимане на избрано действие
        var action = currentActions[actionDropdown.value];
        float cost = selectedRegion.GetActionCost(action);
        float influence = selectedRegion.GetActionInfluence(action);

        // Изпълняване на действието, ако играча има пари
        if (player.SpendMoney(cost))
        {
            Debug.Log($"Performing action: {action.name} in {selectedRegion.regionName} | Cost: {cost}, Influence: {influence}");

            selectedRegion.UpdatePlayerInfluence(influence); // Ъпдейт на влияние на играча
            player.CalculateOverallInfluence(); // Изчисление на влияние на играча
            player.UpdateOverallInfluenceDisplay(); // Ъпдейт на текста на влиянието

            Debug.Log($"New Player Overall Influence: {player.overallInfluence}");

            gameObject.SetActive(false); // Скриване на панел
        }
        else
        {
            Debug.LogWarning("Недостатъчен бюджет за изпълнение на действието.");
        }
    }

}