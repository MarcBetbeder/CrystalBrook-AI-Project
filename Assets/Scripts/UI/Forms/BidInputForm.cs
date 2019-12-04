using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BidInputForm : MonoBehaviour
{
    [SerializeField] private Button submitButton;
    [SerializeField] private TextMeshProUGUI infoText;
    [SerializeField] private TextMeshProUGUI crystalBrookText;
    [SerializeField] private TMP_InputField bidInputField;

    private GameWindow gw;

    // Start is called before the first frame update
    void Start()
    {
        gw = FindObjectOfType<GameWindow>();
        submitButton.onClick.AddListener(SubmitBid);

        bidInputField.Select();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitBid();
        }
    }

    public void SetInfoText(string playerName)
    {
        infoText.text = "Please input the bid for " + playerName + ".";
    }

    public void SetCrystalBrookText(int invalidBid)
    {
        string text;
        if (invalidBid >= 0)
        {
            text = "You may not bid " + invalidBid + ".";
        } else
        {
            text = "You may bid anything!";
        }
        crystalBrookText.text = text;
    }

    public void RemoveCrystalBrookText()
    {
        crystalBrookText.text = " ";
    }

    private void SubmitBid()
    {
        gw.SubmitBid();
    }
}
