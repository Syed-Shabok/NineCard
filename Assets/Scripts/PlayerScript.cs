using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private List<CardScript> mHandCards = new List<CardScript>(); // Holds rerance to player's card list 
    private List<Card> _cardList = new List<Card>(); // List of Card instances received from DecScript. 
    [SerializeField] private List<CardScript> mSetCards = new List<CardScript>();  // Holds Referance to "Card Set's" cards in the scene.  
    private List<List<CardScript>> _playerCardSets = new List<List<CardScript>>(); // List of Card Sets made by player.
    [SerializeField] private TableScript mTable; 
    private int _cardSetsPlaced = 0; // Number of card Sets places by a Player. 
    private int _leadsPlayed = 0; 
    private int _leadsWon = 0;
    [SerializeField] private GameObject mPointsBar; // Referance to player's points indicator in the scene. 
    [SerializeField] private HandRankChecker mHandRankChecker; // Reference to HandRankChecker, used for making player's card sets

    void Start()
    {
        mTable = GameObject.Find("Table").GetComponent<TableScript>();
        mHandRankChecker = GameObject.Find("Hand Rank Checker").GetComponent<HandRankChecker>();
    }

    // Adds a card to the players hand (i.e. the cardList list).
    public void AddCardToHand(Card card)
    {   
        _cardList.Add(card);

        // After players gets 9 cards, SetPlayerCards() runs.
        if(_cardList.Count == 9)
        {  
            SetPlayerCards();
        }
    }

    // Sets the fields of the cards in handCards list.
    private void SetPlayerCards()
    {
        //Debug.Log($"Setting {this.gameObject.name} cards. Total HandCards: {handCards.Count}");
    
        for (int i = 0; i < mHandCards.Count; ++i)
        {   
            //Debug.Log($"Attempting to set {this.gameObject.name}'s card-{i+1}.");
            mHandCards[i].SetCardScript(_cardList[i]);
            mHandCards[i].SetCardOwner(this);
        }
    }

    // Gets the Main Player's cards from the scene and sorts them into appropriate card sets. 
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

            _playerCardSets.Add(newSet);
        }

        GameJudge judge = GameObject.Find("Game Judge").GetComponent<GameJudge>();

        // Sorts Main Player's card sets based on HandRank.  
        judge.SortMainPlayerCards(_playerCardSets);

        UpdateCardSetsOnScreen();
        //PrintCardSets();
    }

    // Creates card sets for Bot Players.  
    public void GetPlayerCardSets()
    {
        Debug.Log($"Fetching {this.gameObject.name} card sets");

        // Gets appropriate card sets based on bot's cards. 
        List<List<CardScript>> tempCardSets = mHandRankChecker.GetCardSets(this, mHandCards);
        
        _playerCardSets = new List<List<CardScript>>();
        foreach (var set in tempCardSets)
        {
            _playerCardSets.Add(new List<CardScript>(set));
        }

        //PrintListOfCardSets();
        UpdateCardSetsOnScreen();
    }

    // Sorts Main Player's cards in Decending order.
    public void SortMainPlayerCardsList()
    {   
        Debug.Log("SortMainPlayerCardsList() has run");
 
        _cardList.Sort((card1, card2) => 
            GetRankValue(card2.GetCardRank()).CompareTo(GetRankValue(card1.GetCardRank())));

        SetPlayerCards();
    }

    // Sets the fields of the cards in setCards list.     
    private void UpdateCardSetsOnScreen()
    {
        int index = 0; 
        foreach(var set in _playerCardSets)
        {
            foreach(CardScript card in set)
            {
                card.SetCardOwner(this); // Also sets this player as the owner of this card.
                mSetCards[index].SetCardScript(card);
                ++index;
            }
        }
    }

    // Places the left-most (best) card set of the player. 
    public void PlaceBestCardSet(int playerTurn)
    {
        if(_cardSetsPlaced > 2)
        {
            Debug.Log("All turns complete.");
            return;
        }
        
        //Debug.Log($"Placing {this.gameObject.name} Card Set: {cardSetsPlaced}.");
        //PrintCardList(this.playerCardSets[cardSetsPlaced]);


        List<CardScript> currentCardSet = new List<CardScript>(this._playerCardSets[_cardSetsPlaced]);

        GameObject CardSetsGameObjects = this.transform.Find($"{this.gameObject.name} Sets").gameObject;
        Destroy(CardSetsGameObjects.transform.Find($"Set {_cardSetsPlaced + 1}").gameObject);

        // Removes the placed cards from the setCards list to maintain consistancy.
        for(int i = 0; i < 3; ++i)
        {
            mSetCards.RemoveAt(0);
        }
        
        ++_cardSetsPlaced;
        ++_leadsPlayed;
        
        // Places the Card Set on the Table. 
        mTable.PlacePlayerCardsOnTable(playerTurn, currentCardSet);
    }
    
    // Updates Player's Score.
    public void GivePointToPlayer()
    {
        Debug.Log($"{this.gameObject.name} has won this lead.");
        UpdatePointBar();
        ++_leadsWon;
    }

    // Shows player's score in the scene. 
    private void UpdatePointBar()
    {   
        GameObject pointIndicator = mPointsBar.transform.GetChild(_leadsPlayed - 1).gameObject;
        pointIndicator.GetComponent<Image>().color = Color.yellow;
    }

    // Returns the number of leads won by this player. 
    public int GetLeadsWon()
    {
        return _leadsWon;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private int GetRankValue(Rank rank)
    {
        return (int)rank;
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

        for(int i = 0; i < _playerCardSets.Count; ++i)
        {
            Debug.Log($"Set - {i}:");

            for(int j = 0; j < _playerCardSets[i].Count; ++j)
            {
                Debug.Log($"{_playerCardSets[i][j].GetCardRank()} of {_playerCardSets[i][j].GetCardSuit()}");
            }
        }
    }

}
