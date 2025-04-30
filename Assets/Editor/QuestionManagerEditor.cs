#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

[CustomEditor(typeof(QuestionManager))]
public class QuestionManagerEditor : Editor
{
    private string questionText = "";
    private string category = "";
    private int difficulty = 1;
    private List<string> answerTexts = new List<string> { "", "", "", "" };
    private List<bool> isCorrect = new List<bool> { false, false, false, false };
    private List<string> feedbacks = new List<string> { "", "", "", "" };
    
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        QuestionManager manager = (QuestionManager)target;
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Редактор на въпроси", EditorStyles.boldLabel);
        
        // Секция за добавяне на нов въпрос
        EditorGUILayout.LabelField("Добавяне на нов въпрос", EditorStyles.boldLabel);
        
        questionText = EditorGUILayout.TextField("Текст на въпроса:", questionText);
        category = EditorGUILayout.TextField("Категория:", category);
        difficulty = EditorGUILayout.IntSlider("Трудност:", difficulty, 1, 5);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Отговори", EditorStyles.boldLabel);
        
        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"Отговор {i + 1}:", EditorStyles.boldLabel);
            answerTexts[i] = EditorGUILayout.TextField("Текст:", answerTexts[i]);
            isCorrect[i] = EditorGUILayout.Toggle("Верен отговор:", isCorrect[i]);
            feedbacks[i] = EditorGUILayout.TextField("Обратна връзка:", feedbacks[i]);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        
        if (GUILayout.Button("Добави въпрос"))
        {
            if (string.IsNullOrEmpty(questionText))
            {
                EditorUtility.DisplayDialog("Грешка", "Трябва да въведете текст на въпроса!", "OK");
                return;
            }
            
            if (!isCorrect.Contains(true))
            {
                EditorUtility.DisplayDialog("Грешка", "Трябва да има поне един верен отговор!", "OK");
                return;
            }
            
            // Създаваме въпроса
            Question newQuestion = new Question
            {
                questionText = questionText,
                category = category,
                difficulty = difficulty,
                answers = new List<Answer>()
            };
            
            // Добавяме отговорите
            for (int i = 0; i < 4; i++)
            {
                if (!string.IsNullOrEmpty(answerTexts[i]))
                {
                    Answer answer = new Answer
                    {
                        answerText = answerTexts[i],
                        isCorrect = isCorrect[i],
                        feedback = feedbacks[i]
                    };
                    newQuestion.answers.Add(answer);
                }
            }
            
            // Добавяме въпроса към базата данни
            manager.AddQuestion(newQuestion);
            
            // Изчистваме полетата
            questionText = "";
            for (int i = 0; i < 4; i++)
            {
                answerTexts[i] = "";
                isCorrect[i] = false;
                feedbacks[i] = "";
            }
            
            EditorUtility.DisplayDialog("Успех", "Въпросът беше добавен успешно!", "OK");
        }
        
        if (GUILayout.Button("Запазване на базата данни"))
        {
            manager.SaveDatabase();
            EditorUtility.DisplayDialog("Успех", "Базата данни беше запазена успешно!", "OK");
        }
        
        if (GUILayout.Button("Презареждане на базата данни"))
        {
            manager.LoadDatabase();
            EditorUtility.DisplayDialog("Успех", "Базата данни беше презаредена успешно!", "OK");
        }
    }
}
#endif