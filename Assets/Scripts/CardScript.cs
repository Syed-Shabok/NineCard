using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{   
    [SerializeField] private Suit mCardSuit;
    [SerializeField] private Rank mCardRank;
    [SerializeField] private Image mCardImageComponent;
    private PlayerScript cardOwner; 

    void Awake()
    {
        if(mCardImageComponent == null) 
        {
            this.mCardImageComponent = GetComponent<Image>();
        }
    }

    // Sets all the fields of the card's CardScript component. 
    public void SetCardScript(Card card)
    {   
        if(this.mCardImageComponent == null)
        {
            Debug.Log("Can not find card object. Check if cards objects is SetActive(false)");
            return;
        }

        this.mCardSuit = card.GetCardSuit();
        this.mCardRank = card.GetCardRank();
        this.mCardImageComponent.sprite = card.GetCardSprite();

        //Debug.Log($"{this.gameObject.name} was set successfully.");
    }

    // Sets all the fields of the card's CardScript component. 
    public void SetCardScript(CardScript card)
    {   
        if(this.mCardImageComponent == null)
        {
            Debug.Log("Can not find card object. Check if cards objects is SetActive(false)");
            return;
        }

        this.mCardSuit = card.GetCardSuit();
        this.mCardRank = card.GetCardRank();
        this.mCardImageComponent.sprite = card.GetCardImage();

        //Debug.Log($"{this.gameObject.name} was set successfully.");
    }

    // Sets the owner of CardScript instance. 
    public void SetCardOwner(PlayerScript player)
    {
        this.cardOwner = player;
    }
    
    // Returns the PlayerScript owner of CardScript instance. 
    public PlayerScript GetCardOwner()
    {
        return this.cardOwner;
    }

    // Returns CardScript instance's Suit. 
    public Suit GetCardSuit()
    {
        return this.mCardSuit;
    }

    // Returns CardScript instance's Rank. 
    public Rank GetCardRank()
    {
        return this.mCardRank;
    }

    // Returns CardScript instance's Sprite Image. 
    public Sprite GetCardImage()
    {
        return this.mCardImageComponent.sprite;
    }   

}



