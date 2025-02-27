using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Suit {SPADES, HEARTS, CLUBS, DIAMONDS};
public enum Rank {TWO = 2, THREE, FOUR, FIVE, SIX, SEVEN, EIGHT, NINE, TEN, JACK, QUEEN, KING, ACE};

public class DeckScript : MonoBehaviour
{
    [SerializeField] private List<Card> mDeck = new List<Card>(); //List of all cards in the Deck.
    [SerializeField] private List<Sprite> mCardImages = new List<Sprite>(); //List of card images, assigned in the inspector.
    private List<PlayerScript> _playerList =  new List<PlayerScript>();
    private int _numberOfPlayers = 5;
    
    void Start()
    {
        InitializeDeck();
        ShuffleDeck();
    }

    // Populates the mDeck list with playing cards. 
    private void InitializeDeck()
    {
        Debug.Log("InitializedDeck() has run.");
        
        mDeck.Clear();

        int imageIndex = 0;

        foreach(Suit suitGroup in (Suit[])System.Enum.GetValues(typeof(Suit)))
        {
            foreach(Rank rankGroup in (Rank[])System.Enum.GetValues(typeof(Rank)))
            {
                Card newCard = new Card();
                newCard.SetCard(suitGroup, rankGroup, mCardImages[imageIndex]);
                mDeck.Add(newCard);

                ++imageIndex;
            }
        }
    }

    // Shuffles the deck of cards (i.e. the mDeck list).
    private void ShuffleDeck()
    {
        Debug.Log("ShuffleDeck() has run");
        
        for(int i = mDeck.Count - 1; i > 0; --i)
        {
            int randomIndex = Random.Range(0, i + 1);
            Card temp = mDeck[i];
            mDeck[i] = mDeck[randomIndex];
            mDeck[randomIndex] = temp;
        }
    }

    // Finds all current players of the game and puts them into the playerList.
    private void FindPlayers()
    {   
        _playerList.Add(GameObject.Find("Main Player").GetComponent<PlayerScript>());

        for(int i = 1; i < _numberOfPlayers; ++i)
        {
            _playerList.Add(GameObject.Find($"Player {i}").GetComponent<PlayerScript>());
        }
    }

    // Distributues cards amoung all current players of the game. 
    public void DistributeCards(int noOfOpponents)
    {   
        Debug.Log("DistributeCards() has run.");

        this._numberOfPlayers = noOfOpponents + 1;

        FindPlayers();

        int index = 0;
        for(int i = 0; i < _numberOfPlayers; ++i)
        {
            Debug.Log($"Giving cards to {_playerList[i].gameObject.name}:");

            // Each player gets 9 cards. 
            for(int j = 0; j < 9; ++j) 
            {
                _playerList[i].AddCardToHand(mDeck[index]);
                ++index;
            }
        }
    }
}

[System.Serializable] public class Card
{
    [SerializeField] private Suit _suit;
    [SerializeField] private Rank _rank;
    private Sprite _CardImage;

    // Sets all fields of the card based on given parameters. 
    public void SetCard(Suit suit, Rank rank, Sprite cardSprite)
    {
        this._suit = suit;
        this._rank = rank;
        this._CardImage = cardSprite;
    }
    
    // Returns Card's Suit. 
    public Suit GetCardSuit()
    {
        return this._suit;
    }

    // Returns Card's Rank. 
    public Rank GetCardRank()
    {
        return this._rank;
    }

    // Returns Card's Sprite Image. 
    public Sprite GetCardSprite()
    {
        return _CardImage;
    }
}