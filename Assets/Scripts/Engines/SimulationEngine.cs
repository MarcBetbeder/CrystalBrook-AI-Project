using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationEngine : Engine
{
    private Player mainPlayer;
    [SerializeField] private float pauseTime = 1f;
    [SerializeField] private float cardMoveSpeed = 10f;

    public override void Initialise(GameManager gm, int numPlayers, int maxHandSize, int zeroValue)
    {
        this.gm = gm;

        this.numPlayers = numPlayers;

        this.maxHandSize = maxHandSize;
        this.zeroValue = zeroValue;

        mainPlayer = gm.FindPlayerByID(0);
        gm.GetDisplayManager().SetCardMoveSpeed(cardMoveSpeed);
    }

    protected override void DealCards()
    {
        Debug.Log("Dealing Cards...");

        if (gm.IsHeadRound())
        {
            for (int i = 0; i < numPlayers; i++)
            {
                Card headCard = deck.DrawRandomCard();
                currentPlayer.AddCardToHand(headCard);
                gm.DisplayHeadCard(headCard, currentPlayer);
                currentPlayer = gm.GetNextPlayer(currentPlayer.GetID());
            }
        }
        else
        {
            gm.DealFullRound();

            DisplayMainPlayerHand();
        }

        trumpCard = deck.DrawRandomCard();
        gm.DisplayTrumpCard(trumpCard);

        BiddingPhase();
    }

    private void DisplayMainPlayerHand()
    {
        List<Card> hand = mainPlayer.GetHand();

        foreach (Card c in hand)
        {
            gm.AddCardToMainHandDisplay(c);
        }
    }

    protected override void BiddingPhase()
    {
        
    }

    private IEnumerator CollectBids()
    {
        
    }

    private IEnumerator CollectPlayerBid(Player player)
    {
        
    }

    protected override void PlayPhase()
    {
        
    }

    private IEnumerator PlayAllTricks()
    {
        
    }

    private IEnumerator PlayTrick()
    {
        
    }

    private IEnumerator PlayCard()
    {
        
    }

    private IEnumerator PlayHeadCard()
    {
        
    }
}
