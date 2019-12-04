using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Brain
{
    // Game Components
    private Player p;
    private Engine e;

    // Brain Components
    private LongTermMemory ltm;
    private ShortTermMemory stm;

    public Brain(Engine engine, Player player)
    {
        p = player;
        e = engine;
    }

    public Card PlayRandomCard()
    {
        List<Card> hand = p.GetHand();
        List<Card> legalCards = e.ConstrainHand(hand);

        int index = Random.Range(0, legalCards.Count);
        return legalCards[index];
    }

    public int MakeBid(int cardsThisRound, bool crystalBrook, int invalidBid)
    {
        int bid = Random.Range(0, 3);

        if (bid != 0)
        {
            int rand = Random.Range(0, 1000);
            if (rand == 0)
            {
                bid = 10;
            } else
            {
                bid = 9 - Mathf.FloorToInt(Mathf.Log10(Mathf.Pow(rand, 3)));
            }

            bid = Mathf.Clamp(bid, 0, cardsThisRound);
        }

        if (crystalBrook && bid == invalidBid)
        {
            if (bid == 0)
            {
                bid++;
            } else
            {
                bid--;
            }
        }

        return bid;
    }
}
