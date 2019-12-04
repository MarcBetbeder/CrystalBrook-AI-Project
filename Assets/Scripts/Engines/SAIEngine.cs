using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SAIEngine : Engine
{
    private Player mainPlayer;
    [SerializeField] private float pauseTime = 2f;

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
        StartCoroutine(CollectPlayerHand());
    }

    private IEnumerator CollectPlayerHand()
    {
        int cardsInHand = mainPlayer.GetNumCards();

        if (gm.IsHeadRound())
        {
            currentPlayer = gm.GetNextPlayer(0);
            while (currentPlayer != mainPlayer)
            {
                yield return CollectHeadCard();
            }
        }
        else
        {
            while (cardsInHand < cardsThisRound)
            {
                yield return CollectPlayerHandCard();

                cardsInHand = mainPlayer.GetNumCards();
            }
        }

        while (trumpCard == null)
        {
            yield return CollectTrumpCard();
        }

        BiddingPhase();
    }

    private IEnumerator CollectPlayerHandCard()
    {
        string info = "Please input each card in your hand, one at a time.";
        gm.CollectCardInput(info);

        while (!recievedInfo)
        {
            yield return null;
        }

        if (currentCard != null)
        {
            mainPlayer.AddCardToHand(gm.AddCardToMainHandDisplay(currentCard));
        }

        recievedInfo = false;
        currentCard = null;
    }

    private IEnumerator CollectHeadCard()
    {
        string info = "Please input the card on " + currentPlayer.GetName() + "'s head.";
        gm.CollectCardInput(info);

        while (!recievedInfo)
        {
            yield return null;
        }

        if (currentCard != null)
        {
            gm.DisplayHeadCard(currentCard, currentPlayer);
            currentPlayer.AddCardToHand(currentCard);

            currentPlayer = gm.GetNextPlayer(currentPlayer.GetID());
        }

        recievedInfo = false;
        currentCard = null;
    }

    private IEnumerator CollectTrumpCard()
    {
        string info = "Please input the trump card.";
        gm.CollectCardInput(info);

        while (!recievedInfo)
        {
            yield return null;
        }

        if (currentCard != null)
        {
            trumpCard = currentCard;
            gm.DisplayTrumpCard(currentCard);
        }

        recievedInfo = false;
        currentCard = null;
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
            yield return CollectPlayerBid(currentPlayer);
        }

        PlayPhase();
    }

    private IEnumerator CollectPlayerBid(Player player)
    {
        if (currentPlayer != mainPlayer)
        {
            gm.CollectPlayerBid(player, IsCrystalBrook(), invalidBid);
        } else
        {
            currentBid = currentPlayer.MakeRandomBid(cardsThisRound, IsCrystalBrook(), invalidBid);
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
                yield return PlayHeadCard();
            }
            else
            {
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
            string info = "Please input the card that " + currentPlayer.GetName() + " played.";
            gm.CollectCardInput(info);
        }
        else
        {
            Card play = currentPlayer.ChooseCardToPlay();
            play.PlayCard();
            yield return new WaitForSeconds(pauseTime);
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
        if (currentPlayer == mainPlayer)
        {
            string info = "Please input the card that was on " + mainPlayer.GetName() + "'s head.";
            gm.CollectCardInput(info);
        }
        else
        {
            currentCard = gm.GetHeadCard(currentPlayer);
            recievedInfo = true;
        }

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
