using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameWindow : UIManager
{
    [SerializeField] private ScoreBoard scoreBoard;
    [SerializeField] private TextMeshProUGUI roundInfo;
    [SerializeField] private Button resetRoundButton;

    [SerializeField] private GameObject cardInputFormPrefab;
    [SerializeField] private GameObject bidInputFormPrefab;

    public void ContinueGame()
    {
        if (gm.GameHasFinished())
        {
            FindObjectOfType<SceneLoader>().LoadMainMenu();
        }
        else
        {
            DisableButtons(false);
            EnableScoreBoard(false);
            EnableResetRoundButton(true);
            gm.NewRound();
        }
    }

    public void InitialiseScoreBoard(List<Player> players)
    {
        scoreBoard.Initialise(players);
    }

    public void UpdateScoreBoard(List<int> scores, int delta, float winningAverage)
    {
        scoreBoard.UpdateScores(scores);
        scoreBoard.SetDeltaText(delta.ToString());
        scoreBoard.SetWinningAverageText(winningAverage.ToString("0.00"));
        scoreBoard.ResetTextColors();
    }

    public void SetWinners(List<Player> winners)
    {
        foreach (Player player in winners)
        {
            scoreBoard.SetWinner(player.GetID(), true);
        }
    }

    public void UpdateRoundInfo(int roundNum, int numCards)
    {
        roundInfo.text = "Round " + roundNum + " (" + numCards + ")";
    }

    public void ShowScoreBoard()
    {
        EnableResetRoundButton(false);
        DisableButtons(true);
        EnableScoreBoard(true);

        if (gm.GameHasFinished())
        {
            scoreBoard.EnableGameFinished();
        }
    }

    public void InputCard(string infoText)
    {
        GameObject cardInputForm = Instantiate(cardInputFormPrefab);
        cardInputForm.transform.SetParent(FindObjectOfType<Canvas>().transform);
        cardInputForm.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        cardInputForm.name = "Card Input Form";
        cardInputForm.transform.SetAsLastSibling();
        cardInputForm.GetComponent<CardInputForm>().SetInfoText(infoText);
        currentForm = cardInputForm;
    }

    public void SubmitCard()
    {
        List<Transform> answers = GetAnswers(currentForm.transform);

        string cardID = null;

        foreach (Transform answer in answers)
        {
            if (answer.name == "Card")
            {
                cardID = answer.GetComponent<TMP_InputField>().text;
            }
        }

        gm.SetCurrentCard(cardID);

        CloseCurrentForm();
    }

    public void InputBid(string playerName, bool crystalBrook, int invalidBid)
    {
        GameObject bidInputForm = Instantiate(bidInputFormPrefab);
        bidInputForm.transform.SetParent(FindObjectOfType<Canvas>().transform);
        bidInputForm.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        bidInputForm.name = "Bid Input Form";
        bidInputForm.transform.SetAsLastSibling();
        bidInputForm.GetComponent<BidInputForm>().SetInfoText(playerName);
        if (crystalBrook)
        {
            bidInputForm.GetComponent<BidInputForm>().SetCrystalBrookText(invalidBid);
        } else
        {
            bidInputForm.GetComponent<BidInputForm>().RemoveCrystalBrookText();
        }
        currentForm = bidInputForm;
    }

    public void SubmitBid()
    {
        List<Transform> answers = GetAnswers(currentForm.transform);

        int bid = -1;

        foreach (Transform answer in answers)
        {
            if (answer.name == "Bid")
            {
                bid = int.Parse(answer.GetComponent<TMP_InputField>().text);
            }
        }

        gm.SetCurrentBid(bid);

        CloseCurrentForm();
    }

    public void ResetRound()
    {
        gm.ResetCurrentRound();
        CloseCurrentForm();
        ShowScoreBoard();
    }

    private void EnableScoreBoard(bool tf)
    {
        scoreBoard.gameObject.SetActive(tf);
    }

    private void EnableResetRoundButton (bool tf)
    {
        resetRoundButton.gameObject.SetActive(tf);
    }
}
