using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayManager : MonoBehaviour
{
    [SerializeField] private List<PlayerPositions> playerPositions;
    [SerializeField] private List<Sprite> playerSprites;

    [SerializeField] private Transform playerHandLocation;
    [SerializeField] private float playerHandSpread = 0.8f;
    [SerializeField] private Transform trumpCardLocation;
    [SerializeField] private float trumpCardScale = 0.8f;
    [SerializeField] private float playedCardsScale = 0.7f;

    [SerializeField] private float playedCardsMoveSpeed = 5f;

    private List<PlayerDisplay> playerDisplays = new List<PlayerDisplay>();
    private List<GameObject> mainPlayerHand = new List<GameObject>();
    private GameObject trumpCard = null;
    private List<GameObject> playedCards = new List<GameObject>();
    private GameObject cardBack = null;

    private int numPlayers;

    private GameManager gm;

    [SerializeField] private GameObject playerDisplayPrefab;
    [SerializeField] private GameObject cardBackPrefab;

    public void Initialise()
    {
        gm = FindObjectOfType<GameManager>();
        numPlayers = gm.GetNumPlayers();
    }

    public bool IsMovingCards()
    {
        bool moving = false;
        foreach (GameObject card in playedCards)
        {
            if (card.GetComponent<Card>().IsMoving())
            {
                moving = true;
            }
        }

        if (cardBack != null)
        {
            if (cardBack.GetComponent<Card>().IsMoving())
            {
                moving = true;
            }
        }
        return moving;
    }

    public void InitialisePlayerDisplays(List<Player> players)
    {
        string[] actualPPs = FindCorrectPlayerPositions();

        if (actualPPs != null)
        {
            Player current;
            for (int i = 0; i < actualPPs.Length; i++)
            {
                current = null;
                foreach (Player p in players)
                {
                    if (p.GetID() == i)
                    {
                        current = p;
                        break;
                    }
                }
                if (current != null)
                {
                    PlayerDisplay pDisplay = Instantiate(playerDisplayPrefab).GetComponent<PlayerDisplay>();
                    pDisplay.SetID(current.GetID());
                    pDisplay.SetLocation(FindPlayerPositionTransform(actualPPs[i], "Player Positions"));
                    pDisplay.SetCardDestination(FindPlayerPositionTransform(actualPPs[i], "Played Card Positions"));
                    pDisplay.SetTextReferences(FindPlayerUIReference(actualPPs[i]));
                    pDisplay.SetName(current.GetName());
                    pDisplay.SetSprite(ChooseRandomSprite());
                    playerDisplays.Add(pDisplay);
                }
            }
            ResetDisplays();
        }
    }

    public void SafeRoundReset()
    {
        RemoveTrumpCard();
        DestroyPlayedCards();
        DestroyPlayerHand();
        WipeHeadCards();
    }

    public void PlayPhase()
    {
        foreach (PlayerDisplay pd in playerDisplays)
        {
            pd.SetTricks("0");
        }
    }

    public void SetPlayerDisplayBid(int id, int bid)
    {
        FindPlayerDisplayByID(id).SetBid(bid.ToString());
    }

    public void SetPlayerHeadCard(int id, Card card)
    {
        PlayerDisplay target = FindPlayerDisplayByID(id);
        Vector3 targetPos = target.transform.position;
        targetPos.y += 0.5f;
        targetPos.z -= 0.5f;
        GameObject headCard = Instantiate(card, targetPos, Quaternion.identity).gameObject;
        headCard.GetComponent<Card>().Initialise();
        headCard.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        target.SetHeadCard(headCard);
    }

    public void WipeBids()
    {
        foreach (PlayerDisplay pd in playerDisplays)
        {
            pd.WipeBid();
        }
    }

    public void WipeTricks()
    {
        foreach (PlayerDisplay pd in playerDisplays)
        {
            pd.WipeTricks();
        }
    }

    public void ResetNames()
    {
        foreach (PlayerDisplay pd in playerDisplays)
        {
            pd.ResetNameText();
        }
    }

    public void WipeHeadCards()
    {
        foreach (PlayerDisplay pd in playerDisplays)
        {
            pd.WipeHeadCard();
        }
    }

    public void SetDealer(int id)
    {
        FindPlayerDisplayByID(id).SetDealer();
    }

    public void SetCardMoveSpeed(float speed)
    {
        playedCardsMoveSpeed = speed;
    }

    public void UpdatePlayerTricks(Player player)
    {
        string tricks = player.GetTricks().ToString();

        FindPlayerDisplayByID(player.GetID()).SetTricks(tricks);
    }

    public void ResetDisplays()
    {
        WipeTricks();
        WipeBids();
        ResetNames();
        WipeHeadCards();
    }

    public Card AddCardToPlayerHand(Card card)
    {
        Vector3 cardLocation = playerHandLocation.position;
        GameObject newCard = Instantiate(card, cardLocation, Quaternion.identity).gameObject;
        newCard.GetComponent<Card>().Initialise();
        mainPlayerHand.Add(newCard);
        UpdateMainPlayerHand();
        return newCard.GetComponent<Card>();
    }

    public void UpdateMainPlayerHand()
    {
        SortMainPlayerHand();
        Vector3 centre = playerHandLocation.position;
        Vector3 firstCardLocation = centre;
        if (mainPlayerHand.Count % 2 == 0)
        {
            firstCardLocation.x -= playerHandSpread / 2f;
        }
        firstCardLocation.x += playerHandSpread * Mathf.FloorToInt(mainPlayerHand.Count / 2);
        Vector3 cardPosition = firstCardLocation;

        for (int i = 0; i < mainPlayerHand.Count; i++)
        {
            mainPlayerHand[i].transform.position = cardPosition;
            cardPosition.x -= playerHandSpread;
            cardPosition.z += 1;
        }
    }

    public void RemoveCardFromPlayerHand(GameObject card)
    {
        mainPlayerHand.Remove(card);
        UpdateMainPlayerHand();
    }

    public void SetHandInteractable(bool tf)
    {
        foreach (GameObject card in mainPlayerHand)
        {
            card.GetComponent<Card>().SetColliderActive(tf);
        }
    }

    public void ConstrainPlayerHand(Suit suit)
    {
        int count = 0;

        foreach (GameObject card in mainPlayerHand)
        {

            if (card.GetComponent<Card>().GetSuit() == suit)
            {
                card.GetComponent<Card>().SetColliderActive(true);
                count++;
            } else
            {
                card.GetComponent<Card>().HighlightCard(true);
            }
        }

        if (count == 0)
        {
            SetHandInteractable(true);
        }
    }

    public void SetTrumpCard(Card card)
    {
        Vector3 cardLocation = trumpCardLocation.position;
        GameObject newCard = Instantiate(card, cardLocation, Quaternion.identity).gameObject;
        newCard.transform.localScale = new Vector3(trumpCardScale, trumpCardScale, 1f);
        newCard.GetComponent<Card>().Initialise();
        trumpCard = newCard;
    }

    public void RemoveTrumpCard()
    {
        Destroy(trumpCard);
        trumpCard = null;
    }

    public void CreateCardAtPlayer(Card card, Player player)
    {
        PlayerDisplay target = FindPlayerDisplayByID(player.GetID());
        Vector3 cardLocation = target.transform.position;
        cardLocation.z -= 3;

        GameObject newCard = Instantiate(card, cardLocation, Quaternion.identity).gameObject;
        newCard.transform.localScale = new Vector3(playedCardsScale, playedCardsScale, 1f);
        newCard.GetComponent<Card>().Initialise();
        playedCards.Add(newCard);

        StartCoroutine(MovePlayedCard(newCard, target.GetCardDestination().position));
    }

    public void PlayMainPlayerCard(GameObject card)
    {
        RemoveCardFromPlayerHand(card);
        playedCards.Add(card);

        card.transform.localScale = new Vector3(playedCardsScale, playedCardsScale, 1f);

        StartCoroutine(MovePlayedCard(card, FindPlayerDisplayByID(0).GetCardDestination().position));
        
        foreach (GameObject c in mainPlayerHand)
        {
            c.GetComponent<Card>().HighlightCard(false);
            c.GetComponent<Card>().SetColliderActive(false);
        }
    }

    public void PlayHeadCard(Player player)
    {
        PlayerDisplay target = FindPlayerDisplayByID(player.GetID());
        GameObject card = target.GetHeadCard();
        target.SetHeadCard(null);
        playedCards.Add(card);
        card.transform.localScale = new Vector3(playedCardsScale, playedCardsScale, 1f);

        StartCoroutine(MovePlayedCard(card, target.GetCardDestination().position));
    }

    public IEnumerator CleanUpTrick(Player winningPlayer)
    {
        Vector3 centre = new Vector3(0f, 0f, -5f);

        foreach (GameObject card in playedCards)
        {
            StartCoroutine(MovePlayedCard(card, centre));
        }

        yield return gm.WaitForMovingCards();

        DestroyPlayedCards();

        cardBack = Instantiate(cardBackPrefab);
        cardBack.transform.position = centre;

        StartCoroutine(MovePlayedCard(cardBack, FindPlayerDisplayByID(winningPlayer.GetID()).transform.position));

        yield return gm.WaitForMovingCards();

        Destroy(cardBack);
        cardBack = null;
    }

    private IEnumerator MovePlayedCard(GameObject card, Vector3 destination)
    {
        card.GetComponent<Card>().SetMoving(true);
        while (card.transform.position != destination)
        {
            float movementThisFrame = playedCardsMoveSpeed * Time.deltaTime;
            card.transform.position = Vector3.MoveTowards(
                card.transform.position, destination, movementThisFrame);

            yield return null;
        }
        card.GetComponent<Card>().SetMoving(false);
    }

    private string[] FindCorrectPlayerPositions()
    {
        string[] retPos = null;

        foreach (PlayerPositions pp in playerPositions)
        {
            if (pp.GetNumPositions() == numPlayers)
            {
                retPos = pp.GetPositions();
                break;
            }
        }

        return retPos;
    }

    private Transform FindPlayerPositionTransform(string target, string positionType)
    {
        Transform ret = null;

        ret = GameObject.Find(positionType).transform.Find(target);

        return ret;
    }

    private Transform FindPlayerUIReference(string target)
    {
        Transform ret = null;

        ret = FindObjectOfType<Canvas>().transform.Find("Player UIs").Find(target);

        return ret;
    }

    private Sprite ChooseRandomSprite()
    {
        int index = Random.Range(0, playerSprites.Count);
        return playerSprites[index];
    }

    private PlayerDisplay FindPlayerDisplayByID(int id)
    {
        PlayerDisplay playerDisplay = null;

        foreach (PlayerDisplay pd in playerDisplays)
        {
            if (pd.GetID() == id)
            {
                playerDisplay = pd;
                break;
            }
        }

        return playerDisplay;
    }

    private void SortMainPlayerHand()
    {
        List<Card> handCards = new List<Card>();
        
        foreach (GameObject g in mainPlayerHand)
        {
            handCards.Add(g.GetComponent<Card>());
        }

        handCards.Sort();

        List<GameObject> sortedHand = new List<GameObject>();

        foreach (Card c in handCards)
        {
            sortedHand.Add(c.gameObject);
        }

        mainPlayerHand = sortedHand;
    }

    private void DestroyPlayedCards()
    {
        foreach (GameObject card in playedCards)
        {
            Destroy(card);
        }

        playedCards = new List<GameObject>();
    }

    private void DestroyPlayerHand()
    {
        foreach (GameObject card in mainPlayerHand)
        {
            Destroy(card);
        }

        mainPlayerHand = new List<GameObject>();
        UpdateMainPlayerHand();
    }
}
