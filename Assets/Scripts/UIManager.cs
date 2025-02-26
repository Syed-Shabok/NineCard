using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private ToggleGroup noOfOpponentsToggle;
    [SerializeField] private GameObject opponentSelectionMenu;
    [SerializeField] private GameObject readyButton;
    [SerializeField] private GameObject gameResultsMenu;
    [SerializeField] private TextMeshProUGUI declareWinnerText;
    [SerializeField] private GameObject gameDrawMessage;
    [SerializeField] private GameObject sortCardsButton;

    [SerializeField] private List<GameObject> playerOptions = new List<GameObject>();
    [SerializeField] private DeckScript deck;
    [SerializeField] private TableScript table;
    [SerializeField] private GameManager gameManager;

    [SerializeField] private List<PlayerScript> playerList =  new List<PlayerScript>();
    public static int noOfPlayers;

    void Start()
    {
        if(PlayerPrefs.GetInt("isReplayed") == 1)
        {
            opponentSelectionMenu.SetActive(false);
            RestartGame();
        }
    }

    private void RestartGame()
    {
        Debug.Log("Restarting game.");

        UIManager.noOfPlayers = PlayerPrefs.GetInt("noOfOpponents") + 1;
        SetUpOpponents(PlayerPrefs.GetInt("noOfOpponents")); 

        FindPlayers(); 
        deck.DistributeCards(PlayerPrefs.GetInt("noOfOpponents"));
        table.SetUpCardPlacementAreas(noOfPlayers);
        gameManager.FindPlayers();
        readyButton.SetActive(true);
    }

    public void PlayAgainScene(PlayerScript playerWhoWon)
    {
        DisableAllOptions();
        GameObject.Find("Main Player").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -637);
        gameResultsMenu.SetActive(true);
        declareWinnerText.SetText($"{playerWhoWon.gameObject.name} has won!");
    }

    public void ShowGameDrawMessage()
    {
        DisableAllOptions();
        GameObject.Find("Main Player").GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -589);
        gameDrawMessage.SetActive(true);

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
        Toggle option = noOfOpponentsToggle.ActiveToggles().FirstOrDefault();
        opponentSelectionMenu.SetActive(false);
        readyButton.SetActive(true);

        Debug.Log($"Player chose option: {option.GetComponentInChildren<TextMeshProUGUI>().text}");
        int noOfOpponents = int.Parse(option.GetComponentInChildren<TextMeshProUGUI>().text);

        UIManager.noOfPlayers = noOfOpponents + 1;
        SetUpOpponents(noOfOpponents); 

        FindPlayers(); 
        deck.DistributeCards(noOfOpponents);
        table.SetUpCardPlacementAreas(noOfPlayers);
        gameManager.FindPlayers();
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
            playerOptions[0].SetActive(true);
            break;

            case 2:
            Debug.Log($"Player chose {noOfOpponents} opponents"); 
            playerOptions[1].SetActive(true);
            break;

            case 3:
            Debug.Log($"Player chose {noOfOpponents} opponents"); 
            playerOptions[2].SetActive(true);
            break;

            case 4: 
            Debug.Log($"Player chose {noOfOpponents} opponents");
            playerOptions[3].SetActive(true);
            break;
        }
    }

    // Disables all player gameObjects.
    private void DisableAllOptions()
    {   
        for(int i = 0; i < playerOptions.Count; ++i)
        {
            playerOptions[i].SetActive(false);
        }
    }

    // Adds all players from the current game into the playerList.
    private void FindPlayers()
    {   
        playerList.Add(GameObject.Find("Main Player").GetComponent<PlayerScript>());

        for(int i = 1; i < noOfPlayers; ++i)
        {
            playerList.Add(GameObject.Find($"Player {i}").GetComponent<PlayerScript>());
        }
    }

    public void SortMainPlayerCards()
    {
        sortCardsButton.GetComponent<Button>().interactable = false;
        GameObject.Find("Main Player").GetComponent<PlayerScript>().SortMainPlayerCardsList();
    }

    public void DeclareFourOfAKind()
    {
        GameObject.Find("Main Player").GetComponent<PlayerScript>().SortMainPlayerCardsList();

    }

    public void ReadyButton()
    {
        readyButton.SetActive(false);
        MakeCardSets();
        StartCoroutine(StartGameAfterDelay(1.0f));
    }
    
    private IEnumerator StartGameAfterDelay(float delay)
    {  
        yield return new WaitForSeconds(delay);
        gameManager.StartGame();
    }
    
    // Creates 3 sets of cards for each player. 
    public void MakeCardSets()
    {
        foreach(PlayerScript player in playerList)
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

    // (Testing)
    public void ShowCardSets()
    {
        return;
        
        // foreach(PlayerScript player in playerList)
        // {
        //     player.ShowCardSets();
        // }
    }

    // (Testing) Place Cards button function.
    public void PlaceCards()
    {
        gameManager.StartGame();
    }

    public void PlayAgainButton()
    {
        Debug.Log("ReloadScene()");

        PlayerPrefs.SetInt("isReplayed", 0);
        PlayerPrefs.SetInt("noOfOpponents", UIManager.noOfPlayers - 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReloadScene()
    {   
        Debug.Log("ReloadScene()");

        PlayerPrefs.SetInt("isReplayed", 1);
        PlayerPrefs.SetInt("noOfOpponents", UIManager.noOfPlayers - 1);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

}


