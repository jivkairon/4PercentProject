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
        // ������� ���������� ��� QuestionManager
        questionManager = FindObjectOfType<QuestionManager>();
        if (questionManager == null)
        {
            Debug.LogError("QuestionManager �� � ������� � �������!");
            return;
        }

        // ����������� ��������
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int buttonIndex = i; // ��������� ������� �� ������ ���������
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(buttonIndex));
        }

     //   nextButton.onClick.AddListener(LoadNextQuestion);
      //  nextButton.gameObject.SetActive(false);
        feedbackText.text = "";

        // ��������� ������ ������
        LoadNextQuestion();

        // ���������� ���������
     //   UpdateScore();
    }

    void LoadNextQuestion()
    {
        // �������� ��������� ������ � ������ �� ������� ������
       feedbackText.text = "";
        //nextButton.gameObject.SetActive(false);

        // ��������� ������� ������
        // �� �� �������� ������ �� ��������� ���������, �����������:
        // currentQuestion = questionManager.GetRandomQuestionFromCategory("������_���������");
        currentQuestion = questionManager.GetRandomQuestion();

        if (currentQuestion == null)
        {
            questionText.text = "���� ������� ������� � ������ �����.";
            foreach (Button button in answerButtons)
            {
                button.gameObject.SetActive(false);
            }
            return;
        }

        // ��������� �������
        questionText.text = currentQuestion.questionText;

        // ��������� ����������
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

        // ���������� ��������
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

        // ������������� ������ ������
        foreach (Button button in answerButtons)
        {
            button.interactable = false;
        }

        // ��������� ������� ������
        if (isCorrect)
        {
            feedbackText.text = "��������!";
            feedbackText.color = Color.green;
            score++;
        }
        else
        {
            feedbackText.text = "������ �������!";
            feedbackText.color = Color.red;
        }

        // �������� ������������ ������� ������, ��� ��� ������
    /*    if (!string.IsNullOrEmpty(currentQuestion.answers[buttonIndex].feedback))
        {
            feedbackText.text += "\n" + currentQuestion.answers[buttonIndex].feedback;
        }
    */
        // ����������� ������ �� ���������
        questionCount++;

        // ���������� ���������
     //   UpdateScore();

        // ��������� ������ �� ������� ������
        //   nextButton.gameObject.SetActive(true);

     //   feedbackText.text = "";
        StartCoroutine(LoadNextQuestionAfterDelay(2f)); // 2 ������� �����


    }
    IEnumerator LoadNextQuestionAfterDelay(float delay)
    {
       

        yield return new WaitForSeconds(delay); // ����� ����� �� ������ ������
        
        LoadNextQuestion();
    }

    void UpdateScore()
    {
        scoreText.text = $"��������: {score}/{questionCount}";
    }
}