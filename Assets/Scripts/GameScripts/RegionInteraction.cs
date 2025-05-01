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
        if (playerQuestionPanel != null) playerQuestionPanel.HidePanel();
        if (botQuestionPanel != null) botQuestionPanel.HidePanel();
        if (actionPanel != null) actionPanel.HidePanel();
    }
    #endregion

    #region Логика на играча
    void Update()
    {
        if (currentState == TurnState.PlayerSelection && Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            HandlePlayerSelection();
        }
    }

    public void ResetAllPanels()
    {
        if (playerQuestionPanel != null) playerQuestionPanel.HidePanel();
        if (botQuestionPanel != null) botQuestionPanel.HidePanel();
        if (actionPanel != null) actionPanel.gameObject.SetActive(false);
    }

    public RegionData GetSelectedRegion() => selectedRegion;

    void HandlePlayerSelection()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null) return;

        RegionData region = hit.collider.GetComponent<RegionData>();
        if (region == null || selectedRegions.ContainsKey(region.regionName)) return;

        Debug.Log($"Selected region: {region.regionName}");
        selectedRegion = region;
        selectedRegions[region.regionName] = true;
        MarkRegionAsSelected(region, PlayerDataManager.Instance.playerColor);

        SetTurnState(TurnState.PlayerQuestion);
      
        //тук мое
      //  selectedRegion = null;

    }

    // Извиква се при верен отговор
    public void OnQuestionAnsweredCorrectly()
    {
        Debug.Log("Question answered correctly! Moving to action panel.");
        SetTurnState(TurnState.PlayerAction);
     
            //
      //selectedRegion = null;


    }

    // Извиква се при грешен отговор
    public void OnQuestionAnsweredIncorrectly()
    {
        Debug.Log("Question answered incorrectly! Moving to bot selection.");
        selectedRegion = null;
        SetTurnState(TurnState.BotSelection);

        //тук
        selectedRegion = null;

    }
    #endregion

    #region Логика на бота
    void BotSelectRegion()
    {
        RegionData[] availableRegions = GetAvailableRegions();
        if (availableRegions.Length == 0)
        {
            gameManager.GameOver();
            return;
        }

        selectedRegion = availableRegions[Random.Range(0, availableRegions.Length)];
        selectedRegions[selectedRegion.regionName] = true;
        MarkRegionAsSelected(selectedRegion, botColor);

        SetTurnState(TurnState.BotQuestion);

//тук
      //  selectedRegion = null;

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
        return available.ToArray();
    }

    IEnumerator BotTurnDelay()
    {
        yield return new WaitForSeconds(1f);
        BotSelectRegion();
    }

    IEnumerator SimulateBotAction()
    {
        yield return new WaitForSeconds(2f);
        if (selectedRegion != null)
        {
            float influence = Random.Range(5f, 15f);
            selectedRegion.UpdateBotInfluence(influence);
        }
        SetTurnState(TurnState.PlayerSelection);

        //тук
       // selectedRegion = null;

    }
    #endregion

    #region Управление на състоянията
    void SetTurnState(TurnState newState)
    {
        Debug.Log("[SetTurnState] -> " + newState);
        currentState = newState;
        if (debugRegions) Debug.Log($"Ново състояние: {newState}");

        HideAllPanels();

        switch (newState)
        {
            case TurnState.PlayerSelection:
                EnableRegionColliders(true);
                break;

            case TurnState.PlayerQuestion:
                if (playerQuestionPanel == null)
                {
                    Debug.LogWarning("Player Question Panel is null! Прескачаме въпроса.");

                    SetTurnState(TurnState.PlayerAction);
                 

                    return;
                }

                Debug.Log("Setting up question panel with region: " +
                          (selectedRegion != null ? selectedRegion.regionName : "null"));

                // ВАЖНО: вместо директно да показваме панела, чакаме кадър
                StartCoroutine(ShowQuestionPanelDelayed());
                EnableRegionColliders(false);
                break;

            case TurnState.PlayerAction:
                /*  
                   if (actionPanel == null)
                   {
                       Debug.LogError("Action Panel is null! Cannot show panel.");
                       SetTurnState(TurnState.BotSelection);

                       //тук
                          selectedRegion = null;

                       return;
                   }

                   Debug.Log("Showing action panel for region: " +
                             (selectedRegion != null ? selectedRegion.regionName : "null"));
                */
                //actionPanel.gameObject.SetActive(true);

                Debug.Log("ТУК");

             actionPanel.ShowForRegion(selectedRegion);
                break;

            case TurnState.BotSelection:
                StartCoroutine(BotTurnDelay());
                break;

            case TurnState.BotQuestion:
                if (botQuestionPanel == null)
                {
                    Debug.LogError("Bot Question Panel is null! Cannot show panel.");
                    SetTurnState(TurnState.BotAction);

                    //тук
                    selectedRegion = null;

                    return;
                }

                botQuestionPanel.SetupQuestion(selectedRegion);
                botQuestionPanel.ShowPanel();
                StartCoroutine(botQuestionPanel.SimulateBotAnswer());
                break;

            case TurnState.BotAction:
                StartCoroutine(SimulateBotAction());
                break;
        }
    }
    #endregion
    private IEnumerator ShowQuestionPanelDelayed()
    {
        yield return new WaitForEndOfFrame();

        Debug.Log("Now calling ShowPanel()");
        playerQuestionPanel.SetupQuestion(selectedRegion);
        playerQuestionPanel.ShowPanel();
    }

    public void OnBotQuestionAnsweredIncorrectly()
    {
       // selectedRegion = null;
        SetTurnState(TurnState.PlayerSelection);

        //тук
        selectedRegion = null;

    }
    public void OnBotQuestionAnsweredCorrectly()
    {
        SetTurnState(TurnState.BotAction);
    }

    #region Помощни функции
    void MarkRegionAsSelected(RegionData region, Color color)
    {
        region.SetMarkerColor(color);

    }

    void EnableRegionColliders(bool enable)
    {
        foreach (RegionData region in FindObjectsOfType<RegionData>())
        {
            Collider2D col = region.GetComponent<Collider2D>();
            if (col != null) col.enabled = enable;
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
        // Можете да добавите допълнителна логика тук ако е необходимо
        SetTurnState(TurnState.BotSelection);

        //тук
        selectedRegion = null;

    }
}
/*
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

    #region Инициализация
    public static RegionInteraction Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Start()
    {
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
        playerQuestionPanel?.HidePanel();
        botQuestionPanel?.HidePanel();
        actionPanel?.HidePanel();
    }
    #endregion

    #region Логика на играча
    void Update()
    {
        if (currentState == TurnState.PlayerSelection && Input.GetMouseButtonDown(0) && !IsPointerOverUI())
        {
            HandlePlayerSelection();
        }
    }

    public void ResetAllPanels()
    {
        if (playerQuestionPanel != null) playerQuestionPanel?.HidePanel();
        if (botQuestionPanel != null) botQuestionPanel?.HidePanel();
        if (actionPanel != null) actionPanel.gameObject.SetActive(false);
    }

    public RegionData GetSelectedRegion() => selectedRegion;

    void HandlePlayerSelection()
    {
        RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (hit.collider == null) return;

        RegionData region = hit.collider.GetComponent<RegionData>();
        if (region == null || selectedRegions.ContainsKey(region.regionName)) return;

        selectedRegion = region;
        selectedRegions[region.regionName] = true;
        MarkRegionAsSelected(region, PlayerDataManager.Instance.playerColor);

        SetTurnState(TurnState.PlayerQuestion);
    }

    // Извиква се при верен отговор
    public void OnQuestionAnsweredCorrectly()
    {
        SetTurnState(TurnState.PlayerAction);
    }

    // Извиква се при грешен отговор
    public void OnQuestionAnsweredIncorrectly()
    {
        SetTurnState(TurnState.BotSelection);
    }
    #endregion

    #region Логика на бота
    void BotSelectRegion()
    {
        RegionData[] availableRegions = GetAvailableRegions();
        if (availableRegions.Length == 0)
        {
            gameManager.GameOver();
            return;
        }

        selectedRegion = availableRegions[Random.Range(0, availableRegions.Length)];
        selectedRegions[selectedRegion.regionName] = true;
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
        return available.ToArray();
    }

    IEnumerator BotTurnDelay()
    {
        yield return new WaitForSeconds(1f);
        BotSelectRegion();
    }

    IEnumerator SimulateBotAction()
    {
        yield return new WaitForSeconds(2f);
        if (selectedRegion != null)
        {
            float influence = Random.Range(5f, 15f);
            selectedRegion.UpdateBotInfluence(influence);
        }
        SetTurnState(TurnState.PlayerSelection);
    }
    #endregion

    #region Управление на състоянията
    void SetTurnState(TurnState newState)
    {
        currentState = newState;
        if (debugRegions) Debug.Log($"Ново състояние: {newState}");

        HideAllPanels();

        switch (newState)
        {
            case TurnState.PlayerSelection:
                EnableRegionColliders(true);
                break;

            case TurnState.PlayerQuestion:
                playerQuestionPanel.SetupQuestion(selectedRegion);
                playerQuestionPanel.ShowPanel();
                EnableRegionColliders(false);
                break;

            case TurnState.PlayerAction:
                actionPanel.ShowForRegion(selectedRegion);
                break;

            case TurnState.BotSelection:
                StartCoroutine(BotTurnDelay());
                break;

            case TurnState.BotQuestion:
                botQuestionPanel.SetupQuestion(selectedRegion);
                botQuestionPanel.ShowPanel();
                StartCoroutine(botQuestionPanel.SimulateBotAnswer());
                break;

            case TurnState.BotAction:
                StartCoroutine(SimulateBotAction());
                break;
        }
    }
    #endregion

    #region Помощни функции
    void MarkRegionAsSelected(RegionData region, Color color)
    {
        region.SetMarkerColor(color);
    }

    void EnableRegionColliders(bool enable)
    {
        foreach (RegionData region in FindObjectsOfType<RegionData>())
        {
            Collider2D col = region.GetComponent<Collider2D>();
            if (col != null) col.enabled = enable;
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
        // Можете да добавите допълнителна логика тук ако е необходимо
        SetTurnState(TurnState.BotSelection);
    }
}
*/