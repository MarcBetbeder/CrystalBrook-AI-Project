using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreBoard : MonoBehaviour
{
    [SerializeField] private Button continueButton;

    [SerializeField] private TextMeshProUGUI deltaText;
    [SerializeField] private TextMeshProUGUI winningAverageText;
    [SerializeField] private TextMeshProUGUI gameFinishedText;

    [SerializeField] private List<GameObject> scoreDisplays;

    [SerializeField] private Color defaultTextColor;
    [SerializeField] private Color winningTextColor;

    private GameWindow gameWindow;

    private int numPlayers;

    // Start is called before the first frame update
    void Start()
    {
        gameWindow = FindObjectOfType<GameWindow>();
        continueButton.onClick.AddListener(Continue);
    }

    public void Initialise(List<Player> players)
    {
        numPlayers = players.Count;
        SetDeltaText("0");
        SetWinningAverageText("0");

        for (int i = 0; i < numPlayers; i ++)
        {
            SetScoreDisplayName(i, players[i].GetName());
        }
    }

    public void UpdateScores(List<int> scores)
    {
        for (int i = 0; i < scores.Count; i++)
        {
            SetScoreDisplayScore(i, scores[i].ToString());
        }
    }

    public void SetDeltaText(string text)
    {
        deltaText.text = text;
    }

    public void SetWinningAverageText(string text)
    {
        winningAverageText.text = text;
    }

    public void ResetTextColors()
    {
        for (int i = 0; i < scoreDisplays.Count; i++)
        {
            SetWinner(i, false);
        }
    }

    public void SetWinner(int id, bool tf)
    {
        GameObject scoreDisplay = scoreDisplays[id];
        if (tf)
        {
            scoreDisplay.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = winningTextColor;
            scoreDisplay.transform.Find("Score").GetComponent<TextMeshProUGUI>().color = winningTextColor;
        } else
        {
            scoreDisplay.transform.Find("Name").GetComponent<TextMeshProUGUI>().color = defaultTextColor;
            scoreDisplay.transform.Find("Score").GetComponent<TextMeshProUGUI>().color = defaultTextColor;
        }
    }

    public void EnableGameFinished()
    {
        gameFinishedText.gameObject.SetActive(true);
    }

    private void Continue()
    {
        gameWindow.ContinueGame();
    }

    private void SetScoreDisplayName(int id, string name)
    {
        GameObject scoreDisplay = scoreDisplays[id];
        scoreDisplay.SetActive(true);
        scoreDisplay.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = name + ":";
    }

    private void SetScoreDisplayScore(int id, string score)
    {
        GameObject scoreDisplay = scoreDisplays[id];
        scoreDisplay.transform.Find("Score").GetComponent<TextMeshProUGUI>().text = score;
    }
}
