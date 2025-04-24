using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel; 
    public TMP_InputField gameCodeInput; 

    // Показване на панела
    public void ShowPopup()
    {
        popupPanel.SetActive(true);
    }

    public void OnJoinButtonClick()
    {
        string enteredCode = gameCodeInput.text;

        if (!string.IsNullOrEmpty(enteredCode))
        {
            
        }
        else
        {
            Debug.Log("Game code is empty!");
        }

        
        popupPanel.SetActive(false);
    }

    
    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}
