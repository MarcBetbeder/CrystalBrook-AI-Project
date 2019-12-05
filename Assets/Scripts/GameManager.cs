using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    // Game Settings
    [SerializeField] private int numPlayers = 6;
    [SerializeField] private int maxHandSize = 8;
    [SerializeField] private int zeroValue = 7;
    [SerializeField] private GameMode mode;
    // First dealer: playerID of first dealer = firstDealer - 1. 0 -> Random choice.
    [SerializeField] private int firstDealer = 0;
    // Mode to determine production of AI hand/bid history.
    [SerializeField] private bool assessmentMode = false;

    // Game Variables
    private Engine engine;
    private DisplayManager dm;
    private GameWindow gw;
    private List<Player> players = new List<Player>();
    private FileManager fm;

    private int numRounds;
    private int currentRound = 1;
    private int cardsThisRound;
    private Player currentDealer;
    private Deck deck;
    private bool gameHasFinished = false;

    // Prefab References
    [SerializeField] private GameObject deckPrefab;
    [SerializeField] private GameObject emulatorEnginePrefab;
    [SerializeField] private GameObject spEnginePrefab;
    [SerializeField] private GameObject sAIEnginePrefab;
    [SerializeField] private GameObject simulationEnginePrefab;

    // Pre-Start method
    void Awake()
    {
        EnsureSingleManager();
    }

    // Start-up
    void Start()
    {
        fm = new FileManager();
        // Add Game Startup as a delegate to the sceneLoaded event.
        SceneManager.sceneLoaded += ExecuteGame;
        if (SceneManager.GetActiveScene().name == "Game Window")
        {
            CheckValidNumPlayers();
            CheckValidHandSize();
            SetUpPlayers(mode);

            for (int i = 0; i < numPlayers; i++)
            {
                SetPlayerName(i, "Player " + (i + 1).ToString());
            }

            PrepareNewGame();
        }
    }

    void OnDestroy()
    {
        // Remove Game Startup as a delegate to the sceneLoaded event.
        SceneManager.sceneLoaded -= ExecuteGame;
    }

    // Getters & Setters
    public int GetNumPlayers()
    {
        return numPlayers;
    }

    public DisplayManager GetDisplayManager()
    {
        return dm;
    }

    public void SetFirstDealer(int firstDealer)
    {
        this.firstDealer = firstDealer;
    }
    // End Getters & Setters

    public void SafeGameAbort()
    {
        Player.ResetIDCounter();
        Destroy(gameObject);
    }

    public void ApplyGameSetup(int numPlayers, int maxHandSize, int zeroValue, GameMode mode)
    {
        this.numPlayers = numPlayers;
        this.maxHandSize = maxHandSize;
        this.zeroValue = zeroValue;
        this.mode = mode;

        CheckValidNumPlayers();
        CheckValidHandSize();

        SetUpPlayers(mode);
    }

    private void SetUpPlayers(GameMode mode)
    {
        switch (mode)
        {
            case GameMode.EMULATOR:
                for (int i = 0; i < numPlayers; i++)
                {
                    if (i == 0)
                    {
                        players.Add(new Player(PlayerType.MAIN_HUMAN));
                    }
                    else
                    {
                        players.Add(new Player(PlayerType.HUMAN));
                    }
                }
                break;
            case GameMode.SINGLE_PLAYER:
                for (int i = 0; i < numPlayers; i++)
                {
                    if (i == 0)
                    {
                        players.Add(new Player(PlayerType.MAIN_HUMAN));
                    }
                    else
                    {
                        players.Add(new Player(PlayerType.AI));
                    }
                }
                break;
            case GameMode.SINGLE_AI:
                for (int i = 0; i < numPlayers; i++)
                {
                    if (i == 0)
                    {
                        players.Add(new Player(PlayerType.MAIN_AI));
                    }
                    else
                    {
                        players.Add(new Player(PlayerType.HUMAN));
                    }
                }
                break;
            case GameMode.SIMULATOR:
                for (int i = 0; i < numPlayers; i++)
                {
                    if (i == 0)
                    {
                        players.Add(new Player(PlayerType.MAIN_AI));
                    }
                    else
                    {
                        players.Add(new Player(PlayerType.AI));
                    }
                }
                break;
            default:
                break;
        }
    }

    public void SetPlayerName(int id, string name)
    {
        FindPlayerByID(id).SetName(name);
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Game Window");
    }

    /*  Awake method to make sure we only ever have a single Game Manager.
        Also ensures the initial Game Manager persists through scene loads. */
    private void EnsureSingleManager()
    {
        int numGameManagers = FindObjectsOfType<GameManager>().Length;
        if (numGameManagers > 1)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start-up method to validate number of players. Keeps numPlayers between 4 and 10.
    private void CheckValidNumPlayers()
    {
        numPlayers = Mathf.Clamp(numPlayers, 4, 10);
    }

    // Start-up method to validate hand size setting.
    private void CheckValidHandSize()
    {
        bool check = numPlayers * maxHandSize < 52;
        if (!check)
        {
            maxHandSize = 51 / numPlayers;
        }
        if (maxHandSize > 10)
        {
            maxHandSize = 10;
        }

        numRounds = maxHandSize * 2;
    }

    // Method to actually begin the game when the user starts the game.
    private void ExecuteGame(Scene scene, LoadSceneMode sceneMode)
    {
        // Only run code if we are loading the Game Window.
        if (scene.name == "Game Window")
        {
            PrepareNewGame();
        }
    }

    private void PrepareNewGame()
    {
        gameHasFinished = false;

        gw = FindObjectOfType<GameWindow>();
        gw.InitialiseScoreBoard(players);
        gw.UpdateScoreBoard(GetScores(), GetDelta(), GetWinningAverage());

        dm = FindObjectOfType<DisplayManager>();
        dm.Initialise();
        dm.InitialisePlayerDisplays(players);

        SetUpEngine();

        currentDealer = SelectFirstDealer();
    }

    public void NewRound()
    {
        if (currentRound > numRounds)
        {
            FinishGame();
        } else
        {
            dm.ResetDisplays();

            dm.SetDealer(currentDealer.GetID());
            CalculateCardsThisRound();
            gw.UpdateRoundInfo(currentRound, cardsThisRound);
            deck = Instantiate(deckPrefab).GetComponent<Deck>();

            engine.PrepareRound(cardsThisRound, currentDealer, deck);
        }
    }

    public void FinaliseRound()
    {
        ResetCurrentRound();

        currentDealer = GetNextPlayer(currentDealer.GetID());

        gw.UpdateScoreBoard(GetScores(), GetDelta(), GetWinningAverage());
        gw.SetWinners(FindWinners());
        gw.ShowScoreBoard();

        currentRound++;
        Destroy(deck.gameObject);
    }

    public void ContinueGame()
    {
        gw.ContinueGame();
    }

    public void ResetCurrentRound()
    {
        engine.SafeRoundReset();
        dm.SafeRoundReset();

        foreach (Player p in players)
        {
            p.SafeRoundReset();
        }
    }

    public bool GameHasFinished()
    {
        return gameHasFinished;
    }

    private void FinishGame()
    {
        gameHasFinished = true;
        Debug.Log("Game Finished!");
        dm.ResetDisplays();

        EndGameAssessment();

        gw.ShowScoreBoard();
    }

    public List<int> GetScores()
    {
        List<int> scores = new List<int>();

        for (int i = 0; i < numPlayers; i++)
        {
            scores.Insert(i, FindPlayerByID(i).GetScore());
        }

        return scores;
    }

    public Player FindPlayerByID(int id)
    {
        Player player = null;

        foreach (Player p in players)
        {
            if (p.GetID() == id)
            {
                player = p;
                break;
            }
        }

        return player;
    }

    public Player GetNextPlayer(int id)
    {
        id++;

        if (id == numPlayers)
        {
            id = 0;
        }

        return FindPlayerByID(id);
    }

    public bool IsHeadRound()
    {
        if (currentRound == numRounds / 2)
        {
            return true;
        } else
        {
            return false;
        }
    }

    public void DealFullRound()
    {
        for (int i = 0; i < cardsThisRound; i++)
        {
            foreach (Player p in players)
            {
                p.AddCardToHand(deck.DrawRandomCard());
            }
        }
    }

    public void CollectCardInput(string infoText)
    {
        gw.InputCard(infoText);
    }

    public void SetCurrentCard(string id)
    {
        Card card = deck.DrawSpecificCard(id);
        engine.SetCurrentCard(card);
        engine.RecievedInfo(true);
    }

    public Card AddCardToMainHandDisplay(Card card)
    {
        return dm.AddCardToPlayerHand(card);
    }

    public void DisplayHeadCard(Card card, Player player)
    {
        dm.SetPlayerHeadCard(player.GetID(), card);
    }

    public void DisplayTrumpCard(Card card)
    {
        dm.SetTrumpCard(card);
    }

    public void CollectPlayerBid(Player player, bool crystalBrook, int invalidBid)
    {
        gw.InputBid(player.GetName(), crystalBrook, invalidBid);
    }

    public void SetCurrentBid(int bid)
    {
        if (CheckValidBid(bid))
        {
            engine.SetCurrentBid(bid);
        } else
        {
            engine.SetCurrentBid(-1);
        }
        engine.RecievedInfo(true);        
    }

    public void DisplayPlayerBid(Player player)
    {
        dm.SetPlayerDisplayBid(player.GetID(), player.GetCurrentBid());
    }

    public void PlayPhaseDisplay()
    {
        dm.PlayPhase();
    }

    public void ConstrainMainPlayerHandDisplay(Trick trick)
    {
        if (trick.GetLeadSuit() == Suit.NONE)
        {
            dm.SetHandInteractable(true);
        } else
        {
            dm.ConstrainPlayerHand(trick.GetLeadSuit());
        }
    }

    public void PlayMainPlayerCard(Card card)
    {
        dm.PlayMainPlayerCard(card.gameObject);
        engine.SetCurrentCard(card);
        engine.RecievedInfo(true);
    }

    public void DisplayPlayedCard(Card card, Player player)
    {
        dm.CreateCardAtPlayer(card, player);
    }

    public Card GetHeadCard(Player player)
    {
        List<Card> hand = player.GetHand();
        Card headCard = null;
        if (hand.Count == 1)
        {
            headCard = hand[0];
        }

        return headCard;
    }

    public void PlayHeadCard(Player player)
    {
        dm.PlayHeadCard(player);
    }

    public IEnumerator WaitForMovingCards()
    {
        while (dm.IsMovingCards())
        {
            yield return null;
        }
    }

    public IEnumerator AwardTrick(Player player)
    {
        player.AddTrick();
        dm.UpdatePlayerTricks(player);

        StartCoroutine(dm.CleanUpTrick(player));

        yield return WaitForMovingCards();
    }

    public void ScoreRound()
    {
        foreach (Player player in players)
        {
            AwardPoints(player);
        }
    }

    private void AwardPoints(Player player)
    {
        int bid = player.GetCurrentBid();
        int tricks = player.GetTricks();
        int score = 0;

        if (bid == tricks)
        {
            if (bid == 0)
            {
                score = zeroValue;
            } else
            {
                score = 10;
            }
        }

        score += tricks;

        player.AddToScore(score);
    }

    private int GetDelta()
    {
        return GetHighestScore() - GetLowestScore();
    }

    private float GetWinningAverage()
    {
        float highest = GetHighestScore();

        return highest / currentRound;
    }

    private int GetHighestScore()
    {
        List<int> scores = GetScores();

        int highest = 0;

        foreach (int score in scores)
        {
            if (score > highest)
            {
                highest = score;
            }
        }

        return highest;
    }

    private int GetLowestScore()
    {
        List<int> scores = GetScores();

        int lowest = int.MaxValue;

        foreach (int score in scores)
        {
            if (score < lowest)
            {
                lowest = score;
            }
        }

        return lowest;
    }

    private List<Player> FindWinners()
    {
        List<Player> winners = new List<Player>();
        int winningScore = GetHighestScore();

        foreach (Player p in players)
        {
            if (p.GetScore() == winningScore)
            {
                winners.Add(p);
            }
        }

        return winners;
    }

    public string[] DeterminePlacings()
    {
        List<Player> sortedPlayers = new List<Player>();
        List<Player> unsortedPlayers = new List<Player>();

        foreach (Player p in players)
        {
            unsortedPlayers.Add(p);
        }

        int count = unsortedPlayers.Count;

        string[] placings = new string[count];
        int numSorted = 0;
        string placing = "1st";

        while (sortedPlayers.Count < count)
        {
            int topScore = 0;
            foreach (Player p in unsortedPlayers)
            {
                if (p.GetScore() > topScore)
                {
                    topScore = p.GetScore();
                }
            }

            bool check = false;

            while (!check)
            {
                check = true;
                Player temp = null;
                foreach (Player p in unsortedPlayers)
                {
                    if (p.GetScore() == topScore)
                    {
                        sortedPlayers.Add(p);
                        numSorted++;
                        placings[p.GetID()] = placing;
                        check = false;
                        temp = p;
                        break;
                    }
                }

                unsortedPlayers.Remove(temp);
            }

            switch (numSorted)
            {
                case 0:
                    placing = "1st";
                    break;
                case 1:
                    placing = "2nd";
                    break;
                case 2:
                    placing = "3rd";
                    break;
                case 3:
                    placing = "4th";
                    break;
                case 4:
                    placing = "5th";
                    break;
                case 5:
                    placing = "6th";
                    break;
                case 6:
                    placing = "7th";
                    break;
                case 7:
                    placing = "8th";
                    break;
                case 8:
                    placing = "9th";
                    break;
                case 9:
                    placing = "10th";
                    break;
                default:
                    placing = "Other";
                    break;
            }
        }

        return placings;
    }

    private Player SelectFirstDealer()
    {
        if (firstDealer == 0)
        {
            return SelectRandomPlayer();
        } else
        {
            return FindPlayerByID(firstDealer - 1);
        }
    }

    private Player SelectRandomPlayer()
    {
        int index = Random.Range(0, players.Count);
        return players[index];
    }

    private void CalculateCardsThisRound()
    {
        if (currentRound > maxHandSize)
        {
            cardsThisRound = currentRound - maxHandSize;
        } else
        {
            cardsThisRound = maxHandSize - (currentRound - 1);
        }
    }

    private void SetUpEngine()
    {
        GameObject prefab;

        switch (mode)
        {
            case GameMode.EMULATOR:
                prefab = emulatorEnginePrefab;
                engine = Instantiate(prefab).GetComponent<EmulatorEngine>();
                break;
            case GameMode.SINGLE_PLAYER:
                prefab = spEnginePrefab;
                engine = Instantiate(prefab).GetComponent<Engine>();
                break;
            case GameMode.SINGLE_AI:
                prefab = sAIEnginePrefab;
                engine = Instantiate(prefab).GetComponent<Engine>();
                break;
            default:
                prefab = simulationEnginePrefab;
                engine = Instantiate(prefab).GetComponent<Engine>();
                break;
        }

        engine.Initialise(this, numPlayers, maxHandSize, zeroValue);

        AssignBrains();
    }

    private void AssignBrains()
    {
        foreach (Player p in players)
        {
            if (p.GetPlayerType() == PlayerType.AI || p.GetPlayerType() == PlayerType.MAIN_AI)
            {
                p.CreateBrain(engine, fm);
            }
        }
    }

    private bool CheckValidBid(int bid)
    {
        if (bid == engine.GetInvalidBid() && engine.IsCrystalBrook())
        {
            return false;
        }

        if (bid < 0)
        {
            return false;
        } else if (bid > cardsThisRound)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public List<Card> GetVisibleCards(int id)
    {
        List<Card> cards = new List<Card>();

        foreach (Player p in players)
        {
            if (id != p.GetID())
            {
                foreach (Card c in p.GetHand())
                {
                    cards.Add(c);
                }
            }
        }

        return cards;
    }

    public void StartRoundAssessment()
    {
        foreach (Player p in players)
        {
            if (p.GetPlayerType() == PlayerType.AI || p.GetPlayerType() == PlayerType.MAIN_AI)
            {
                p.StartRoundAssessment();
            }
        }
    }

    public void EndRoundAssessment()
    {
        foreach (Player p in players)
        {
            if (p.GetPlayerType() == PlayerType.AI || p.GetPlayerType() == PlayerType.MAIN_AI)
            {
                p.EndRoundAssessment();
            }
        }
    }

    public void EndGameAssessment()
    {
        foreach (Player p in players)
        {
            if (p.GetPlayerType() == PlayerType.AI || p.GetPlayerType() == PlayerType.MAIN_AI)
            {
                p.EndGameAssessment();
            }
        }
    }
}
