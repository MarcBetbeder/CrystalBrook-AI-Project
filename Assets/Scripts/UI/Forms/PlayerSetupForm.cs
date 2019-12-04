using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerSetupForm : MonoBehaviour
{
    private MainMenu menu;
    [SerializeField] private Button submit;
    [SerializeField] private Button close;
    [SerializeField] private GameObject playerFormPrefab;

    [SerializeField] private float playerFormHeight;

    [SerializeField] private GameObject contentWindow;
    [SerializeField] private RectTransform contentWindowTransform;

    private List<TMP_InputField> inputFields;
    private int currentlySelectedInput = 0;

    // Start is called before the first frame update
    void Awake()
    {
        menu = FindObjectOfType<MainMenu>();
        submit.onClick.AddListener(SubmitPlayerSetup);
        close.onClick.AddListener(CloseForm);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SubmitPlayerSetup();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            currentlySelectedInput++;
            if (currentlySelectedInput >= inputFields.Count)
            {
                currentlySelectedInput = 0;
            }

            inputFields[currentlySelectedInput].Select();
        }
    }

    public void SetupForm(int numPlayers)
    {
        contentWindow = transform.Find("Scroll View").Find("Viewport").Find("Content").gameObject;
        contentWindowTransform = contentWindow.GetComponent<RectTransform>();

        inputFields = new List<TMP_InputField>();

        for (int i = 0; i < numPlayers; i++)
        {
            IncreaseContentSize();
            PlayerForm playerForm = Instantiate(playerFormPrefab).GetComponent<PlayerForm>();
            playerForm.SetAnswerName((i).ToString());
            playerForm.transform.SetParent(contentWindow.transform);
            playerForm.SetNameText("P" + (i + 1).ToString() + ":");
            float height = contentWindowTransform.rect.height - (playerFormHeight / 2);
            playerForm.transform.localPosition = new Vector3(0, -height);

            inputFields.Add(playerForm.transform.Find(i.ToString()).GetComponent<TMP_InputField>());
        }

        inputFields[currentlySelectedInput].Select();
    }
    
    private void SubmitPlayerSetup()
    {
        menu.SubmitPlayerSetup(contentWindow.transform);
    }

    private void CloseForm()
    {
        menu.CloseCurrentForm();
    }

    private void IncreaseContentSize()
    {
        float currentSize = contentWindowTransform.rect.height;
        contentWindowTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, currentSize + playerFormHeight);
    }
}
