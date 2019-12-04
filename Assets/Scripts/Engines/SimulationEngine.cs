using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulationEngine : Engine
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
        
    }

    private void DisplayMainPlayerHand()
    {
        
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
