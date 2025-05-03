using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RegionInteraction : MonoBehaviour
{
    public enum TurnState
    {
        PlayerSelection,
        PlayerQuestion,
        PlayerAction,
        BotSelection,
        BotQuestion,
        BotAction
    }

    // Референции към UI елементи
    public QuestionPanel playerQuestionPanel;
    public QuestionPanel botQuestionPanel;
    public ActionPanel actionPanel;
    public GameManager gameManager;

    // Настройки
    public bool debugRegions = false;
    public Color botColor = Color.red;

    private TurnState currentState;
    private RegionData selectedRegion;
    private Dictionary<string, bool> selectedRegions = new Dictionary<string, bool>();

    // Флаг, който показва дали е в процес на обработка на действие
    private bool isProcessingAction = false;

    #region Инициализация
    public static RegionInteraction Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
        // Проверка за налични панели
        if (playerQuestionPanel == null)
        {
            Debug.LogError("Player Question Panel is not assigned in RegionInteraction!");
        }

        if (botQuestionPanel == null)
        {
            Debug.LogError("Bot Question Panel is not assigned in RegionInteraction!");
        }

        if (actionPanel == null)
        {
            Debug.LogError("Action Panel is not assigned in RegionInteraction!");
        }

        InitializeGame();
    }

    void InitializeGame()
    {
        selectedRegions.Clear();
        HideAllPanels();
        SetTurnState(TurnState.PlayerSelection);
    }
    #endregion

    #region Управление на панели
    void HideAllPanels()
    {
        Debug.Log("[RegionInteraction] Hiding all panels");
        if (playerQuestionPanel != null) playerQuestionPanel.HidePanel();
        if (botQuestionPanel != null) botQuestionPanel.HidePanel();
        if (actionPanel != null) actionPanel.HidePanel();
    }
    #endregion

    #region Логика на играча
    void Update()
    {
        // Debug информация
        Debug.Log($"State: {currentState} | Selected: {selectedRegion?.regionName} | Processing: {isProcessingAction}");
        // Проверяваме дали е ход на играча за избор на регион и дали не е в процес на обработка
        if (currentState == TurnState.PlayerSelection && !isProcessingAction && Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            HandlePlayerSelection();
        }
    }

    public void ResetAllPanels()
    {
        Debug.Log("[RegionInteraction] Resetting all panels");
        if (playerQuestionPanel != null) playerQuestionPanel.HidePanel();
        if (botQuestionPanel != null) botQuestionPanel.HidePanel();
        if (actionPanel != null) actionPanel.HidePanel();
    }

    public RegionData GetSelectedRegion() => selectedRegion;

    void HandlePlayerSelection()
    {
        // Блокираме по-нататъшна обработка
        isProcessingAction = true;

        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null)
        {
            isProcessingAction = false;
            return;
        }

        RegionData region = hit.collider.GetComponent<RegionData>();
        if (region == null || selectedRegions.ContainsKey(region.regionName))
        {
            isProcessingAction = false;
            return;
        }

        Debug.Log($"[RegionInteraction] Player selected region: {region.regionName}");
        selectedRegion = region;
        selectedRegions[region.regionName] = true;
        MarkRegionAsSelected(region, PlayerDataManager.Instance.playerColor);

        SetTurnState(TurnState.PlayerQuestion);
    }

    // Извиква се при верен отговор на въпрос
    public void OnQuestionAnsweredCorrectly()
    {
        Debug.Log("[RegionInteraction] Question answered correctly! Moving to action panel.");
        SetTurnState(TurnState.PlayerAction);
    }

    // Извиква се при грешен отговор на въпрос
    public void OnQuestionAnsweredIncorrectly()
    {
        Debug.Log("[RegionInteraction] Question answered incorrectly! Moving to bot selection.");
        if (selectedRegion != null)
        {
            selectedRegion.ResetMarkerColor();  // Добавете този метод в RegionData
            selectedRegion = null;              // Нулира избрания регион
        }
        SetTurnState(TurnState.BotSelection);
    }
    #endregion

    #region Логика на бота
    void BotSelectRegion()
    {
        RegionData[] availableRegions = GetAvailableRegions();
        if (availableRegions.Length == 0)
        {
            Debug.Log("[RegionInteraction] No available regions for bot! Game over.");
            gameManager.GameOver();
            return;
        }

        selectedRegion = availableRegions[Random.Range(0, availableRegions.Length)];
        selectedRegions[selectedRegion.regionName] = true;
        Debug.Log($"[RegionInteraction] Bot selected region: {selectedRegion.regionName}");
        MarkRegionAsSelected(selectedRegion, botColor);

        SetTurnState(TurnState.BotQuestion);
    }

    RegionData[] GetAvailableRegions()
    {
        List<RegionData> available = new List<RegionData>();
        foreach (RegionData region in FindObjectsOfType<RegionData>())
        {
            if (!selectedRegions.ContainsKey(region.regionName))
            {
                available.Add(region);
            }
        }
        Debug.Log($"[RegionInteraction] Found {available.Count} available regions for bot");
        return available.ToArray();
    }

    IEnumerator BotTurnDelay()
    {
        Debug.Log("[RegionInteraction] Starting bot turn with delay");
        yield return new WaitForSeconds(1f);
        isProcessingAction = false; // Сега ботът може да избере регион
        BotSelectRegion();
    }

    IEnumerator SimulateBotAction()
    {
        Debug.Log("[RegionInteraction] Simulating bot action");

        // Добавете проверка за null
        if (selectedRegion == null)
        {
            Debug.LogError("Cannot perform bot action - no region selected!");
            SetTurnState(TurnState.PlayerSelection);
            yield break;
        }

    
        yield return new WaitForSeconds(2f);

        if (selectedRegion != null)
        {
            float influence = Random.Range(5f, 15f);
            selectedRegion.UpdateBotInfluence(influence);
            Debug.Log($"[RegionInteraction] Bot applied {influence} influence to {selectedRegion.regionName}");
        }

        
        selectedRegion = null;

       
        isProcessingAction = false;

        Debug.Log("[RegionInteraction] Bot action completed, passing turn to player");
        SetTurnState(TurnState.PlayerSelection);
    }
    /*
        IEnumerator SimulateBotAction()
        {
            Debug.Log("[RegionInteraction] Simulating bot action");
            yield return new WaitForSeconds(2f);
            if (selectedRegion != null)
            {
                float influence = Random.Range(5f, 15f);
                selectedRegion.UpdateBotInfluence(influence);
                Debug.Log($"[RegionInteraction] Bot applied {influence} influence to {selectedRegion.regionName}");
            }

            // Важно: След завършване на действието на бота, предаваме хода на играча
            Debug.Log("[RegionInteraction] Bot action completed, passing turn to player");
            SetTurnState(TurnState.PlayerSelection);
        }
 */
    #endregion

    #region Управление на състоянията
    void SetTurnState(TurnState newState)
    {
        Debug.Log($"[RegionInteraction] SetTurnState: {currentState} -> {newState}. isProcessingAction: {isProcessingAction}");
        currentState = newState;

        // Скриваме всички панели при смяна на състоянието
        HideAllPanels();

        // В повечето случаи, когато сменяме състоянието, ще искаме да блокираме обработката
        isProcessingAction = true;

        switch (newState)
        {
            /*    case TurnState.PlayerSelection:
                    // Това е началото на нов ход, тук е подходящо да нулираме избрания регион
                    Debug.Log("[RegionInteraction] Player's turn to select a region");
                    selectedRegion = null;
                    EnableRegionColliders(true);
                    // Разрешаваме обработката на действия
                    isProcessingAction = false;
                    break;
            */
            case TurnState.PlayerSelection:
                Debug.Log("[RegionInteraction] Player's turn to select a region");
                selectedRegion = null; // Явно нулираме избрания регион
                EnableRegionColliders(true);
                isProcessingAction = false;

                //ТУК
                actionPanel.HidePanel();
                break;
            case TurnState.PlayerQuestion:
                if (playerQuestionPanel == null)
                {
                    Debug.LogWarning("[RegionInteraction] Player Question Panel is null! Skipping question.");
                    SetTurnState(TurnState.PlayerAction);
                    return;
                }

                Debug.Log($"[RegionInteraction] Setting up question for player with region: {(selectedRegion != null ? selectedRegion.regionName : "null")}");
                EnableRegionColliders(false);
                // Показваме панела с въпрос със закъснение
                StartCoroutine(ShowQuestionPanelDelayed());
                break;

            case TurnState.PlayerAction:
                if (actionPanel == null)
                {
                    Debug.LogError("[RegionInteraction] Action Panel is null! Cannot show panel.");
                    SetTurnState(TurnState.BotSelection);
                    return;
                }

                Debug.Log($"[RegionInteraction] Showing action panel for region: {(selectedRegion != null ? selectedRegion.regionName : "null")}");
                actionPanel.ShowForRegion(selectedRegion);
                break;

            case TurnState.BotSelection:
                Debug.Log("[RegionInteraction] Bot's turn to select a region");
                // НЕ нулираме selectedRegion тук, ще го направим в PlayerSelection

                //тук
                actionPanel.HidePanel();
                StartCoroutine(BotTurnDelay());
                break;

            case TurnState.BotQuestion:
                if (botQuestionPanel == null)
                {
                    Debug.LogError("[RegionInteraction] Bot Question Panel is null! Cannot show panel.");
                    SetTurnState(TurnState.BotAction); // Ако няма панел, премини директно към действие
                    return;
                }

                Debug.Log($"[RegionInteraction] Setting up question for bot with region: {(selectedRegion != null ? selectedRegion.regionName : "null")}");
                botQuestionPanel.SetupQuestion(selectedRegion);
                botQuestionPanel.ShowPanel();
                StartCoroutine(botQuestionPanel.SimulateBotAnswer());
                break;

            case TurnState.BotAction:
                Debug.Log("[RegionInteraction] Bot is taking action");

                // Уверете се, че регионът все още е избран
                if (selectedRegion != null)
                {
                    StartCoroutine(SimulateBotAction());
                }
                else
                {
                    Debug.LogError("No region selected for bot action!");
                    SetTurnState(TurnState.PlayerSelection);
                }
                break;
        }
    }
    #endregion

    private IEnumerator ShowQuestionPanelDelayed()
    {
        Debug.Log("[RegionInteraction] Waiting to show question panel");
        yield return new WaitForEndOfFrame();

        Debug.Log("[RegionInteraction] Now showing player question panel");
        playerQuestionPanel.SetupQuestion(selectedRegion);
        playerQuestionPanel.ShowPanel();
    }

    public void OnBotQuestionAnsweredIncorrectly()
    {
        Debug.Log("[RegionInteraction] Bot answered incorrectly, moving to player's turn");
        SetTurnState(TurnState.PlayerSelection);
        // Нулиране на selectedRegion ще се направи в PlayerSelection
    }

    public void OnBotQuestionAnsweredCorrectly()
    {
        Debug.Log("[RegionInteraction] Bot answered correctly, moving to bot action");

        // Уверете се, че selectedRegion не е null
        if (selectedRegion == null)
        {
            Debug.LogError("Selected region is null when bot answered correctly!");
            return;
        }

        SetTurnState(TurnState.BotAction);
    }

    #region Помощни функции
    void MarkRegionAsSelected(RegionData region, Color color)
    {
        if (region == null)
        {
            Debug.LogError("[RegionInteraction] Attempted to mark null region as selected!");
            return;
        }

        Debug.Log($"[RegionInteraction] Marking region {region.regionName} as selected with color");
        region.SetMarkerColor(color);
    }

    void EnableRegionColliders(bool enable)
    {
        Debug.Log($"[RegionInteraction] {(enable ? "Enabling" : "Disabling")} all region colliders - current state: {currentState}");
        foreach (RegionData region in FindObjectsOfType<RegionData>())
        {
            Collider2D col = region.GetComponent<Collider2D>();
            if (col != null)
            {
                col.enabled = enable;
                Debug.Log($"[RegionInteraction] Region {region.regionName} collider enabled: {col.enabled}");
            }
        }
    }

    bool IsPointerOverUI()
    {
        PointerEventData eventData = new PointerEventData(UnityEngine.EventSystems.EventSystem.current);
        eventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        UnityEngine.EventSystems.EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
    #endregion

    public void OnActionsComplete()
    {
        Debug.Log("[RegionInteraction] Actions completed, moving to bot's turn");
        // Действието на играча е завършило, предаваме хода на бота
        SetTurnState(TurnState.BotSelection);
    }
}