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

    private RegionData selectedRegion; // Избран регион
    private Player player; // Играч
    private List<RegionData.ActionData> currentActions = new List<RegionData.ActionData>();

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

            gameObject.SetActive(false); // Скриване на панел
        }
        else
        {
            Debug.LogWarning("Недостатъчен бюджет за изпълнение на действието.");
        }

        // Предава хода на бота
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EndPlayerTurn();
        }
    }

    // Example button callback
    public void OnCampaignButtonClicked()
    {
        var action = currentActions[actionDropdown.value];
        float cost = selectedRegion.GetActionCost(action);
        GameManager.Instance.PerformPlayerAction(cost); // or different cost
    }
}


/*using UnityEngine;
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

    private RegionData selectedRegion; // Избран регион
    private Player player; // Играч
    private List<RegionData.ActionData> currentActions = new List<RegionData.ActionData>();

    private bool isInitialized = false;
    void OnEnable() => Debug.Log("Панелът се активира!");
    void OnDisable() => Debug.Log("Панелът се деактивира!");
    private void Awake()
    {
        // Задаваме висок priority, за да не бъде деактивиран от други скриптове
        transform.SetAsLastSibling();
    }

  
    void Start()
    {
        gameObject.SetActive(false);

        Debug.Log("ActionPanel initialized and ready to use");

     //   InitializePanel();

    //    if (startHidden) StartCoroutine(DelayedHide());
    }
    IEnumerator DelayedHide()
    {
        yield return new WaitForEndOfFrame();
        gameObject.SetActive(false);
    }
    public void ShowForRegion(RegionData region)
    {
        transform.SetAsLastSibling();
        UpdateRegionInfo(region);
        gameObject.SetActive(true);
    }
    IEnumerator DelayedEnable()
    {
        yield return new WaitForEndOfFrame(); // Изчаква края на кадъра
        if (!isInitialized)
        {
            gameObject.SetActive(true);
            isInitialized = true;
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
     
    }
    private void UpdateRegionInfo(RegionData region)
    {
        selectedRegion = region;
        regionNameText.text = region.regionName;
        playerInfluenceText.text = $"{PlayerDataManager.Instance.playerName} влияние: {region.playerInfluence:F1}%";
        botInfluenceText.text = $"Влияние на бота: {region.botInfluence:F1}%";
        OnCategorySelected(0); // Зарежда първата категория
    }

    // Публичен метод за показване (извиква се от RegionInteraction)
   
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
        GameManager.Instance.EndPlayerTurn(); // Важно! Предава хода на бота
    }

}
*/