using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class QuizGame : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private TextMeshProUGUI[] buttonTexts;
    [SerializeField] private TextMeshProUGUI feedbackText;
  //  [SerializeField] private Button nextButton;
    [SerializeField] private TextMeshProUGUI scoreText;

    private QuestionManager questionManager;
    private Question currentQuestion;
    private int score = 0;
    private int questionCount = 0;

    void Start()
    {
        // Взимаме референция към QuestionManager
        questionManager = FindObjectOfType<QuestionManager>();
        if (questionManager == null)
        {
            Debug.LogError("QuestionManager не е намерен в сцената!");
            return;
        }

        // Настройваме бутоните
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int buttonIndex = i; // Запазваме индекса за ламбда функцията
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(buttonIndex));
        }

     //   nextButton.onClick.AddListener(LoadNextQuestion);
      //  nextButton.gameObject.SetActive(false);
        feedbackText.text = "";

        // Зареждаме първия въпрос
        LoadNextQuestion();

        // Обновяваме резултата
     //   UpdateScore();
    }

    void LoadNextQuestion()
    {
        // Скриваме обратната връзка и бутона за следващ въпрос
       feedbackText.text = "";
        //nextButton.gameObject.SetActive(false);

        // Зареждаме случаен въпрос
        // За да заредите въпрос от конкретна категория, използвайте:
        // currentQuestion = questionManager.GetRandomQuestionFromCategory("вашата_категория");
        currentQuestion = questionManager.GetRandomQuestion();

        if (currentQuestion == null)
        {
            questionText.text = "Няма налични въпроси в базата данни.";
            foreach (Button button in answerButtons)
            {
                button.gameObject.SetActive(false);
            }
            return;
        }

        // Показваме въпроса
        questionText.text = currentQuestion.questionText;

        // Показваме отговорите
        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (i < currentQuestion.answers.Count)
            {
                answerButtons[i].gameObject.SetActive(true);
                buttonTexts[i].text = currentQuestion.answers[i].answerText;
            }
            else
            {
                answerButtons[i].gameObject.SetActive(false);
            }
        }

        // Активираме бутоните
        foreach (Button button in answerButtons)
        {
            button.interactable = true;
        }
    }

    void OnAnswerSelected(int buttonIndex)
    {
        if (currentQuestion == null || buttonIndex >= currentQuestion.answers.Count)
            return;

        bool isCorrect = currentQuestion.answers[buttonIndex].isCorrect;

        // Дезактивираме всички бутони
        foreach (Button button in answerButtons)
        {
            button.interactable = false;
        }

        // Показваме обратна връзка
        if (isCorrect)
        {
            feedbackText.text = "Правилно!";
            feedbackText.color = Color.green;
            score++;
        }
        else
        {
            feedbackText.text = "Грешен отговор!";
            feedbackText.color = Color.red;
        }

        // Добавяме допълнителна обратна връзка, ако има такава
    /*    if (!string.IsNullOrEmpty(currentQuestion.answers[buttonIndex].feedback))
        {
            feedbackText.text += "\n" + currentQuestion.answers[buttonIndex].feedback;
        }
    */
        // Увеличаваме брояча на въпросите
        questionCount++;

        // Обновяваме резултата
     //   UpdateScore();

        // Показваме бутона за следващ въпрос
        //   nextButton.gameObject.SetActive(true);

     //   feedbackText.text = "";
        StartCoroutine(LoadNextQuestionAfterDelay(2f)); // 2 секунди пауза


    }
    IEnumerator LoadNextQuestionAfterDelay(float delay)
    {
       

        yield return new WaitForSeconds(delay); // Малка пауза за плавен преход
        
        LoadNextQuestion();
    }

    void UpdateScore()
    {
        scoreText.text = $"Резултат: {score}/{questionCount}";
    }
}