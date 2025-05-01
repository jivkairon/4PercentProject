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

    private bool isInitialized = false;

   /* private void Awake()
    {
        // Скриваме панела в началото
        if (startHidden)
        {
            gameObject.SetActive(false);
        }
    }
   */
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
            Debug.LogWarning("RegionInteraction not found in scene! Flow management may not work correctly.");
        }

        // Намираме референция към играча
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            Debug.LogWarning("Player not found in scene! Spending money may not work correctly.");
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

        Debug.Log($"Showing ActionPanel for region: {region.regionName}");

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
        Debug.Log("Hiding ActionPanel");
        gameObject.SetActive(false);
    }

    private void UpdateRegionInfo(RegionData region)
    {
        if (region == null) return;

        selectedRegion = region;

        if (regionNameText != null)
            regionNameText.text = region.regionName;

        if (playerInfluenceText != null && PlayerDataManager.Instance != null)
            playerInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {region.playerInfluence:F1}%";

        if (botInfluenceText != null)
            botInfluenceText.text = $"Влияние на бота: {region.botInfluence:F1}%";

        // Зареждаме първата категория
        OnCategorySelected(0);
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

        if (playerInfluenceText != null && PlayerDataManager.Instance != null)
        {
            playerInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {region.playerInfluence:F1}%";
        }

        if (botInfluenceText != null)
        {
            botInfluenceText.text = $"Влияние на бота: {region.botInfluence:F1}%";
        }

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

        // Обновяване на влиянието според регион
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

        Debug.Log($"Performing action: {action.name}, Cost: {cost}, Influence: {influence}");

        // Изпълняване на действието, ако играча има пари
        if (player.SpendMoney(cost))
        {
            selectedRegion.UpdatePlayerInfluence(influence); // Ъпдейт на влияние на играча
            player.CalculateOverallInfluence(); // Изчисление на влияние на играча
            player.UpdateOverallInfluenceDisplay(); // Ъпдейт на текста на влиянието

            // Уведомяваме RegionInteraction скрипта, че действието е завършено
            if (regionInteraction != null)
            {
                // Изчакваме малко, за да може играчът да види промените
                StartCoroutine(CompleteActionWithDelay());
            }
            else
            {
                // Ако няма RegionInteraction, просто предаваме хода на бота
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.EndPlayerTurn();
                }
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Недостатъчен бюджет за изпълнение на действието.");
        }
    }

    private IEnumerator CompleteActionWithDelay()
    {
        // Изчакваме малко, за да може играчът да види промените
        yield return new WaitForSeconds(0.5f);

        gameObject.SetActive(false);

        // Уведомяваме RegionInteraction, че действията са завършени
        regionInteraction.OnActionsComplete();

        // Предаваме хода на бота и през GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndPlayerTurn();
        }
    }

    // Можем да използваме този метод за бутон "Готово" (ако има отделен от performActionButton)
    public void OnDoneButtonClicked()
    {
        // Уведомяваме RegionInteraction, че действията са завършени
        if (regionInteraction != null)
        {
            regionInteraction.OnActionsComplete();
        }

        // Предаваме хода на бота
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndPlayerTurn();
        }

        // Скриваме панела
        gameObject.SetActive(false);
    }
}

// Изпълняване на действи
/*
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

    private bool isInitialized = false;

    private void Awake()
    {
        // Задаваме висок priority, за да не бъде деактивиран от други скриптове
        transform.SetAsLastSibling();

        // Инициализиране на панела още в Awake
        InitializePanel();
    }

    void Start()
    {
        // Отбелязваме, че панелът е инициализиран
        isInitialized = true;

        // Намираме референция към RegionInteraction
        regionInteraction = FindObjectOfType<RegionInteraction>();
        if (regionInteraction == null)
        {
            Debug.LogWarning("RegionInteraction not found in scene! Flow management may not work correctly.");
        }

        // Скриваме панела в началото, ако е нужно
        if (startHidden)
        {
            gameObject.SetActive(false);
        }
    }

    void InitializePanel()
    {
        player = FindObjectOfType<Player>();

        if (categoryDropdown != null)
        {
            categoryDropdown.ClearOptions();
            categoryDropdown.AddOptions(new List<string> { "Политически Кампании", "Социални Инициативи", "Икономически Мерки" });
            categoryDropdown.onValueChanged.AddListener(OnCategorySelected);
        }
        else
        {
            Debug.LogError("Category Dropdown is not assigned in ActionPanel");
        }

        // Списък - действия
        if (actionDropdown != null)
        {
            actionDropdown.ClearOptions();
            actionDropdown.onValueChanged.AddListener(OnActionSelected);
        }
        else
        {
            Debug.LogError("Action Dropdown is not assigned in ActionPanel");
        }

        // Бутон - готово
        if (performActionButton != null)
        {
            performActionButton.onClick.AddListener(OnPerformAction);
        }
        else
        {
            Debug.LogError("Perform Action Button is not assigned in ActionPanel");
        }
    }

    public void ShowForRegion(RegionData region)
    {
        if (region == null)
        {
            Debug.LogError("Attempted to show ActionPanel with null region!");
            return;
        }

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
        gameObject.SetActive(false);
    }
    private void UpdateRegionInfo(RegionData region)
    {
        if (region == null) return;

        selectedRegion = region;

        if (regionNameText != null)
            regionNameText.text = region.regionName;

        if (playerInfluenceText != null && PlayerDataManager.Instance != null)
            playerInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {region.playerInfluence:F1}%";

        if (botInfluenceText != null)
            botInfluenceText.text = $"Влияние на бота: {region.botInfluence:F1}%";

        // Зареждаме първата категория
        OnCategorySelected(0);
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

        if (playerInfluenceText != null && PlayerDataManager.Instance != null)
        {
            playerInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {region.playerInfluence:F1}%";
        }

        if (botInfluenceText != null)
        {
            botInfluenceText.text = $"Влияние на бота: {region.botInfluence:F1}%";
        }

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

        // Обновяване на влиянието според регион
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

        // Изпълняване на действието, ако играча има пари
        if (player.SpendMoney(cost))
        {
            selectedRegion.UpdatePlayerInfluence(influence); // Ъпдейт на влияние на играча
            player.CalculateOverallInfluence(); // Изчисление на влияние на играча
            player.UpdateOverallInfluenceDisplay(); // Ъпдейт на текста на влиянието

            // Уведомяваме RegionInteraction скрипта, че действието е завършено
            if (regionInteraction != null)
            {
                // Изчакваме малко, за да може играчът да види промените
                StartCoroutine(CompleteActionWithDelay());
            }
            else
            {
                // Ако няма RegionInteraction, просто предаваме хода на бота
                if (GameManager.Instance != null)
                {
                    GameManager.Instance.EndPlayerTurn();
                }
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning("Недостатъчен бюджет за изпълнение на действието.");
        }
    }

    private IEnumerator CompleteActionWithDelay()
    {
        // Изчакваме малко, за да може играчът да види промените
        yield return new WaitForSeconds(0.5f);

        // Скриваме панела
        gameObject.SetActive(false);

        // Уведомяваме RegionInteraction, че действията са завършени
        regionInteraction.OnActionsComplete();

        // Предаваме хода на бота и през GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndPlayerTurn();
        }
    }

    // Можем да използваме този метод за бутон "Готово" (ако има отделен от performActionButton)
    public void OnDoneButtonClicked()
    {
        // Уведомяваме RegionInteraction, че действията са завършени
        if (regionInteraction != null)
        {
            regionInteraction.OnActionsComplete();
        }

        // Предаваме хода на бота
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndPlayerTurn();
        }

        // Скриваме панела
        gameObject.SetActive(false);
    }
}
*/