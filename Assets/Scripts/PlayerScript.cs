using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private List<CardScript> handCards = new List<CardScript>();
    private List<Card> cardList = new List<Card>();
    [SerializeField] private List<CardScript> setCards = new List<CardScript>();  // Holds Referance to the on screen Card Set Objects. 
    private List<List<CardScript>> playerCardSets = new List<List<CardScript>>();
    private Dictionary<int, List<CardScript>> cardSetDictionary = new Dictionary<int, List<CardScript>>();
    [SerializeField] private TableScript table;
    private int cardSetsPlaced = 0;
    private int leadsPlayed = 0;
    [SerializeField] private int leadsWon = 0;
    [SerializeField] private GameObject pointsBar; 
    [SerializeField] private HandRankChecker handRankChecker; 

    void Start()
    {
        table = GameObject.Find("Table").GetComponent<TableScript>();
        handRankChecker = GameObject.Find("Hand Rank Checker").GetComponent<HandRankChecker>();
    }

    // Adds a card to the players hand (i.e. the cardList list).
    public void AddCardToHand(Card card)
    {   
        cardList.Add(card);

        // After players gets 9 cards, SetPlayerCards() runs.
        if(cardList.Count == 9)
        {  
            SetPlayerCards();
        }
    }

    // Sets the fields of the cards in handCards list.
    private void SetPlayerCards()
    {
        //Debug.Log($"Setting {this.gameObject.name} cards. Total HandCards: {handCards.Count}");
    
        for (int i = 0; i < handCards.Count; ++i)
        {   
            //Debug.Log($"Attempting to set {this.gameObject.name}'s card-{i+1}.");
            handCards[i].SetCardScript(cardList[i]);
            handCards[i].SetCardOwner(this);
        }
    }

    public void GetMainPlayerCardSets()
    {
        int index = 0;
        for(int i = 0; i < 3; ++i)
        {
            List<CardScript> newSet = new List<CardScript>();
            
            for(int j = 0; j < 3; ++j)
            {
                
                GameObject playerCards = this.gameObject.transform.Find($"{this.gameObject.name} Cards").gameObject;
                newSet.Add(playerCards.transform.GetChild(index).GetComponent<CardScript>());

                ++index;
            }

            playerCardSets.Add(newSet);
        }

        GameJudge judge = GameObject.Find("Game Judge").GetComponent<GameJudge>();

        judge.SortMainPlayerCards(playerCardSets);

        UpdateCardSetsOnScreen();
        //PrintCardSets();
    }


    public void GetPlayerCardSets()
    {
        Debug.Log($"Fetching {this.gameObject.name} card sets");

        List<List<CardScript>> tempCardSets = handRankChecker.GetCardSets(this, handCards);
        
        playerCardSets = new List<List<CardScript>>();
        foreach (var set in tempCardSets)
        {
            playerCardSets.Add(new List<CardScript>(set));
        }

        //PrintListOfCardSets();
        UpdateCardSetsOnScreen();
    }

    public void SortMainPlayerCardsList()
    {   
        Debug.Log("SortMainPlayerCardsList() has run");
 
        cardList.Sort((card1, card2) => 
            GetRankValue(card2.GetCardRank()).CompareTo(GetRankValue(card1.GetCardRank())));

        SetPlayerCards();
    }

    // Sets the fields of the cards in setCards list.   
    private void UpdateCardSetsOnScreen()
    {
        int index = 0; 
        foreach(var set in playerCardSets)
        {
            foreach(CardScript card in set)
            {
                card.SetCardOwner(this); // Also sets this player as the owner of this card.
                setCards[index].SetCardScript(card);
                ++index;
            }
        }
    }


    // Places the left-most card set of the player. 
    public void PlaceBestCardSet(int playerTurn)
    {
        if(cardSetsPlaced > 2)
        {
            Debug.Log("All turns complete.");
            return;
        }
        
        //Debug.Log($"Placing {this.gameObject.name} Card Set: {cardSetsPlaced}.");
        //PrintCardList(this.playerCardSets[cardSetsPlaced]);


        List<CardScript> currentCardSet = new List<CardScript>(this.playerCardSets[cardSetsPlaced]);

        GameObject CardSetsGameObjects = this.transform.Find($"{this.gameObject.name} Sets").gameObject;
        Destroy(CardSetsGameObjects.transform.Find($"Set {cardSetsPlaced + 1}").gameObject);

        // Removes the placed cards from the setCards list to maintain consistancy.
        for(int i = 0; i < 3; ++i)
        {
            setCards.RemoveAt(0);
        }
        
        ++cardSetsPlaced;
        ++leadsPlayed;
        
        table.PlacePlayerCardsOnTable(playerTurn, currentCardSet);
    }

    public void RemoveCardsFromPlayer(List<CardScript> cardSet)
    {
        foreach(CardScript card in cardSet)
        {
            handCards.Remove(card);
        }
    }

    public void GivePointToPlayer()
    {
        Debug.Log($"{this.gameObject.name} has won this lead.");
        UpdatePointBar();
        ++leadsWon;
    }

    // Number of leads played not updated correctly, must look into it.
    private void UpdatePointBar()
    {   
        GameObject pointIndicator = pointsBar.transform.GetChild(leadsPlayed - 1).gameObject;
        pointIndicator.GetComponent<Image>().color = Color.yellow;
    }

    private int GetRankValue(Rank rank)
    {
        return (int)rank;
    }

    public int GetLeadsWon()
    {
        return leadsWon;
    }

    private void PrintCardList(List<CardScript> cardList)
    {
        foreach(CardScript card in cardList)
        {
            Debug.Log($"{card.GetCardRank()} of {card.GetCardSuit()}");
        }
    }

    private void PrintListOfCardSets()
    {   
        Debug.Log($"{this.gameObject.name} card sets are:");

        for(int i = 0; i < playerCardSets.Count; ++i)
        {
            Debug.Log($"Set - {i}:");

            for(int j = 0; j < playerCardSets[i].Count; ++j)
            {
                Debug.Log($"{playerCardSets[i][j].GetCardRank()} of {playerCardSets[i][j].GetCardSuit()}");
            }
        }
    }

}
