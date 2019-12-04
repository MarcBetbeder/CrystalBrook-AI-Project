using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    protected GameManager gm;
    [SerializeField] protected GameObject buttonDisabler;
    protected GameObject currentForm = null;

    // Start is called before the first frame update
    protected void Start()
    {
        gm = FindObjectOfType<GameManager>();
    }

    protected List<Transform> GetAnswers(Transform form)
    {
        List<Transform> answers = new List<Transform>();

        foreach (Transform child in form)
        {
            if (child.tag == "Question Group")
            {
                foreach (Transform child2 in child)
                {
                    if (child2.tag == "Answer")
                    {
                        answers.Add(child2);
                    }
                }
            }
        }

        return answers;
    }

    public void CloseCurrentForm()
    {
        Destroy(currentForm);
        currentForm = null;

        DisableButtons(false);
    }

    protected void DisableButtons(bool tf)
    {
        buttonDisabler.SetActive(tf);
    }
}
