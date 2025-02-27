using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ToggleGroup mNoOfOpponentsToggle;
    [SerializeField] private GameObject mOpponentSelectionMenu;
    [SerializeField] private GameObject mReadyButton;
    [SerializeField] private GameObject mGameResultsMenu;
    [SerializeField] private TextMeshProUGUI mDeclareWinnerText;
    [SerializeField] private GameObject mGameDrawMessage;
    [SerializeField] private GameObject mSortCardsButton;
    [SerializeField] private List<GameObject> mPlayerOptions = new List<GameObject>();
    [SerializeField] private DeckScript mDeck;
    [SerializeField] private TableScript mTable;
    [SerializeField] private GameManager mGameManager;
    private List<PlayerScript> _playerList =  new List<PlayerScript>();
    public static int noOfPlayers;

    void Start()
    {
        if(PlayerPrefs.GetInt("isReplayed") == 1)
        {
            mOpponentSelectionMenu.SetActive(false);
            RestartGame();
        }
    }

    // Resets the game when there is a Draw.
    private void RestartGame()
    {
        Debug.Log("Restarting game.");

        UIManager.noOfPlayers = PlayerPrefs.GetInt("noOfOpponents") + 1;
        SetUpOpponents(PlayerPrefs.GetInt("noOfOpponents")); 

        FindPlayers(); 
        mDeck.DistributeCards(PlayerPrefs.GetInt("noOfOpponents"));
        mTable.SetUpCardPlacementAreas(noOfPlayers);
        mGameManager.FindPlayers();
        mReadyButton.SetActive(true);
    }

    // Loads the "Play Again" menu after a game is finished. 
    public void PlayAgainScene(PlayerScript playerWhoWon)
    {
        DisableAllOptions();
        GameObject.Find("Main Player").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -637);
        mGameResultsMenu.SetActive(true);
        mDeclareWinnerText.SetText($"{playerWhoWon.gameObject.name} has won!");
    }

    // Loads the "Game Draw? menu when the game is a draw.
    public void ShowGameDrawMessage()
    {
        DisableAllOptions();
        GameObject.Find("Main Player").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -589);
        mGameDrawMessage.SetActive(true);

        StartCoroutine(GameReloadAfterDelay(3.0f));
    }

    private IEnumerator GameReloadAfterDelay(float delay)
    {  
        yield return new WaitForSeconds(delay);
        //gameDrawMessage.SetActive(false);
        ReloadScene();
    }
    

    // Sets up the number of Opponents for the current game, and distribues cards to them. 
    public void StartGameButton()
    {
        Toggle option = mNoOfOpponentsToggle.ActiveToggles().FirstOrDefault();
        mOpponentSelectionMenu.SetActive(false);
        mReadyButton.SetActive(true);

        Debug.Log($"Player chose option: {option.GetComponentInChildren<TextMeshProUGUI>().text}");
        int noOfOpponents = int.Parse(option.GetComponentInChildren<TextMeshProUGUI>().text);

        UIManager.noOfPlayers = noOfOpponents + 1;
        SetUpOpponents(noOfOpponents); 

        FindPlayers(); 
        mDeck.DistributeCards(noOfOpponents);
        mTable.SetUpCardPlacementAreas(noOfPlayers);
        mGameManager.FindPlayers();
    }

    // Sets up the UI based on user preferance (i.e. the no. of opponents the user wants).
    private void SetUpOpponents(int noOfOpponents)
    {
        DisableAllOptions();
        
        GameObject.Find("Main Player").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -218);

        switch(noOfOpponents)
        {
            case 1:
            Debug.Log($"Player chose {noOfOpponents} opponents"); 
            mPlayerOptions[0].SetActive(true);
            break;

            case 2:
            Debug.Log($"Player chose {noOfOpponents} opponents"); 
            mPlayerOptions[1].SetActive(true);
            break;

            case 3:
            Debug.Log($"Player chose {noOfOpponents} opponents"); 
            mPlayerOptions[2].SetActive(true);
            break;

            case 4: 
            Debug.Log($"Player chose {noOfOpponents} opponents");
            mPlayerOptions[3].SetActive(true);
            break;
        }
    }

    // Disables all player gameObjects.
    private void DisableAllOptions()
    {   
        for(int i = 0; i < mPlayerOptions.Count; ++i)
        {
            mPlayerOptions[i].SetActive(false);
        }
    }

    // Adds all players from the current game into the playerList.
    private void FindPlayers()
    {   
        _playerList.Add(GameObject.Find("Main Player").GetComponent<PlayerScript>());

        for(int i = 1; i < noOfPlayers; ++i)
        {
            _playerList.Add(GameObject.Find($"Player {i}").GetComponent<PlayerScript>());
        }
    }

    // Sorts Main Player's cards in decending order.
    public void SortMainPlayerCards()
    {
        mSortCardsButton.GetComponent<Button>().interactable = false;
        GameObject.Find("Main Player").GetComponent<PlayerScript>().SortMainPlayerCardsList();
    }

    // Starts placing player cards on the table when the player presses the Ready Button. 
    public void ReadyButton()
    {
        mReadyButton.SetActive(false);
        MakeCardSets();
        StartCoroutine(StartGameAfterDelay(1.0f));
    }
    
    private IEnumerator StartGameAfterDelay(float delay)
    {  
        yield return new WaitForSeconds(delay);
        mGameManager.StartGame();
    }
    
    // Creates 3 sets of cards for each player. 
    public void MakeCardSets()
    {
        foreach(PlayerScript player in _playerList)
        {   
            player.gameObject.transform.Find($"{player.gameObject.name} Sets").gameObject.SetActive(true);
            player.gameObject.transform.Find($"{player.gameObject.name} Cards").gameObject.SetActive(false);
            
            if(player.gameObject.name == "Main Player")
            {
                player.GetMainPlayerCardSets();
            }
            else
            {   
                player.GetPlayerCardSets();
            }
        }
    }

    // Creates a new game for the player. 
    public void PlayAgainButton()
    {
        Debug.Log("ReloadScene()");

        PlayerPrefs.SetInt("isReplayed", 0);
        PlayerPrefs.SetInt("noOfOpponents", UIManager.noOfPlayers - 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    // Reloads the game after a draw. 
    public void ReloadScene()
    {   
        Debug.Log("ReloadScene()");

        PlayerPrefs.SetInt("isReplayed", 1);
        PlayerPrefs.SetInt("noOfOpponents", UIManager.noOfPlayers - 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}


