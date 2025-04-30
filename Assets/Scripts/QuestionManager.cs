// Първо, нека създадем класове за нашата структура от въпроси

using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

// Структура за един въпрос
[Serializable]
public class Question
{
    public string questionId;      // Уникален идентификатор на въпроса
    public string questionText;    // Текст на въпроса
    public List<Answer> answers;   // Възможни отговори
    public string category;        // Категория на въпроса (опционално)
    public int difficulty;         // Трудност (опционално, от 1 до 5)
    public string imageRef;        // Референция към изображение (опционално)
}

// Структура за един отговор
[Serializable]
public class Answer
{
    public string answerText;      // Текст на отговора
    public bool isCorrect;         // Дали отговорът е верен
    public string feedback;        // Обратна връзка при избор на този отговор (опционално)
}

// Структура за цялата база от въпроси
[Serializable]
public class QuestionDatabase
{
    public List<Question> questions;
}

// Мениджър клас за работа с базата данни
public class QuestionManager : MonoBehaviour
{
    private QuestionDatabase database;
    private string databasePath;

    // Категории въпроси за по-лесно филтриране
    private Dictionary<string, List<Question>> categorizedQuestions;

    void Awake()
    {
        // Пътят до JSON файла във вашата Unity папка
        databasePath = Path.Combine(Application.streamingAssetsPath, "questions.json");
       // databasePath = Path.Combine(Application.persistentDataPath, "questions.json");
        LoadDatabase();
    }

    // Зареждане на базата данни от JSON файл
    public void LoadDatabase()
    {
        try
        {
            if (File.Exists(databasePath))
            {
                string jsonData = File.ReadAllText(databasePath);
                database = JsonUtility.FromJson<QuestionDatabase>(jsonData);

                // Инициализиране на категориите
                CategorizeQuestions();

                Debug.Log($"Успешно заредени {database.questions.Count} въпроса.");
            }
            else
            {
                Debug.LogWarning("База данни с въпроси не е намерена. Създава се нова празна база.");
                database = new QuestionDatabase
                {
                    questions = new List<Question>()
                };
                SaveDatabase();
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Грешка при зареждане на базата данни: {e.Message}");
            database = new QuestionDatabase
            {
                questions = new List<Question>()
            };
        }
    }

    // Запазване на базата данни в JSON файл
    public void SaveDatabase()
    {
        try
        {
            // Уверяваме се, че директорията съществува
            string directory = Path.GetDirectoryName(databasePath);
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string jsonData = JsonUtility.ToJson(database, true); // true за форматиран изход
            File.WriteAllText(databasePath, jsonData);
            Debug.Log("Базата данни е успешно записана.");
        }
        catch (Exception e)
        {
            Debug.LogError($"Грешка при запазване на базата данни: {e.Message}");
        }
    }

    // Организиране на въпросите по категории
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

    // Добавяне на нов въпрос
    public void AddQuestion(Question question)
    {
        // Проверка за уникален ID
        if (string.IsNullOrEmpty(question.questionId))
        {
            question.questionId = Guid.NewGuid().ToString();
        }
        else if (database.questions.Exists(q => q.questionId == question.questionId))
        {
            Debug.LogWarning("Въпрос с този ID вече съществува. Генериране на нов ID.");
            question.questionId = Guid.NewGuid().ToString();
        }

        database.questions.Add(question);

        // Обновяване на категориите
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

    // Премахване на въпрос по ID
    public bool RemoveQuestion(string questionId)
    {
        Question questionToRemove = database.questions.Find(q => q.questionId == questionId);
        if (questionToRemove != null)
        {
            database.questions.Remove(questionToRemove);

            // Обновяване на категориите
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

    // Получаване на случаен въпрос
    public Question GetRandomQuestion()
    {
        if (database.questions.Count == 0)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, database.questions.Count);
        return database.questions[randomIndex];
    }

    // Получаване на случаен въпрос от определена категория
    public Question GetRandomQuestionFromCategory(string category)
    {
        if (!categorizedQuestions.ContainsKey(category) ||
            categorizedQuestions[category].Count == 0)
            return null;

        int randomIndex = UnityEngine.Random.Range(0, categorizedQuestions[category].Count);
        return categorizedQuestions[category][randomIndex];
    }

    // Получаване на всички въпроси
    public List<Question> GetAllQuestions()
    {
        return database.questions;
    }

    // Получаване на всички въпроси от определена категория
    public List<Question> GetQuestionsByCategory(string category)
    {
        if (categorizedQuestions.ContainsKey(category))
            return categorizedQuestions[category];
        return new List<Question>();
    }

    // Получаване на въпрос по ID
    public Question GetQuestionById(string questionId)
    {
        return database.questions.Find(q => q.questionId == questionId);
    }
}