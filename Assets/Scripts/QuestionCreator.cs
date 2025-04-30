using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Пример за създаване на въпрос от редактора или кода
public class QuestionCreator : MonoBehaviour
{
    private QuestionManager questionManager;

    void Start()
    {
        questionManager = FindObjectOfType<QuestionManager>();
        if (questionManager == null)
        {
            Debug.LogError("QuestionManager не е намерен в сцената!");
            return;
        }

        // Пример за добавяне на въпрос програмно
        CreateSampleQuestions();
    }

    void CreateSampleQuestions()
    {
        // Създаваме първия въпрос
        Question question1 = new Question
        {
            questionText = "Коя е столицата на България?",
            category = "География",
            difficulty = 1,
            answers = new List<Answer>
            {
                new Answer { answerText = "София", isCorrect = true, feedback = "Точно така! София е столицата на България от 1879 г." },
                new Answer { answerText = "Пловдив", isCorrect = false, feedback = "Пловдив е вторият по големина град в България." },
                new Answer { answerText = "Варна", isCorrect = false, feedback = "Варна е третият по големина град в България." },
                new Answer { answerText = "Бургас", isCorrect = false, feedback = "Бургас е четвъртият по големина град в България." }
            }
        };

        // Създаваме втория въпрос
        Question question2 = new Question
        {
            questionText = "Колко планети има в Слънчевата система?",
            category = "Астрономия",
            difficulty = 2,
            answers = new List<Answer>
            {
                new Answer { answerText = "7", isCorrect = false, feedback = "Неправилно. В Слънчевата система има 8 планети." },
                new Answer { answerText = "8", isCorrect = true, feedback = "Правилно! След 2006 г. Плутон вече не се счита за планета." },
                new Answer { answerText = "9", isCorrect = false, feedback = "Преди 2006 г. се смяташе, че са 9, но Плутон беше прекласифициран." },
                new Answer { answerText = "6", isCorrect = false, feedback = "Неправилно. В Слънчевата система има 8 планети." }
            }
        };

        // Добавяме въпросите към базата данни
        questionManager.AddQuestion(question1);
        questionManager.AddQuestion(question2);
    }
}