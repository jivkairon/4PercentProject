using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PopupManager : MonoBehaviour
{
    public GameObject popupPanel; // Reference to the popup panel
    public TMP_InputField gameCodeInput; // Reference to the input field

    // Function to show the popup
    public void ShowPopup()
    {
        popupPanel.SetActive(true);
    }

    // Function to handle the "Join" button
    public void OnJoinButtonClick()
    {
        string enteredCode = gameCodeInput.text;

        if (!string.IsNullOrEmpty(enteredCode))
        {
            // Add your logic here to join the game
        }
        else
        {
            Debug.Log("Game code is empty!");
        }

        // Optionally close the popup
        popupPanel.SetActive(false);
    }

    // Function to close the popup (if needed)
    public void ClosePopup()
    {
        popupPanel.SetActive(false);
    }
}
