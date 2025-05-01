using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public int currentRound = 1;
    public int totalRounds = 10;
    public Player player;
    public Bot bot;
   
    public TextMeshProUGUI roundCounterText; //Рундове - текст
    public GameObject gameOverPanel; // Край на играта - панел
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI playerMandatesText;
    public TextMeshProUGUI botMandatesText;

    public Button nextRoundButton; // бутон за следващ рунд
    public Button restartButton; 

    private int playerTotalMandates = 0;
    private int botTotalMandates = 0;
    private int playerActionsTaken = 0;
    private int botActionsTaken = 0;

    private bool isGameOver = false;

    public GameObject[] regions; // Референции към GameObjects на областите (дърпай ги в Inspector)
    public bool isPlayerTurn = true;

    private GameObject selectedRegion; // Add this at the top of the class

    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Опционално, ако искаш да запазиш GameManager между сцени
        }
        else
        {
            Destroy(gameObject); // Предотвратява дублиране
        }
    }
    public void StartPlayerTurn()
    {
        isPlayerTurn = true;
        Debug.Log("Ред на играча! Избери област.");
    }

   public void StartBotTurn()
    {
        isPlayerTurn = false;
        StartCoroutine(BotTakesTurnAfterDelay());
      //  StartCoroutine(BotSelectsRegion());
    }
    

    private IEnumerator BotSelectsRegion()
    {
        yield return new WaitForSeconds(1.5f); // Забавяне за по-естествено поведение

        // Филтрирай само неутрални области (без цвят или с default цвят)
        List<GameObject> availableRegions = new List<GameObject>();
        foreach (var region in regions)
        {
            SpriteRenderer renderer = region.GetComponent<SpriteRenderer>();
            if (renderer.color == Color.white)
            { // Ако все още е неутрална
                availableRegions.Add(region);
            }
        }

        if (availableRegions.Count > 0 && bot.SpendMoney(100f))
        { // Примерна цена
            int randomIndex = Random.Range(0, availableRegions.Count);
            SpriteRenderer botRenderer = availableRegions[randomIndex].GetComponent<SpriteRenderer>();
            botRenderer.color = PlayerDataManager.Instance.botColor; // Оцветява в цвета на бота
            botActionsTaken++;
            bot.UpdateBotBudgetDisplay();
        }

        yield return new WaitForSeconds(0.5f);
        StartPlayerTurn(); // Обратно към играча
    }

    public void OnRegionClicked(GameObject region)
    {
        if (!isPlayerTurn || isGameOver) return;

        SpriteRenderer renderer = region.GetComponent<SpriteRenderer>();
        if (renderer == null)
        {
            Debug.LogError("Няма SpriteRenderer на региона!");
            return;
        }

        selectedRegion = region; // Save the selected region for use in actions
        Debug.Log("Избрана област: " + region.name);
    }

    public void PerformPlayerAction(float cost)
    {
        if (!isPlayerTurn || !selectedRegion || isGameOver) return;

        if (player.SpendMoney(cost))
        {
            // Color change happens here
            SpriteRenderer renderer = selectedRegion.GetComponent<SpriteRenderer>();
            if (renderer != null)
            {
                renderer.color = PlayerDataManager.Instance.playerColor;
            }

            // Influence update, budget display, etc.
            RegionData regionData = selectedRegion.GetComponent<RegionData>();
            if (regionData != null)
            {
                float influence = 10f; // Example influence value
                regionData.playerInfluence += influence;
                regionData.UpdateInfluenceDisplay();
            }

            player.UpdatePlayerBudgetDisplay();
            playerActionsTaken++;
            EndPlayerTurn();
        }
    }

    void Update()
    {
        //Debug.Log($"Състояние: {(isPlayerTurn ? "Ред на играча" : "Ред на бота")}");
    }
    void Start()
    {
     /*   playerTotalMandates = 0;
        botTotalMandates = 0;

        if (player == null)
        {
            player = FindObjectOfType<Player>();
        }
        if (bot == null)
        {
            bot = FindObjectOfType<Bot>();
            if (bot == null) Debug.LogError("Няма Bot обект в сцената!");
        }
     */
    /*    ActionPanel actionPanel = FindObjectOfType<ActionPanel>();
        if (actionPanel != null)
        {
            actionPanel.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError("ActionPanel не е намерен!");
        }
    */
    /*    UpdateRoundCounter();
        player.pdatePlayerBudgetDisplay();
        bot.UpdateBotBudgetDisplay();

        if (nextRoundButton != null)
        {
            nextRoundButton.onClick.AddListener(EndPlayerTurn);
        }
        else
        {
            Debug.LogError("Next Round button is not assigned.");
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogError("Restart button is not assigned.");
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    */
        StartPlayerTurn();
    }

    // Край на хода на играча
    public void EndPlayerTurn()
    {
        /*    if (isGameOver) return;

            Debug.Log("Player has ended their turn.");
            StartCoroutine(BotTakesTurnAfterDelay());
        */
        if (isGameOver) return;

        isPlayerTurn = false; // Критично добавяне!
        Debug.Log("Край на хода на играча. Ботът започва...");
        StartCoroutine(BotTakesTurnAfterDelay());
    }

    // Забавяне на хода на бота
    private IEnumerator BotTakesTurnAfterDelay()
    {
        yield return new WaitForSeconds(1.5f); // Забавяне за UX
        if (!isPlayerTurn)
        {
            // Избери случайна неутрална област
            GameObject[] allRegions = GameObject.FindGameObjectsWithTag("Region"); // Добави tag "Region" на регионите!
            List<GameObject> neutralRegions = new List<GameObject>();

            foreach (var region in allRegions)
            {
                SpriteRenderer renderer = region.GetComponent<SpriteRenderer>();
                if (renderer.color == Color.white)
                {
                    neutralRegions.Add(region);
                }
            }

            if (neutralRegions.Count > 0 && bot.SpendMoney(100f))
            {
                int randomIndex = Random.Range(0, neutralRegions.Count);
                SpriteRenderer botRenderer = neutralRegions[randomIndex].GetComponent<SpriteRenderer>();
                botRenderer.color = PlayerDataManager.Instance.botColor;
                botActionsTaken++;
                bot.UpdateBotBudgetDisplay();
            }

            yield return new WaitForSeconds(0.5f);
            StartPlayerTurn(); // Обратно към играча
        }
    }


    // Ход на бота
    private void BotTakesTurn()
    {
        if (isGameOver) return;

        Debug.Log("Bot is taking its turn...");

        // Брой на действията, които ботът трябва да извърши
        int botActions = playerActionsTaken + Random.Range(-3, 4);
        botActions = Mathf.Max(1, botActions);

        // Рандом ходовете на бота
        for (int i = 0; i < botActions; i++)
        {
            RegionData[] regions = FindObjectsOfType<RegionData>();
            if (regions.Length > 0)
            {
                RegionData chosenRegion = regions[Random.Range(0, regions.Length)];
                int categoryIndex = Random.Range(0, 3);
                RegionData.ActionData[] actions = GetActionsForCategory(chosenRegion, categoryIndex);

                if (actions.Length > 0)
                {
                    RegionData.ActionData chosenAction = actions[Random.Range(0, actions.Length)];
                    float cost = chosenRegion.GetActionCost(chosenAction);
                    float influence = chosenRegion.GetActionInfluence(chosenAction);

                    if (bot.SpendMoney(cost))
                    {
                        chosenRegion.UpdateBotInfluence(influence); // Влияние на бота на регион
                        Debug.Log($"Bot performed {chosenAction.name} in {chosenRegion.regionName} (Cost: {cost:F1} лв., Influence: {influence:F1}%)");
                        botActionsTaken++;
                    }
                }
            }
            else
            {
                Debug.LogWarning("No regions found for the bot to act on.");
                break;
            }
        }

        bot.CalculateOverallInfluence(); // Влияние на бота в страната 
        CheckRoundEnd();
    }

    // Действия по категории
    private RegionData.ActionData[] GetActionsForCategory(RegionData region, int categoryIndex)
    {
        switch (categoryIndex)
        {
            case 0: return region.politicalCampaigns;
            case 1: return region.socialInitiatives;
            case 2: return region.economicMeasures;
            default: return new RegionData.ActionData[0];
        }
    }

    // Край на рунда
    private void CheckRoundEnd()
    {
        Debug.Log("Both player and bot have completed their actions. Advancing to the next round.");
        playerActionsTaken = 0;
        botActionsTaken = 0;
        NextRound();
    }

    // Следващ рунд, ако има такъв
    public void NextRound()
    {
        if (isGameOver) return;

        if (currentRound < totalRounds)
        {
            currentRound++;
            UpdateRoundCounter();
        }
        else
        {
            EndGame();
        }
    }

    // Ъпдейт на текста на рундовете
    private void UpdateRoundCounter()
    {
        if (roundCounterText != null)
        {
            roundCounterText.text = $"Кръг: {currentRound}/{totalRounds}";
        }
    }

    // Край на играта
    public void EndGame()
    {
        isGameOver = true;

        playerTotalMandates = 0;
        botTotalMandates = 0;

        // Изчисление на мандатите на всички региони
        foreach (var region in FindObjectsOfType<RegionData>())
        {
            CalculateMandates(region);
        }

        // Определяне на победителя
        if (playerTotalMandates > botTotalMandates)
        {
            gameOverText.text = "Вие спечелихте!";
        }
        else if (botTotalMandates > playerTotalMandates)
        {
            gameOverText.text = "Ботът спечели!";
        }
        else
        {
            gameOverText.text = "Равенство!";
        }

        // Ъпдейт на текста за мандатите
        if (playerMandatesText != null)
        {
            playerMandatesText.text = $"{PlayerDataManager.Instance.playerName} мандати: {playerTotalMandates}";
        }
        if (botMandatesText != null)
        {
            botMandatesText.text = $"Мандати на бота: {botTotalMandates}";
        }

        // Показване на панел - Край на играта
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Спиране на бутоните по сцената
        DisableInteractions();
    }

    // Изчисление на мандатите
    public void CalculateMandates(RegionData region)
    {
        if (region == null)
        {
            Debug.LogError("RegionData is null.");
            return;
        }

        if (region.playerInfluence < 0 || region.botInfluence < 0)
        {
            Debug.LogError($"Invalid influence values in region {region.regionName}: Player = {region.playerInfluence}, Bot = {region.botInfluence}");
            return;
        }

        float totalInfluence = region.playerInfluence + region.botInfluence; // Общо влияние
        if (totalInfluence > 0)
        {
            int playerMandates = Mathf.RoundToInt(region.mandates * (region.playerInfluence / totalInfluence));
            int botMandates = Mathf.RoundToInt(region.mandates * (region.botInfluence / totalInfluence));

            playerTotalMandates += playerMandates;
            botTotalMandates += botMandates;

            Debug.Log($"Region {region.regionName}: Player Mandates = {playerMandates}, Bot Mandates = {botMandates}");
        }
        else
        {
            Debug.LogWarning($"No influence in region {region.regionName}. Mandates not allocated.");
        }
    }


    // Брояч на действията на играча
    public void PlayerActionTaken()
    {
        if (isGameOver) return;

        playerActionsTaken++; // Increment the player's actions
    }

    // Спиране на бутоните в сцената
    private void DisableInteractions()
    {
        // Спиране на бутон - Следващ рунд
        if (nextRoundButton != null)
        {
            nextRoundButton.interactable = false;
        }

        // Спиране на бутоните на регионите
        RegionInteraction[] regionInteractions = FindObjectsOfType<RegionInteraction>();
        foreach (var regionInteraction in regionInteractions)
        {
            regionInteraction.enabled = false;
        }

        // Спиране на панела за действия
        ActionPanel actionPanel = FindObjectOfType<ActionPanel>();
        if (actionPanel != null)
        {
            actionPanel.gameObject.SetActive(false);
        }
    }

    // Нова игра
    public void RestartGame()
    {
        isGameOver = false;
        currentRound = 1;
        playerTotalMandates = 0;
        botTotalMandates = 0;
        playerActionsTaken = 0;
        botActionsTaken = 0;

        if (player != null)
        {
            player.budget = 1000f;
            player.overallInfluence = 0f;
            player.UpdateBudgetDisplay();
            player.UpdateOverallInfluenceDisplay();
        }
        if (bot != null)
        {
            bot.budget = 1000f;
            bot.overallInfluence = 0f;
            bot.UpdateBudgetDisplay();
            bot.UpdateOverallInfluenceDisplay();
        }

        foreach (var region in FindObjectsOfType<RegionData>())
        {
            region.playerInfluence = 0f;
            region.botInfluence = 0f;
            region.UpdateInfluenceDisplay();
        }

        if (nextRoundButton != null)
        {
            nextRoundButton.interactable = true;
        }
        RegionInteraction[] regionInteractions = FindObjectsOfType<RegionInteraction>();
        foreach (var regionInteraction in regionInteractions)
        {
            regionInteraction.enabled = true;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateRoundCounter();
        player.UpdatePlayerBudgetDisplay();
        bot.UpdateBotBudgetDisplay();
    }

    public bool IsGameOver()
    {
        return isGameOver;
    }
    public void GameOver()
    {
        // Примерна имплементация:
        Debug.Log("Game Over!");
        // Тук можете да добавите логика за край на играта
    }
}