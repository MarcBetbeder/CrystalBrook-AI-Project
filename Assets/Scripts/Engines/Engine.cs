using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Engine : MonoBehaviour
{
    protected GameManager gm;
    
    protected int numPlayers;
    protected int maxHandSize;
    protected int zeroValue;

    protected Deck deck;
    protected Player currentPlayer;
    protected Player currentDealer;
    protected Player currentLeader;
    protected int cardsThisRound;
    protected Card trumpCard;

    protected bool recievedInfo = false;
    protected Card currentCard = null;
    protected int bidsRecieved = 0;
    protected int currentBid = -1;
    protected int invalidBid;
    protected int tricksAwarded = 0;
    protected int cardsPlayed = 0;
    protected Trick currentTrick = null;

    public abstract void Initialise(GameManager gm, int numPlayers, int maxHandSize, int zeroValue);

    public void PrepareRound(int numCards, Player dealer, Deck deck)
    {
        cardsThisRound = numCards;
        currentDealer = dealer;
        currentLeader = gm.GetNextPlayer(dealer.GetID());
        currentPlayer = currentLeader;
        this.deck = deck;

        DealCards();
    }

    protected abstract void DealCards();

    protected abstract void BiddingPhase();

    protected abstract void PlayPhase();

    protected virtual void ScoringPhase()
    {
        Debug.Log("Scoring the Round...");

        gm.ScoreRound();

        gm.EndRoundAssessment();

        gm.FinaliseRound();
    }

    public void RecievedInfo(bool tf)
    {
        recievedInfo = tf;
    }

    public void SetCurrentCard(Card card)
    {
        currentCard = card;
    }

    public Card GetTrumpCard()
    {
        return trumpCard;
    }

    public int GetInvalidBid()
    {
        return invalidBid;
    }

    public bool IsCrystalBrook()
    {
        if (currentPlayer == currentDealer && cardsThisRound != 1)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public void SetCurrentBid(int bid)
    {
        currentBid = bid;
    }

    public int GetMaxHandSize()
    {
        return maxHandSize;
    }

    public int GetCardsThisRound()
    {
        return cardsThisRound;
    }

    public List<Card> ConstrainHand(List<Card> hand)
    {
        List<Card> legalHand = new List<Card>();

        if (currentTrick != null)
        {
            int count = 0;

            foreach (Card c in hand)
            {
                if (c.GetSuit() == currentTrick.GetLeadSuit())
                {
                    legalHand.Add(c);
                    count++;
                }
            }

            if (count == 0)
            {
                legalHand = hand;
            }
        }

        return legalHand;
    }

    public string GetPlacingByID(int id)
    {
        return gm.DeterminePlacings()[id];
    }

    public void SafeRoundReset()
    {
        StopAllCoroutines();
        deck = null;
        currentDealer = null;
        currentPlayer = null;
        currentLeader = null;
        trumpCard = null;
        recievedInfo = false;
        currentCard = null;
    }

    public bool IsHeadRound()
    {
        return gm.IsHeadRound();
    }

    public bool IsCrystalBrooked(int id)
    {
        if (id == currentDealer.GetID() && cardsThisRound != 1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public List<Card> GetVisibleCards(int id)
    {
        return gm.GetVisibleCards(id);
    }
}
