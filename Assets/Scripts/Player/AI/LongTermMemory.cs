using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class LongTermMemory
{
    private Engine engine;
    private FileManager fm;
    private Player player;

    private string reportPath;
    private StreamWriter writer;

    private string linebreak = "===================================";
    string sep = " | ";

    private string roundline;

    private int numBidsMade = 0;
    private int numBidsMissed = 0;

    private int reward = 0;

    public LongTermMemory(Engine engine, FileManager fm, Player player)
    {
        this.engine = engine;
        this.fm = fm;
        this.player = player;

        reportPath = fm.createAIReport(player.GetName());
        InitialiseReport();
    }

    private void InitialiseReport()
    {
        using (writer = new StreamWriter(reportPath, true))
        {
            string line = player.GetName();
            writer.WriteLine(line);
            writer.WriteLine(linebreak);
        }
    }

    public void StartRoundReport()
    {
        BuildStartRoundLine();
    }

    private void BuildStartRoundLine()
    {
        roundline = "";

        string cards = engine.GetCardsThisRound().ToString();
        string trumps = GetTrumps();
        string hand = BuildHandString(player.GetHand());

        roundline += "Cards: " + cards + sep + "Trumps: " + trumps + sep;

        if (engine.IsHeadRound())
        {
            string visibleCards = BuildHandString(BuildVisibleCards());
            roundline += "Visible Cards: " + visibleCards + sep;
        }

        roundline += "Hand: " + hand + sep;
    }

    private string GetTrumps()
    {
        Card trumpCard = engine.GetTrumpCard();

        string trumps = trumpCard.GetID();

        return trumps;
    }

    private string BuildHandString(List<Card> cards)
    {
        cards.Sort();

        string handString = "<";

        for (int i = 0; i < cards.Count; i++)
        {
            handString += cards[i].ToString();

            if (i != cards.Count - 1)
            {
                handString += ", ";
            } else
            {
                handString += ">";
            }
        }

        return handString;
    }
    
    private List<Card> BuildVisibleCards()
    {
        return engine.GetVisibleCards(player.GetID());
    }

    public void EndRoundReport()
    {
        using (writer = new StreamWriter(reportPath, true))
        {
            BuildEndRoundLine();
            string line = roundline;
            writer.WriteLine(line);
        }
    }

    private void BuildEndRoundLine()
    {
        string bid = player.GetCurrentBid().ToString();
        if (engine.IsCrystalBrooked(player.GetID()))
        {
            bid += "*";
        }
        string tricks = player.GetTricks().ToString();
        string evaluation = BuildEvaluation();

        roundline += "Bid: " + bid + sep + "Tricks: " + tricks + sep + evaluation + ";";
    }

    private string BuildEvaluation()
    {
        string evaluation;

        if (player.GetTricks() == player.GetCurrentBid())
        {
            evaluation = "GOOD";
            numBidsMade++;
            reward += 10;
        } else
        {
            evaluation = "BAD";
            numBidsMissed++;
        }

        if (Mathf.Abs(player.GetTricks() - player.GetCurrentBid()) == 1)
        {
            if (engine.IsCrystalBrooked(player.GetID()))
            {
                reward += 8;
            } else
            {
                reward += 3;
            }
        }

        return evaluation;
    }

    public void FinaliseReport()
    {
        reward += player.GetScore();

        using (writer = new StreamWriter(reportPath, true))
        {
            writer.WriteLine(linebreak);
            string line = BuildSummaryLine();
            writer.WriteLine(line);
        }
    }

    private string BuildSummaryLine()
    {
        string score = player.GetScore().ToString();
        string average = CalculateAverage().ToString("0.00");
        string placing = GetPlacing();
        string made = numBidsMade.ToString();
        string missed = numBidsMissed.ToString();
        string rew = reward.ToString();

        string summary = "Place: " + placing + sep + "Final Score: " + score + sep + "Average: " + average + sep +
            "Bids Made: " + made + sep + "Bids Missed: " + missed + sep + "Reward: " + rew;

        return summary;
    }

    private float CalculateAverage()
    {
        float average = (float)player.GetScore() / (engine.GetMaxHandSize() * 2);

        return average;
    }

    private string GetPlacing()
    {
        return engine.GetPlacingByID(player.GetID());
    }
}
