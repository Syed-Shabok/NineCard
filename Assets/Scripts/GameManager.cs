using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{   
    private int playerTurn = 0;
    private int turnsTaken = 0;
    private int leadsPlayed = 0;
    [SerializeField] private List<PlayerScript> playerList = new List<PlayerScript>();
    [SerializeField] private TableScript table;
    [SerializeField] private UIManager userInterface; 
    

    public void StartGame()
    {   
        playerTurn = GetRandomStartingPlayer();
        MakePlayerTurn();
    }

    public void FindPlayers()
    {   
        playerList.Add(GameObject.Find("Main Player").GetComponent<PlayerScript>());

        for(int i = 1; i < UIManager.noOfPlayers; ++i)
        {
            playerList.Add(GameObject.Find($"Player {i}").GetComponent<PlayerScript>());
        }
    }

    private int GetRandomStartingPlayer()
    {   
        int ranndomTurn = UnityEngine.Random.Range(0, playerList.Count);
        Debug.Log($"First Player turn: {ranndomTurn}");

        return ranndomTurn;
    }

    private void MakePlayerTurn()
    {   
        Debug.Log($"Player Turn:{playerTurn} - MakePlayerTurn() has run.");
        Debug.Log($"Turns taken: {turnsTaken}");
        
        ++turnsTaken;

        if(turnsTaken > UIManager.noOfPlayers)
        {   
            Debug.Log("All turns finished, checking who won this round.");
            turnsTaken = 0;
            ++leadsPlayed;
            StartCoroutine(DeclareWinnerAfterDelay(1.0f));
        }
        else
        {
            if(leadsPlayed == 3)
            {
                FindGameWinner();
            }
            else
            {
                playerList[playerTurn].PlaceBestCardSet(playerTurn);
            }
        }
    }

    public void NextTurn()
    {
        playerTurn = (playerTurn + 1) % playerList.Count;
        StartCoroutine(StartNewRoundAfterDelay(1.0f));
    }

    public void NextTurn(int winningPlayerTurn)
    {
        playerTurn = winningPlayerTurn;
        playerList[winningPlayerTurn].GivePointToPlayer();
        StartCoroutine(StartNewRoundAfterDelay(1.0f));
    }

    private IEnumerator StartNewRoundAfterDelay(float delay)
    {  
        yield return new WaitForSeconds(delay);
        MakePlayerTurn();
    }

    private IEnumerator DeclareWinnerAfterDelay(float delay)
    {   
        yield return new WaitForSeconds(delay);
        table.FindLeadWinner();
    }  

    public bool IsRoundOngoing()
    {
        // If turnsTaken is 0, the round is over.
        return turnsTaken != 0; 
    } 

    public void FindGameWinner()
    {
        for(int i = 0; i < playerList.Count; ++i)
        {
            if(playerList[i].GetLeadsWon() >= 2)
            {
                Debug.Log($"{playerList[i].gameObject.name} has won the game.");
                userInterface.PlayAgainScene(playerList[i]);
                return;
            }
        }
        Debug.Log("Game is a Draw.");
        userInterface.ShowGameDrawMessage();
    }

    public int GetPlayerIndex(PlayerScript player)
    {
        return playerList.IndexOf(player);
    }


    public int GetLeadNumber()
    {
        return leadsPlayed;
    }


    void OnApplicationQuit()
    {
        Debug.Log("Application is quitting!");

        ClearAllRecords();
    }

    // Resets all PlayerPrefs values.
    private void ClearAllRecords()
    {
        Debug.Log("ClearAllRecords() has run.");

        PlayerPrefs.SetInt("isReplayed", 0);
    }
}
