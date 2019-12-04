using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SPEngine : Engine
{
    private Player mainPlayer;
    [SerializeField] private float pauseTime = 1f;

    public override void Initialise(GameManager gm, int numPlayers, int maxHandSize, int zeroValue)
    {
        this.gm = gm;

        this.numPlayers = numPlayers;

        this.maxHandSize = maxHandSize;
        this.zeroValue = zeroValue;

        mainPlayer = gm.FindPlayerByID(0);
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
                if (currentPlayer != mainPlayer)
                {
                    gm.DisplayHeadCard(headCard, currentPlayer);
                }
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
        Debug.Log("Collecting Bids...");

        invalidBid = cardsThisRound;
        if (cardsThisRound == 1)
        {
            currentPlayer = mainPlayer;
        }
        else
        {
            currentPlayer = currentLeader;
        }
        StartCoroutine(CollectBids());
    }

    private IEnumerator CollectBids()
    {
        bidsRecieved = 0;

        while (bidsRecieved < numPlayers)
        {
            yield return new WaitForSeconds(pauseTime);

            yield return CollectPlayerBid(currentPlayer);
        }

        PlayPhase();
    }

    private IEnumerator CollectPlayerBid(Player player)
    {
        if (currentPlayer == mainPlayer)
        {
            gm.CollectPlayerBid(player, IsCrystalBrook(), invalidBid);
        } else
        {
            currentBid = player.MakeRandomBid(cardsThisRound, IsCrystalBrook(), invalidBid);
            recievedInfo = true;
        }

        while (!recievedInfo)
        {
            yield return null;
        }

        if (currentBid >= 0)
        {
            player.SetBid(currentBid);
            invalidBid -= currentBid;
            gm.DisplayPlayerBid(player);
            bidsRecieved++;
            currentPlayer = gm.GetNextPlayer(player.GetID());
        }

        recievedInfo = false;
        currentBid = -1;
    }

    protected override void PlayPhase()
    {
        Debug.Log("Playing Cards...");

        gm.PlayPhaseDisplay();

        StartCoroutine(PlayAllTricks());
    }

    private IEnumerator PlayAllTricks()
    {
        tricksAwarded = 0;

        while (tricksAwarded < cardsThisRound)
        {
            yield return new WaitForSeconds(pauseTime);
            yield return PlayTrick();
        }

        ScoringPhase();
    }

    private IEnumerator PlayTrick()
    {
        currentTrick = new Trick(numPlayers, trumpCard.GetSuit());
        currentPlayer = currentLeader;

        while (!currentTrick.IsComplete())
        {
            if (gm.IsHeadRound())
            {
                yield return new WaitForSeconds(pauseTime);
                yield return PlayHeadCard();
            }
            else
            {
                yield return new WaitForSeconds(pauseTime);
                yield return PlayCard();
            }
        }

        yield return gm.WaitForMovingCards();

        Debug.Log(currentTrick.GetWinningPlayer().GetName() + " has won this trick!");

        yield return gm.AwardTrick(currentTrick.GetWinningPlayer());

        tricksAwarded++;
        currentLeader = currentTrick.GetWinningPlayer();
    }

    private IEnumerator PlayCard()
    {
        if (currentPlayer != mainPlayer)
        {
            currentCard = currentPlayer.ChooseCardToPlay();
            recievedInfo = true;
        }
        else
        {
            gm.ConstrainMainPlayerHandDisplay(currentTrick);
        }

        while (!recievedInfo)
        {
            yield return null;
        }

        if (currentCard != null)
        {
            if (currentPlayer != mainPlayer)
            {
                gm.DisplayPlayedCard(currentCard, currentPlayer);
            }
            else
            {
                mainPlayer.PlayCard(currentCard);
            }

            currentTrick.AddCard(currentCard, currentPlayer);
            currentPlayer = gm.GetNextPlayer(currentPlayer.GetID());
        }

        recievedInfo = false;
        currentCard = null;
    }

    private IEnumerator PlayHeadCard()
    {
        currentCard = gm.GetHeadCard(currentPlayer);
        recievedInfo = true;

        while (!recievedInfo)
        {
            yield return null;
        }

        if (currentCard != null)
        {
            if (currentPlayer != mainPlayer)
            {
                gm.PlayHeadCard(currentPlayer);
            }
            else
            {
                gm.DisplayPlayedCard(currentCard, currentPlayer);
            }

            currentTrick.AddCard(currentCard, currentPlayer);
            currentPlayer = gm.GetNextPlayer(currentPlayer.GetID());
        }

        recievedInfo = false;
        currentCard = null;
    }
}
