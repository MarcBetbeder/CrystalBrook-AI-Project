using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardInputForm : MonoBehaviour
{
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TMP_InputField cardInputField;

    private GameWindow gw;

    // Start is called before the first frame update
    void Start()
    {
        gw = FindObjectOfType<GameWindow>();
        submitButton.onClick.AddListener(SubmitCard);

        cardInputField.Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitCard();
        }
    }

    public void SetInfoText(string text)
    {
        infoText.text = text;
    }

    private void SubmitCard()
    {
        gw.SubmitCard();
    }
}
