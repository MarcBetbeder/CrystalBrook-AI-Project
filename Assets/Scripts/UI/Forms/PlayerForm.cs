using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerForm : MonoBehaviour
{
    public void SetNameText(string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == "Player Title")
            {
                child.GetComponent<TextMeshProUGUI>().SetText(name);
            }
        }
    }

    public void SetAnswerName(string name)
    {
        foreach (Transform child in transform)
        {
            if (child.name == "Name")
            {
                child.name = name;
            }
        }
    }
}
