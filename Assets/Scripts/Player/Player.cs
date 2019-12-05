using System.Collections;
using System.Collections.Generic;

public class Player
{
    private static int ids = 0;

    private int id;
    private PlayerType type;

    private string name;
    private int score;

    private int currentBid;
    private List<Card> hand;
    private int currentTricks;

    private Brain brain = null;

    public Player (PlayerType type)
    {
        id = ids;
        ids++;

        score = 0;
        currentTricks = 0;

        hand = new List<Card>();

        this.type = type;
    }

    public void CreateBrain(Engine engine, FileManager fm)
    {
        brain = new Brain(engine, fm, this);
    }

    public void StartRoundAssessment()
    {
        brain.StartRoundAssessment();
    }

    public void EndRoundAssessment()
    {
        brain.EndRoundAssessment();
    }

    public void EndGameAssessment()
    {
        brain.EndGameAssessment();
    }

    public void SetName(string name)
    {
        this.name = name;
    }

    public string GetName()
    {
        return name;
    }

    public int GetID()
    {
        return id;
    }

    public int GetScore()
    {
        return score;
    }

    public void AddToScore(int score)
    {
        this.score += score;
    }

    public void SetBid(int bid)
    {
        currentBid = bid;
    }

    public int MakeRandomBid(int cardsThisRound, bool crystalBrook, int invalidBid)
    {
        int bid = brain.MakeBid(cardsThisRound, crystalBrook, invalidBid);
        return bid;
    }

    public int GetCurrentBid()
    {
        return currentBid;
    }

    public void AddCardToHand(Card card)
    {
        hand.Add(card);
    }

    public void PlayCard(Card card)
    {
        hand.Remove(card);
    }

    public Card ChooseCardToPlay()
    {
        Card play = brain.PlayRandomCard();
        PlayCard(play);
        return play;
    }

    public int GetNumCards()
    {
        return hand.Count;
    }

    public List<Card> GetHand()
    {
        return hand;
    }

    public void AddTrick()
    {
        currentTricks++;
    }

    public int GetTricks()
    {
        return currentTricks;
    }

    public PlayerType GetPlayerType()
    {
        return type;
    }

    public void SafeRoundReset()
    {
        currentBid = 0;
        currentTricks = 0;
        hand = new List<Card>();
    }

    public static void ResetIDCounter()
    {
        ids = 0;
    }
}
