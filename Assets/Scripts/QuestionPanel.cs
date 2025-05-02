using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionPanel : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;
        public string[] specificRegions;

        public bool IsValidForRegion(string regionName)
        {
            if (specificRegions == null || specificRegions.Length == 0)
                return true;

            foreach (string region in specificRegions)
            {
                if (region == regionName) return true;
            }
            return false;
        }
    }

    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI[] answerTexts;
    public TextMeshProUGUI feedbackText;
    public Image feedbackPanel;

    [SerializeField] private List<Question> questions = new List<Question>();
    private Question currentQuestion;
    private RegionData currentRegion;
    private RegionInteraction regionInteraction;

    [SerializeField] private bool isBotPanel = false;

    private Color correctColor = new Color(0.2f, 0.8f, 0.2f, 0.8f); // Зелен
    private Color incorrectColor = new Color(0.8f, 0.2f, 0.2f, 0.8f); // Червен

    public QuestionManager externalQuestionManager;

    void Awake()
    {
        // При Awake трябва да се убедим, че панелът е неактивен
        HidePanel();
    }

    private void ImportQuestionsFromManager()
    {
        QuestionManager qm = FindObjectOfType<QuestionManager>();
        if (qm == null)
        {
            Debug.LogError("QuestionManager не е намерен. Няма откъде да заредим въпроси.");
            return;
        }

        var externalQuestions = qm.GetAllQuestions();

        foreach (var extQ in externalQuestions)
        {
            if (extQ.answers == null || extQ.answers.Count == 0)
            {
                Debug.LogWarning($"Въпросът \"{extQ.questionText}\" няма отговори. Пропуска се.");
                continue;
            }

            Question q = new Question();
            q.questionText = extQ.questionText;
            q.answers = new string[extQ.answers.Count];
            q.correctAnswerIndex = -1;

            for (int i = 0; i < extQ.answers.Count; i++)
            {
                q.answers[i] = extQ.answers[i].answerText;
                if (extQ.answers[i].isCorrect)
                    q.correctAnswerIndex = i;
            }

            if (q.correctAnswerIndex == -1)
            {
                Debug.LogWarning($"Пропускаме въпрос без правилен отговор: \"{q.questionText}\"");
                continue;
            }

            q.specificRegions = new string[0]; // Може да добавиш логика по категории ако искаш
            questions.Add(q);
        }

        Debug.Log($"✅ Заредени {questions.Count} въпроса от QuestionManager в QuestionPanel.");
    }

    void Start()
    {
        regionInteraction = FindObjectOfType<RegionInteraction>();
        if (regionInteraction == null)
        {
            Debug.LogError("❌ RegionInteraction not found! QuestionPanel won't function properly.");
        }

        externalQuestionManager = FindObjectOfType<QuestionManager>();
        if (externalQuestionManager != null)
        {
            Debug.Log("📥 Импортираме въпроси от QuestionManager в QuestionPanel");
            ImportQuestionsFromManager();
        }
        else
        {
            Debug.LogWarning("⚠️ QuestionManager не е намерен – няма въпроси!");
        }

        if (questions.Count == 0)
        {
            Debug.LogWarning("❌ QuestionPanel не получи въпроси! Зареждаме примерни.");
            AddExampleQuestions();
        }

        // Проверка за UI елементи
        if (questionText == null) Debug.LogError("❌ Question Text is not assigned in QuestionPanel!");
        if (answerButtons == null || answerButtons.Length == 0) Debug.LogError("❌ Answer Buttons are not assigned in QuestionPanel!");
        if (answerTexts == null || answerTexts.Length == 0) Debug.LogError("❌ Answer Texts are not assigned in QuestionPanel!");

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null)
            {
                Debug.LogError($"❌ Answer Button {i} is not assigned in QuestionPanel!");
                continue;
            }

            int buttonIndex = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(buttonIndex));
        }

        if (feedbackPanel != null) feedbackPanel.gameObject.SetActive(false);
    }

    private bool isActive = false;
    public void ShowPanel()
    {
        Debug.Log("Showing question panel");
        isActive = true;
        transform.SetAsLastSibling();
        gameObject.SetActive(true);
    }

    public void HidePanel()
    {
        Debug.Log("Hiding question panel");
        isActive = false;
        gameObject.SetActive(false);
    }

    public void SetupQuestion(RegionData region)
    {
        Debug.Log($"Setting up question for region: {(region != null ? region.regionName : "null")}");
        currentRegion = region;
        ShowQuestionForRegion(region);
    }

    void ShowQuestionForRegion(RegionData region)
    {
        if (region == null)
        {
            Debug.LogWarning("Region is null! Showing random question.");
            ShowRandomQuestion();
            return;
        }

        List<Question> validQuestions = questions.FindAll(q => q.IsValidForRegion(region.regionName));

        if (validQuestions.Count > 0)
        {
            currentQuestion = validQuestions[Random.Range(0, validQuestions.Count)];
            Debug.Log($"Found {validQuestions.Count} region-specific questions. Selected: {currentQuestion.questionText}");
        }
        else
        {
            Debug.Log("No region-specific questions found, selecting random question.");
            currentQuestion = questions[Random.Range(0, questions.Count)];
        }

        DisplayQuestion();
    }

    void ShowRandomQuestion()
    {
        if (questions.Count == 0)
        {
            Debug.LogError("No questions available!");
            return;
        }

        currentQuestion = questions[Random.Range(0, questions.Count)];
        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        if (questionText == null)
        {
            Debug.LogError("Question Text component is missing!");
            return;
        }

        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null) continue;

            bool shouldShow = i < currentQuestion.answers.Length;
            answerButtons[i].gameObject.SetActive(shouldShow);
            answerButtons[i].interactable = true;

            if (shouldShow && i < answerTexts.Length && answerTexts[i] != null)
            {
                answerTexts[i].text = currentQuestion.answers[i];
            }
        }

        if (feedbackPanel != null) feedbackPanel.gameObject.SetActive(false);
    }

    public void OnAnswerSelected(int answerIndex)
    {
        if (!isActive)
        {
            Debug.LogWarning("Ignored answer selection: panel not active.");
            return;
        }
        Debug.Log($"Answer selected: {answerIndex} (manual click?)");
        bool isCorrect = (answerIndex == currentQuestion.correctAnswerIndex);
        ShowFeedback(isCorrect);

        foreach (Button button in answerButtons)
        {
            if (button != null) button.interactable = false;
        }

        StartCoroutine(ProcessAnswerAfterDelay(isCorrect));
    }

    IEnumerator ProcessAnswerAfterDelay(bool isCorrect)
    {
        yield return new WaitForSeconds(1.5f);

        if (isCorrect)
        {
            if (isBotPanel)
                regionInteraction.OnBotQuestionAnsweredCorrectly();
            else
            {
                Debug.Log("Answer was correct!");
                if (currentRegion != null) currentRegion.UpdatePlayerInfluence(5f);
                if (regionInteraction != null) regionInteraction.OnQuestionAnsweredCorrectly();
            }
        }
        else
        {
            if (isBotPanel)
                regionInteraction.OnBotQuestionAnsweredIncorrectly();
            else
            {
                Debug.Log("Answer was incorrect!");
                if (regionInteraction != null) regionInteraction.OnQuestionAnsweredIncorrectly();
            }
        }

        HidePanel();
    }
    void ShowFeedback(bool isCorrect)
    {
        if (feedbackPanel == null || feedbackText == null) return;

        feedbackPanel.gameObject.SetActive(true);
        feedbackPanel.color = isCorrect ? correctColor : incorrectColor;
        feedbackText.text = isCorrect ? "Правилен отговор!" : "Грешен отговор!";
    }

    // Симулира отговор на бота
    public IEnumerator SimulateBotAnswer()
    {
        yield return new WaitForSeconds(1.5f);
        bool correct = Random.value < 0.7f;
        OnAnswerSelected(correct ? currentQuestion.correctAnswerIndex :
            Random.Range(0, currentQuestion.answers.Length));
    }

    void AddExampleQuestions()
    {
        Debug.Log("Adding example questions since none were configured");

        questions.Add(new Question
        {
            questionText = "Коя е най-голямата политическа партия в България?",
            answers = new string[] { "ГЕРБ", "БСП", "ИТН", "ДПС" },
            correctAnswerIndex = 0,
            specificRegions = new string[] { "София", "Пловдив", "Варна" }
        });

        questions.Add(new Question
        {
            questionText = "Колко е населението на България?",
            answers = new string[] { "Около 5 милиона", "Около 7 милиона", "Около 10 милиона", "Около 15 милиона" },
            correctAnswerIndex = 1
        });

        questions.Add(new Question
        {
            questionText = "Кой е столицата на България?",
            answers = new string[] { "Пловдив", "София", "Варна", "Бургас" },
            correctAnswerIndex = 1
        });

        questions.Add(new Question
        {
            questionText = "През коя година България влиза в Европейския съюз?",
            answers = new string[] { "2004", "2007", "2010", "2013" },
            correctAnswerIndex = 1
        });
    }

    public Question GetCurrentQuestion() => currentQuestion;
}
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class QuestionPanel : MonoBehaviour
{
    [System.Serializable]
    public class Question
    {
        public string questionText;
        public string[] answers;
        public int correctAnswerIndex;
        public string[] specificRegions;

        public bool IsValidForRegion(string regionName)
        {
            if (specificRegions == null || specificRegions.Length == 0)
                return true;

            foreach (string region in specificRegions)
            {
                if (region == regionName) return true;
            }
            return false;
        }
    }

    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI[] answerTexts;
    public TextMeshProUGUI feedbackText;
    public Image feedbackPanel;

    [SerializeField] private List<Question> questions = new List<Question>();
    private Question currentQuestion;
    private RegionData currentRegion;
    private RegionInteraction regionInteraction;

    private Color correctColor = new Color(0.2f, 0.8f, 0.2f, 0.8f); // Зелен
    private Color incorrectColor = new Color(0.8f, 0.2f, 0.2f, 0.8f); // Червен

    void Start()
    {
        HidePanel();
        regionInteraction = FindObjectOfType<RegionInteraction>();

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int buttonIndex = i;
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(buttonIndex));
        }

        if (questions.Count == 0) AddExampleQuestions();
        if (feedbackPanel != null) feedbackPanel.gameObject.SetActive(false);
    }

    public void ShowPanel() => gameObject.SetActive(true);
    public void HidePanel() => gameObject.SetActive(false);

    public void SetupQuestion(RegionData region)
    {
        currentRegion = region;
        ShowQuestionForRegion(region);
    }

    void ShowQuestionForRegion(RegionData region)
    {
        if (region == null)
        {
            ShowRandomQuestion();
            return;
        }

        List<Question> validQuestions = questions.FindAll(q => q.IsValidForRegion(region.regionName));
        currentQuestion = validQuestions.Count > 0 ?
            validQuestions[Random.Range(0, validQuestions.Count)] :
            questions[Random.Range(0, questions.Count)];

        DisplayQuestion();
    }

    void ShowRandomQuestion()
    {
        if (questions.Count == 0) return;
        currentQuestion = questions[Random.Range(0, questions.Count)];
        DisplayQuestion();
    }

    void DisplayQuestion()
    {
        questionText.text = currentQuestion.questionText;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool shouldShow = i < currentQuestion.answers.Length;
            answerButtons[i].gameObject.SetActive(shouldShow);
            if (shouldShow) answerTexts[i].text = currentQuestion.answers[i];
        }

        if (feedbackPanel != null) feedbackPanel.gameObject.SetActive(false);
    }

    public void OnAnswerSelected(int answerIndex)
    {
        bool isCorrect = (answerIndex == currentQuestion.correctAnswerIndex);
        ShowFeedback(isCorrect);

        foreach (Button button in answerButtons)
        {
            button.interactable = false;
        }

        StartCoroutine(ProcessAnswerAfterDelay(isCorrect));
    }

    IEnumerator ProcessAnswerAfterDelay(bool isCorrect)
    {
        yield return new WaitForSeconds(1.5f);

        if (isCorrect)
        {
            regionInteraction.OnQuestionAnsweredCorrectly();
            if (currentRegion != null) currentRegion.UpdatePlayerInfluence(5f);
        }
        else
        {
            regionInteraction.OnQuestionAnsweredIncorrectly();
        }

        HidePanel();
    }

    void ShowFeedback(bool isCorrect)
    {
        if (feedbackPanel == null || feedbackText == null) return;

        feedbackPanel.gameObject.SetActive(true);
        feedbackPanel.color = isCorrect ? correctColor : incorrectColor;
        feedbackText.text = isCorrect ? "Правилен отговор!" : "Грешен отговор!";
    }

    // Симулира отговор на бота
    public IEnumerator SimulateBotAnswer()
    {
        yield return new WaitForSeconds(1.5f);
        bool correct = Random.value < 0.7f;
        OnAnswerSelected(correct ? currentQuestion.correctAnswerIndex :
            Random.Range(0, currentQuestion.answers.Length));
    }

    void AddExampleQuestions()
    {
        questions.Add(new Question
        {
            questionText = "Коя е най-голямата политическа партия в България?",
            answers = new string[] { "ГЕРБ", "БСП", "ИТН", "ДПС" },
            correctAnswerIndex = 0,
            specificRegions = new string[] { "София", "Пловдив", "Варна" }
        });

        questions.Add(new Question
        {
            questionText = "Колко е населението на България?",
            answers = new string[] { "Около 5 милиона", "Около 7 милиона", "Около 10 милиона", "Около 15 милиона" },
            correctAnswerIndex = 1
        });
    }

    public Question GetCurrentQuestion() => currentQuestion;
}
*/