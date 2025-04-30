using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������ �� ��������� �� ������ �� ��������� ��� ����
public class QuestionCreator : MonoBehaviour
{
    private QuestionManager questionManager;

    void Start()
    {
        questionManager = FindObjectOfType<QuestionManager>();
        if (questionManager == null)
        {
            Debug.LogError("QuestionManager �� � ������� � �������!");
            return;
        }

        // ������ �� �������� �� ������ ���������
        CreateSampleQuestions();
    }

    void CreateSampleQuestions()
    {
        // ��������� ������ ������
        Question question1 = new Question
        {
            questionText = "��� � ��������� �� ��������?",
            category = "���������",
            difficulty = 1,
            answers = new List<Answer>
            {
                new Answer { answerText = "�����", isCorrect = true, feedback = "����� ����! ����� � ��������� �� �������� �� 1879 �." },
                new Answer { answerText = "�������", isCorrect = false, feedback = "������� � ������� �� �������� ���� � ��������." },
                new Answer { answerText = "�����", isCorrect = false, feedback = "����� � ������� �� �������� ���� � ��������." },
                new Answer { answerText = "������", isCorrect = false, feedback = "������ � ���������� �� �������� ���� � ��������." }
            }
        };

        // ��������� ������ ������
        Question question2 = new Question
        {
            questionText = "����� ������� ��� � ���������� �������?",
            category = "����������",
            difficulty = 2,
            answers = new List<Answer>
            {
                new Answer { answerText = "7", isCorrect = false, feedback = "����������. � ���������� ������� ��� 8 �������." },
                new Answer { answerText = "8", isCorrect = true, feedback = "��������! ���� 2006 �. ������ ���� �� �� ����� �� �������." },
                new Answer { answerText = "9", isCorrect = false, feedback = "����� 2006 �. �� �������, �� �� 9, �� ������ ���� ���������������." },
                new Answer { answerText = "6", isCorrect = false, feedback = "����������. � ���������� ������� ��� 8 �������." }
            }
        };

        // �������� ��������� ��� ������ �����
        questionManager.AddQuestion(question1);
        questionManager.AddQuestion(question2);
    }
}