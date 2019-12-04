using System.Collections;
using System.Collections.Generic;

public class Trick
{
    private int numPlayers;
    private Suit trumpSuit;

    private Suit leadSuit;
    private List<Card> playedCards;
    private Player winningPlayer;
    private Card winningCard;

    public Trick (int numPlayers, Suit trumpSuit)
    {
        this.numPlayers = numPlayers;
        this.trumpSuit = trumpSuit;

        leadSuit = Suit.NONE;
        playedCards = new List<Card>();
        winningPlayer = null;
        winningCard = null;
    }

    public Suit GetLeadSuit()
    {
        return leadSuit;
    }

    public Player GetWinningPlayer()
    {
        return winningPlayer;
    }

    public void AddCard(Card card, Player player)
    {
        if (DetermineWinningCard(card))
        {
            winningPlayer = player;
            winningCard = card;
        }
        playedCards.Add(card);
    }

    public bool IsComplete()
    {
        if (playedCards.Count == numPlayers)
        {
            return true;
        } else
        {
            return false;
        }
    }

    private bool DetermineWinningCard(Card card)
    {
        if (winningCard == null)
        {
            SetLeadSuit(card.GetSuit());
            return true;
        } else
        {
            if (winningCard.GetSuit() == card.GetSuit())
            {
                if (card.GetValue() > winningCard.GetValue())
                {
                    return true;
                } else
                {
                    return false;
                }
            } else
            {
                if (card.GetSuit() == trumpSuit)
                {
                    return true;
                } else
                {
                    return false;
                }
            }
        }
    }

    private void SetLeadSuit(Suit suit)
    {
        leadSuit = suit;
    }
}
