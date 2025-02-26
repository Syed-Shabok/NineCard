using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TableScript : MonoBehaviour
{   
    private List<List<CardScript>> cardSetsOnTable = new List<List<CardScript>>();
    [SerializeField] private List<GameObject> cardPlacementAreas = new List<GameObject>();
    [SerializeField] GameManager gameManager;
    [SerializeField] GameJudge gameJudge;

    private void FindPlacementAreas(int noOfPlayers)
    {   
        cardPlacementAreas.Add(GameObject.Find("Main Player Playing Location"));

        for(int i = 1; i < noOfPlayers; ++i)
        {
            cardPlacementAreas.Add(GameObject.Find($"Player {i} Playing Location"));
        }
    }

    public void SetUpCardPlacementAreas(int noOfPlayers)
    {
        FindPlacementAreas(noOfPlayers);
    }

    public void PlacePlayerCardsOnTable(int playerTurn, List<CardScript> cardSet)
    {   
        cardSetsOnTable.Add(new List<CardScript>(cardSet));
        
        for(int i = 0; i < cardSet.Count; ++i)
        {
            GameObject newCard = Instantiate(cardSet[i].gameObject, cardPlacementAreas[playerTurn].transform);
        }

        // Check if round should continue
        if (gameManager.IsRoundOngoing()) 
        {
            gameManager.NextTurn();
        }
    }

    // Function that Checks which player has won a round (Currenlty at random).
    public void FindLeadWinner()
    {   
        Debug.Log("FindLeadWinner() has run.");
        Debug.Log("Card Sets on Table:");
        PrintCardSetsOnTable();


        //int winningPlayerIndex = gameJudge.FindWinner(new List<List<CardScript>>(cardSetsOnTable));

        List<CardScript> winningCardSet = gameJudge.FindWinner(new List<List<CardScript>>(cardSetsOnTable));
        //int winningPlayerIndex = cardSetsOnTable.IndexOf(winningCardSet);

        int playerIndex = -1;
        for (int i = 0; i < cardSetsOnTable.Count; ++i)
        {
            if (cardSetsOnTable[i].SequenceEqual(winningCardSet))
            {
                playerIndex = i;
                break;
            }
        }

        ClearTable();

        Debug.Log($"Winning P Index: {playerIndex}");
        Debug.Log($"Winning Player: {winningCardSet[0].GetCardOwner().gameObject.name}");

        int winningPlayerIndex = gameManager.GetPlayerIndex(winningCardSet[0].GetCardOwner());

        gameManager.NextTurn(winningPlayerIndex);
    }
    

    public void ClearTable()
    {
        Debug.Log("ClearTable() has started.");

        foreach (GameObject area in cardPlacementAreas)
        {
            int childCount = area.transform.childCount;
            for (int i = childCount - 1; i >= 0; --i) 
            {
                DestroyImmediate(area.transform.GetChild(i).gameObject);
            }
        }

        cardSetsOnTable.Clear();

        Debug.Log("ClearTable() has run.");
    }

    private void PrintCardSetsOnTable()
    {   
        Debug.Log($"{this.gameObject.name} card sets are:");

        for(int i = 0; i < cardSetsOnTable.Count; ++i)
        {
            Debug.Log($"Set - {i}:");

            for(int j = 0; j < cardSetsOnTable[i].Count; ++j)
            {
                Debug.Log($"{cardSetsOnTable[i][j].GetCardRank()} of {cardSetsOnTable[i][j].GetCardSuit()}");
            }
        }
    }

}
