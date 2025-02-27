using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableScript : MonoBehaviour
{   
    private List<List<CardScript>> _cardSetsOnTable = new List<List<CardScript>>();
    private List<GameObject> _cardPlacementAreas = new List<GameObject>();
    [SerializeField] private GameManager mGameManager;
    [SerializeField] private GameJudge mGameJudge;

    // Gets the card placement areas for each player. 
    private void FindPlacementAreas(int noOfPlayers)
    {   
        _cardPlacementAreas.Add(GameObject.Find("Main Player Playing Location"));

        for(int i = 1; i < noOfPlayers; ++i)
        {
            _cardPlacementAreas.Add(GameObject.Find($"Player {i} Playing Location"));
        }
    }

    // Based on the number of players, this sets up the card placement areas for all players.
    public void SetUpCardPlacementAreas(int noOfPlayers)
    {
        FindPlacementAreas(noOfPlayers);
    }

    // Places a player's card sets in that player's card placement location. 
    public void PlacePlayerCardsOnTable(int playerTurn, List<CardScript> cardSet)
    {   
        _cardSetsOnTable.Add(new List<CardScript>(cardSet));
        
        for(int i = 0; i < cardSet.Count; ++i)
        {
            GameObject newCard = Instantiate(cardSet[i].gameObject, _cardPlacementAreas[playerTurn].transform);
        }

        // Check if round should continue
        if (mGameManager.IsRoundOngoing()) 
        {
            mGameManager.NextTurn();
        }
    }

    // Function that Checks which player has won a round. 
    public void FindLeadWinner()
    {   
        Debug.Log("FindLeadWinner() has run.");
        Debug.Log("Card Sets on Table:");
        PrintCardSetsOnTable();

        // Checks all card sets on table and decides the winning card set.
        List<CardScript> winningCardSet = mGameJudge.FindWinner(new List<List<CardScript>>(_cardSetsOnTable));

        ClearTable();

        Debug.Log($"Winning Player: {winningCardSet[0].GetCardOwner().gameObject.name}");

        int winningPlayerIndex = mGameManager.GetPlayerIndex(winningCardSet[0].GetCardOwner());

        mGameManager.NextTurn(winningPlayerIndex);
    }
    
    // Removes all card sets from the table. 
    public void ClearTable()
    {
        Debug.Log("ClearTable() has started.");

        foreach (GameObject area in _cardPlacementAreas)
        {
            int childCount = area.transform.childCount;
            for (int i = childCount - 1; i >= 0; --i) 
            {
                DestroyImmediate(area.transform.GetChild(i).gameObject);
            }
        }

        _cardSetsOnTable.Clear();

        Debug.Log("ClearTable() has run.");
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    

    private void PrintCardSetsOnTable()
    {   
        Debug.Log($"{this.gameObject.name} card sets are:");

        for(int i = 0; i < _cardSetsOnTable.Count; ++i)
        {
            Debug.Log($"Set - {i}:");

            for(int j = 0; j < _cardSetsOnTable[i].Count; ++j)
            {
                Debug.Log($"{_cardSetsOnTable[i][j].GetCardRank()} of {_cardSetsOnTable[i][j].GetCardSuit()}");
            }
        }
    }

}
