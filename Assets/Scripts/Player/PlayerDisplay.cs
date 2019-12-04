using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerDisplay : MonoBehaviour
{
    private Transform cardDestination;
    [SerializeField] private int id;
    private string playerName;
    private GameObject headCard;

    TextMeshProUGUI bidText;
    TextMeshProUGUI nameText;
    TextMeshProUGUI tricksText;

    public void SetLocation(Transform target)
    {
        transform.position = target.position;
    }

    public void SetCardDestination(Transform target)
    {
        cardDestination = target;
    }

    public Transform GetCardDestination()
    {
        return cardDestination;
    }

    public void SetID(int id)
    {
        this.id = id;
    }

    public int GetID()
    {
        return id;
    }

    public void SetTextReferences(Transform container)
    {
        bidText = container.Find("Bid").GetComponent<TextMeshProUGUI>();
        nameText = container.Find("Name").GetComponent<TextMeshProUGUI>();
        tricksText = container.Find("Tricks").GetComponent<TextMeshProUGUI>();
    }

    public void SetName(string name)
    {
        playerName = name;
        ResetNameText();
    }

    public void ResetNameText()
    {
        nameText.text = playerName;
    }

    public void SetDealer()
    {
        nameText.text = playerName + "*";
    }

    public void SetBid(string bid)
    {
        bidText.text = bid;
    }

    public void WipeBid()
    {
        bidText.text = " ";
    }

    public void SetTricks(string tricks)
    {
        tricksText.text = tricks;
    }

    public void WipeTricks()
    {
        tricksText.text = " ";
    }

    public void SetHeadCard(GameObject card)
    {
        headCard = card;
    }

    public GameObject GetHeadCard()
    {
        return headCard;
    }

    public void WipeHeadCard()
    {
        if (headCard != null)
        {
            Destroy(headCard);
            headCard = null;
        }
    }

    public void SetSprite(Sprite sprite)
    {
        GetComponent<SpriteRenderer>().sprite = sprite;
    }
}
