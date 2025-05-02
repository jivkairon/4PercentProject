using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class ActionPanel : MonoBehaviour
{
    [SerializeField] private bool startHidden = true;
    public TMP_Dropdown categoryDropdown; // Списък за категориите
    public TMP_Dropdown actionDropdown;  // Списък за действията
    public TextMeshProUGUI actionDescriptionText; // Описание на действие - текст
    public Button performActionButton; // Бутон - готово

    public TextMeshProUGUI regionNameText; // Текст - име на регион
    public TextMeshProUGUI playerInfluenceText; // Текст - влияние на играча
    public TextMeshProUGUI botInfluenceText;    // Текст - влияние на бота

    public RegionData selectedRegion; // Избран регион
    private Player player; // Играч
    private List<RegionData.ActionData> currentActions = new List<RegionData.ActionData>();

    // Референция към RegionInteraction скрипта
    private RegionInteraction regionInteraction;

    // Флаг, който показва дали панелът е в процес на завършване на действие
    private bool isCompletingAction = false;

    private bool isInitialized = false;

    void Awake()
    {
        // Скриваме панела в началото
        if (startHidden)
        {
            gameObject.SetActive(false);
        }
    }

    void Start()
    {
        // Инициализиране на панела
        InitializePanel();

        // Отбелязваме, че панелът е инициализиран
        isInitialized = true;

        // Намираме референция към RegionInteraction
        regionInteraction = FindObjectOfType<RegionInteraction>();
        if (regionInteraction == null)
        {
            Debug.LogError("RegionInteraction not found in scene! Flow management may not work correctly.");
        }

        // Намираме референция към играча
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogError("Player not found in scene! Spending money may not work correctly.");
        }
    }

    void InitializePanel()
    {
        // Проверка за UI елементи
        if (categoryDropdown == null) Debug.LogError("Category Dropdown is not assigned in ActionPanel!");
        if (actionDropdown == null) Debug.LogError("Action Dropdown is not assigned in ActionPanel!");
        if (actionDescriptionText == null) Debug.LogError("Action Description Text is not assigned in ActionPanel!");
        if (performActionButton == null) Debug.LogError("Perform Action Button is not assigned in ActionPanel!");
        if (regionNameText == null) Debug.LogError("Region Name Text is not assigned in ActionPanel!");
        if (playerInfluenceText == null) Debug.LogError("Player Influence Text is not assigned in ActionPanel!");
        if (botInfluenceText == null) Debug.LogError("Bot Influence Text is not assigned in ActionPanel!");

        // Инициализиране на dropdown за категории
        if (categoryDropdown != null)
        {
            categoryDropdown.ClearOptions();
            categoryDropdown.AddOptions(new List<string> { "Политически Кампании", "Социални Инициативи", "Икономически Мерки" });
            categoryDropdown.onValueChanged.AddListener(OnCategorySelected);
        }

        // Инициализиране на dropdown за действия
        if (actionDropdown != null)
        {
            actionDropdown.ClearOptions();
            actionDropdown.onValueChanged.AddListener(OnActionSelected);
        }

        // Инициализиране на бутон за действие
        if (performActionButton != null)
        {
            performActionButton.onClick.AddListener(OnPerformAction);
        }
    }

    public void ShowForRegion(RegionData region)
    {
        if (region == null)
        {
            Debug.LogError("Attempted to show ActionPanel with null region!");
            return;
        }

        // Ако вече сме в процес на завършване на действие, игнорираме
        if (isCompletingAction)
        {
            Debug.Log("Action is already being completed, ignoring new show request");
            return;
        }

        Debug.Log($"[ActionPanel] Showing for region: {region.regionName}");

        // Задаваме последен в йерархията, за да е най-отгоре
        transform.SetAsLastSibling();

        // Ако панелът не е инициализиран, инициализираме го
        if (!isInitialized)
        {
            InitializePanel();
            isInitialized = true;
        }

        // Обновяваме информацията за региона
        SetRegion(region);

        // Изрично активираме панела
        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        Debug.Log("[ActionPanel] Hiding panel");
        gameObject.SetActive(false);
        // Ресетваме флага при скриване на панела
        isCompletingAction = false;
    }

    public void SetRegion(RegionData region)
    {
        if (region == null)
        {
            Debug.LogError("Attempted to set null region in ActionPanel!");
            return;
        }

        selectedRegion = region;

        // Име на регион и влияния
        if (regionNameText != null)
        {
            regionNameText.text = region.regionName;
        }

        UpdateInfluenceText();

        // По подразбиране - първа категория
        if (categoryDropdown != null)
        {
            categoryDropdown.value = 0;
            OnCategorySelected(0);
        }

        // Връщане на списък за действия по подразбиране
        if (actionDropdown != null)
        {
            actionDropdown.value = 0;
            OnActionSelected(0);
        }
    }

    private void OnCategorySelected(int index)
    {
        if (selectedRegion == null)
        {
            Debug.LogWarning("OnCategorySelected called with null selectedRegion");
            return;
        }

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

            // Смяна на описание на действие (ако има действия)
            if (currentActions.Count > 0)
            {
                actionDropdown.value = 0;
                OnActionSelected(0);
            }
        }
    }

    private void OnActionSelected(int index)
    {
        if (selectedRegion == null || currentActions.Count == 0)
        {
            return;
        }

        if (index < 0 || index >= currentActions.Count)
        {
            Debug.LogWarning($"Invalid action index: {index}, max: {currentActions.Count - 1}");
            return;
        }

        // Обновяване на описание на действие
        var action = currentActions[index];
        if (actionDescriptionText != null)
        {
            actionDescriptionText.text = $"{action.name}\nЦена: {selectedRegion.GetActionCost(action):F1} лв.\nВлияние: {selectedRegion.GetActionInfluence(action):F1}%";
        }

        // Обновяване на влиянието според регион - не нужно тук
    }

    private void UpdateInfluenceText()
    {
        if (selectedRegion == null)
            return;

        if (playerInfluenceText != null && PlayerDataManager.Instance != null)
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
        // Проверяваме дали вече сме в процес на завършване на действие
        if (isCompletingAction)
        {
            Debug.Log("[ActionPanel] Action completion already in progress, ignoring button click");
            return;
        }

        if (selectedRegion == null || actionDropdown == null || player == null)
        {
            Debug.LogError("Cannot perform action: missing components");
            return;
        }

        if (actionDropdown.value < 0 || actionDropdown.value >= currentActions.Count)
        {
            Debug.LogError("Invalid action selected");
            return;
        }

        // Взимане на избрано действие
        var action = currentActions[actionDropdown.value];
        float cost = selectedRegion.GetActionCost(action);
        float influence = selectedRegion.GetActionInfluence(action);

        Debug.Log($"[ActionPanel] Performing action: {action.name}, Cost: {cost}, Influence: {influence}");

        // Изпълняване на действието, ако играча има пари
        if (player.SpendMoney(cost))
        {
            selectedRegion.UpdatePlayerInfluence(influence); // Ъпдейт на влияние на играча
            player.CalculateOverallInfluence(); // Изчисление на влияние на играча
            player.UpdateOverallInfluenceDisplay(); // Ъпдейт на текста на влиянието

            UpdateInfluenceText();

            // Маркираме, че сме в процес на завършване на действие
            isCompletingAction = true;

            // Деактивираме бутона, за да предотвратим множество кликвания
            performActionButton.interactable = false;

            // Изчакваме малко, за да може играчът да види промените и след това завършваме действието
            StartCoroutine(CompleteActionWithDelay());
        }
        else
        {
            Debug.LogWarning("Недостатъчен бюджет за изпълнение на действието.");
            // Показваме съобщение на играча, че няма достатъчно пари
            // Тук можете да добавите UI за показване на съобщение
        }
    }

    private IEnumerator CompleteActionWithDelay()
    {
        Debug.Log("[ActionPanel] Starting action completion delay");

        // Изчакваме малко, за да може играчът да види промените
        yield return new WaitForSeconds(1.0f);

        Debug.Log("[ActionPanel] Completing action after delay");

        // Скриваме панела преди да уведомим другите системи
        gameObject.SetActive(false);

        // Уведомяваме RegionInteraction, че действията са завършени
        if (regionInteraction != null)
        {
            Debug.Log("[ActionPanel] Notifying RegionInteraction that actions are complete");
            regionInteraction.OnActionsComplete();
        }
        else
        {
            Debug.LogError("[ActionPanel] RegionInteraction is null, cannot notify");
        }

        // Ресетваме флага за завършване на действие
        isCompletingAction = false;

        // Активираме бутона отново за следващия път
        if (performActionButton != null)
        {
            performActionButton.interactable = true;
        }
    }

    // Метод за бутон "Готово" (ако има отделен от performActionButton)
    public void OnDoneButtonClicked()
    {
        // Проверяваме дали вече сме в процес на завършване на действие
        if (isCompletingAction)
        {
            Debug.Log("[ActionPanel] Action completion already in progress, ignoring Done button");
            return;
        }

        Debug.Log("[ActionPanel] Done button clicked");

        // Маркираме, че сме в процес на завършване на действие
        isCompletingAction = true;

        // Скриваме панела преди да уведомим другите системи
        gameObject.SetActive(false);

        // Уведомяваме RegionInteraction, че действията са завършени
        if (regionInteraction != null)
        {
            Debug.Log("[ActionPanel] Notifying RegionInteraction about done action from button");
            regionInteraction.OnActionsComplete();
        }
        else
        {
            Debug.LogError("[ActionPanel] RegionInteraction is null, cannot notify from Done button");
        }

        // Ресетваме флага за завършване на действие
        isCompletingAction = false;
    }
}