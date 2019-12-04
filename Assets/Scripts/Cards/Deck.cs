using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour {

    [SerializeField] private List<Card> cards;

    public Card DrawRandomCard()
    {
        int rand = Random.Range(0, cards.Count - 1);

        Card retCard = cards[rand];
        cards.Remove(retCard);

        return retCard;
    }

    public Card DrawSpecificCard(string id)
    {
        Card retCard = null;

        foreach(Card c in cards)
        {
            if (c.GetID() == id)
            {
                retCard = c;
                cards.Remove(retCard);
                break;
            }
        }

        return retCard;
    }
}
