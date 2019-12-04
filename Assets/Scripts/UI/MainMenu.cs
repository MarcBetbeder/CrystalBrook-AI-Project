using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MainMenu : UIManager
{
    // UI References
    [SerializeField] private GameObject gameSetupFormPrefab;
    [SerializeField] private GameObject playerSetupFormPrefab;
    [SerializeField] private GameObject firstDealerFormPrefab;

    public void GameSetup()
    {
        DisableButtons(true);

        GameObject gameSetupForm = Instantiate(gameSetupFormPrefab);
        gameSetupForm.transform.SetParent(FindObjectOfType<Canvas>().transform);
        gameSetupForm.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        gameSetupForm.name = "Game Setup Form";
        gameSetupForm.transform.SetAsLastSibling();

        currentForm = gameSetupForm;
    }

    public void SubmitGameSetup(bool tf)
    {
        List<Transform> answers = GetAnswers(currentForm.transform);

        int numPlayers = 0;
        int maxHandSize = 0;
        int zeroValue = 0;
        GameMode mode = GameMode.EMULATOR;

        foreach (Transform answer in answers)
        {
            if (answer.name == "Players")
            {
                numPlayers = int.Parse(answer.GetComponent<TMP_InputField>().text);

            }
            else if (answer.name == "HandSize")
            {
                maxHandSize = int.Parse(answer.GetComponent<TMP_InputField>().text);

            }
            else if (answer.name == "Zero")
            {
                zeroValue = int.Parse(answer.GetComponent<TMP_InputField>().text);

            }
            else if (answer.name == "Mode")
            {
                switch (answer.GetComponent<TMP_Dropdown>().value)
                {
                    case 0:
                        mode = GameMode.EMULATOR;
                        break;
                    case 1:
                        mode = GameMode.SINGLE_PLAYER;
                        break;
                    case 2:
                        mode = GameMode.SINGLE_AI;
                        break;
                    case 3:
                        mode = GameMode.SIMULATOR;
                        break;
                    default:
                        break;
                }
            }
        }

        gm.ApplyGameSetup(numPlayers, maxHandSize, zeroValue, mode, tf);

        CloseCurrentForm();

        SetupPlayers(numPlayers);
    }

    private void SetupPlayers(int numPlayers)
    {
        DisableButtons(true);

        GameObject playerSetupForm = Instantiate(playerSetupFormPrefab);
        playerSetupForm.transform.SetParent(FindObjectOfType<Canvas>().transform);
        playerSetupForm.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        playerSetupForm.name = "Player Setup Form";
        playerSetupForm.transform.SetAsLastSibling();
        currentForm = playerSetupForm;

        currentForm.GetComponent<PlayerSetupForm>().SetupForm(numPlayers);
    }

    public void SubmitPlayerSetup(Transform contentWindow)
    {
        List<Transform> answers = GetAnswers(contentWindow);

        foreach (Transform answer in answers)
        {
            gm.SetPlayerName(int.Parse(answer.name), answer.GetComponent<TMP_InputField>().text);
        }

        CloseCurrentForm();

        SetupDealer(gm.GetNumPlayers());
    }

    public void SetupDealer(int numPlayers)
    {
        DisableButtons(true);

        GameObject firstDealerForm = Instantiate(firstDealerFormPrefab);
        firstDealerForm.transform.SetParent(FindObjectOfType<Canvas>().transform);
        firstDealerForm.GetComponent<RectTransform>().localPosition = new Vector3(0, 0);
        firstDealerForm.name = "First Dealer Form";
        firstDealerForm.transform.SetAsLastSibling();

        currentForm = firstDealerForm;

        currentForm.GetComponent<FirstDealerForm>().ConstrainTextInput(numPlayers);
    }

    public void SubmitFirstDealer(int firstDealer)
    {
        gm.SetFirstDealer(firstDealer);

        gm.StartGame();
    }
}
