// �����, ���� �������� ������� �� ������ ��������� �� �������

using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// ��������� �� ���� ������
[Serializable]
public class Question
{
    public string questionId;      // �������� ������������� �� �������
    public string questionText;    // ����� �� �������
    public List<Answer> answers;   // �������� ��������
    public string category;        // ��������� �� ������� (����������)
    public int difficulty;         // �������� (����������, �� 1 �� 5)
    public string imageRef;        // ���������� ��� ����������� (����������)
}

// ��������� �� ���� �������
[Serializable]
public class Answer
{
    public string answerText;      // ����� �� ��������
    public bool isCorrect;         // ���� ��������� � �����
    public string feedback;        // ������� ������ ��� ����� �� ���� ������� (����������)
}

// ��������� �� ������ ���� �� �������
[Serializable]
public class QuestionDatabase
{
    public List<Question> questions;
}

// �������� ���� �� ������ � ������ �����
public class QuestionManager : MonoBehaviour
{
    private QuestionDatabase database;
    private string databasePath;

    // ��������� ������� �� ��-����� ����������
    private Dictionary<string, List<Question>> categorizedQuestions;

    void Awake()
    {
        // ����� �� JSON ����� ��� ������ Unity �����
        databasePath = Path.Combine(Application.streamingAssetsPath, "questions.json");
       // databasePath = Path.Combine(Application.persistentDataPath, "questions.json");
        LoadDatabase();
    }

    // ��������� �� ������ ����� �� JSON ����
    public void LoadDatabase()
    {
        try
        {
            if (File.Exists(databasePath))
            {
                string jsonData = File.ReadAllText(databasePath);
                database = JsonUtility.FromJson<QuestionDatabase>(jsonData);

                // �������������� �� �����������
                CategorizeQuestions();

                Debug.Log($"������� �������� {database.questions.Count} �������.");
            }
            else
            {
                Debug.LogWarning("���� ����� � ������� �� � ��������. ������� �� ���� ������ ����.");
                database = new QuestionDatabase
                {
                    questions = new List<Question>()
                };
                SaveDatabase();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"������ ��� ��������� �� ������ �����: {e.Message}");
            database = new QuestionDatabase
            {
                questions = new List<Question>()
            };
        }
    }

    // ��������� �� ������ ����� � JSON ����
    public void SaveDatabase()
    {
        try
        {
            // ��������� ��, �� ������������ ����������
            string directory = Path.GetDirectoryName(databasePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string jsonData = JsonUtility.ToJson(database, true); // true �� ���������� �����
            File.WriteAllText(databasePath, jsonData);
            Debug.Log("������ ����� � ������� ��������.");
        }
        catch (Exception e)
        {
            Debug.LogError($"������ ��� ��������� �� ������ �����: {e.Message}");
        }
    }

    // ������������ �� ��������� �� ���������
    private void CategorizeQuestions()
    {
        categorizedQuestions = new Dictionary<string, List<Question>>();

        foreach (Question q in database.questions)
        {
            if (!string.IsNullOrEmpty(q.category))
            {
                if (!categorizedQuestions.ContainsKey(q.category))
                {
                    categorizedQuestions[q.category] = new List<Question>();
                }
                categorizedQuestions[q.category].Add(q);
            }
        }
    }

    // �������� �� ��� ������
    public void AddQuestion(Question question)
    {
        // �������� �� �������� ID
        if (string.IsNullOrEmpty(question.questionId))
        {
            question.questionId = Guid.NewGuid().ToString();
        }
        else if (database.questions.Exists(q => q.questionId == question.questionId))
        {
            Debug.LogWarning("������ � ���� ID ���� ����������. ���������� �� ��� ID.");
            question.questionId = Guid.NewGuid().ToString();
        }

        database.questions.Add(question);

        // ���������� �� �����������
        if (!string.IsNullOrEmpty(question.category))
        {
            if (!categorizedQuestions.ContainsKey(question.category))
            {
                categorizedQuestions[question.category] = new List<Question>();
            }
            categorizedQuestions[question.category].Add(question);
        }

        SaveDatabase();
    }

    // ���������� �� ������ �� ID
    public bool RemoveQuestion(string questionId)
    {
        Question questionToRemove = database.questions.Find(q => q.questionId == questionId);
        if (questionToRemove != null)
        {
            database.questions.Remove(questionToRemove);

            // ���������� �� �����������
            if (!string.IsNullOrEmpty(questionToRemove.category) &&
                categorizedQuestions.ContainsKey(questionToRemove.category))
            {
                categorizedQuestions[questionToRemove.category].Remove(questionToRemove);
            }

            SaveDatabase();
            return true;
        }
        return false;
    }

    // ���������� �� ������� ������
    public Question GetRandomQuestion()
    {
        if (database.questions.Count == 0)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, database.questions.Count);
        return database.questions[randomIndex];
    }

    // ���������� �� ������� ������ �� ���������� ���������
    public Question GetRandomQuestionFromCategory(string category)
    {
        if (!categorizedQuestions.ContainsKey(category) ||
            categorizedQuestions[category].Count == 0)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, categorizedQuestions[category].Count);
        return categorizedQuestions[category][randomIndex];
    }

    // ���������� �� ������ �������
    public List<Question> GetAllQuestions()
    {
        return database.questions;
    }

    // ���������� �� ������ ������� �� ���������� ���������
    public List<Question> GetQuestionsByCategory(string category)
    {
        if (categorizedQuestions.ContainsKey(category))
            return categorizedQuestions[category];
        return new List<Question>();
    }

    // ���������� �� ������ �� ID
    public Question GetQuestionById(string questionId)
    {
        return database.questions.Find(q => q.questionId == questionId);
    }
}