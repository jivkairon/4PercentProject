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
        EditorGUILayout.LabelField("�������� �� �������", EditorStyles.boldLabel);
        
        // ������ �� �������� �� ��� ������
        EditorGUILayout.LabelField("�������� �� ��� ������", EditorStyles.boldLabel);
        
        questionText = EditorGUILayout.TextField("����� �� �������:", questionText);
        category = EditorGUILayout.TextField("���������:", category);
        difficulty = EditorGUILayout.IntSlider("��������:", difficulty, 1, 5);
        
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("��������", EditorStyles.boldLabel);
        
        for (int i = 0; i < 4; i++)
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.LabelField($"������� {i + 1}:", EditorStyles.boldLabel);
            answerTexts[i] = EditorGUILayout.TextField("�����:", answerTexts[i]);
            isCorrect[i] = EditorGUILayout.Toggle("����� �������:", isCorrect[i]);
            feedbacks[i] = EditorGUILayout.TextField("������� ������:", feedbacks[i]);
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        
        if (GUILayout.Button("������ ������"))
        {
            if (string.IsNullOrEmpty(questionText))
            {
                EditorUtility.DisplayDialog("������", "������ �� �������� ����� �� �������!", "OK");
                return;
            }
            
            if (!isCorrect.Contains(true))
            {
                EditorUtility.DisplayDialog("������", "������ �� ��� ���� ���� ����� �������!", "OK");
                return;
            }
            
            // ��������� �������
            Question newQuestion = new Question
            {
                questionText = questionText,
                category = category,
                difficulty = difficulty,
                answers = new List<Answer>()
            };
            
            // �������� ����������
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
            
            // �������� ������� ��� ������ �����
            manager.AddQuestion(newQuestion);
            
            // ���������� ��������
            questionText = "";
            for (int i = 0; i < 4; i++)
            {
                answerTexts[i] = "";
                isCorrect[i] = false;
                feedbacks[i] = "";
            }
            
            EditorUtility.DisplayDialog("�����", "�������� ���� ������� �������!", "OK");
        }
        
        if (GUILayout.Button("��������� �� ������ �����"))
        {
            manager.SaveDatabase();
            EditorUtility.DisplayDialog("�����", "������ ����� ���� �������� �������!", "OK");
        }
        
        if (GUILayout.Button("������������ �� ������ �����"))
        {
            manager.LoadDatabase();
            EditorUtility.DisplayDialog("�����", "������ ����� ���� ����������� �������!", "OK");
        }
    }
}
#endif