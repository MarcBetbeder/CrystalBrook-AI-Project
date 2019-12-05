using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSetupForm : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button submitButton;
    [SerializeField] private List<TMP_InputField> inputFields;

    private int currentlySelectedIndex;

    private MainMenu menu;
    
    void Awake()
    {
        menu = FindObjectOfType<MainMenu>();
        submitButton.onClick.AddListener(SubmitButtonClick);
        closeButton.onClick.AddListener(CloseButtonClick);
    }

    private void Start()
    {
        currentlySelectedIndex = 0;
        inputFields[currentlySelectedIndex].Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitButtonClick();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentlySelectedIndex++;
            if (currentlySelectedIndex >= inputFields.Count)
            {
                currentlySelectedIndex = 0;
            }

            inputFields[currentlySelectedIndex].Select();
        }
    }

    private void SubmitButtonClick()
    {
        menu.SubmitGameSetup();
    }

    private void CloseButtonClick()
    {
        menu.CloseCurrentForm();
    }
}
