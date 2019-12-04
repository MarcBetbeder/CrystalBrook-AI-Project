using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FirstDealerForm : MonoBehaviour
{
    [SerializeField] private Button submitButton;
    [SerializeField] private TMP_InputField answerField;
    [SerializeField] private TextMeshProUGUI infoText;

    private MainMenu menu;

    private int maxInput;

    void Awake()
    {
        menu = FindObjectOfType<MainMenu>();
        submitButton.onClick.AddListener(SubmitButtonClick);
    }

    private void Start()
    {
        answerField.Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitButtonClick();
        }
    }

    public void ConstrainTextInput(int numPlayers)
    {
        maxInput = numPlayers;
    }

    private void SubmitButtonClick()
    {
        int answer = int.Parse(answerField.text);

        if (answer > maxInput || answer < 0)
        {
            infoText.text = "Invalid Input. Number too high.";
        } else
        {
            menu.SubmitFirstDealer(answer);
        }
    }
}
