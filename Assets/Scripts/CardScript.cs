using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardScript : MonoBehaviour
{   
    [SerializeField] private Suit mCardSuit;
    [SerializeField] private Rank mCardRank;
    [SerializeField] private Image CardImageComponent;
    private PlayerScript cardOwner; 

    void Awake()
    {
        if(CardImageComponent == null) 
        {
            this.CardImageComponent = GetComponent<Image>();
        }
    }

    // Sets all the fields of the card's CardScript component. 
    public void SetCardScript(Card card)
    {   
        if(this.CardImageComponent == null)
        {
            Debug.Log("Can not find card object. Check if cards objects is SetActive(false)");
            return;
        }

        this.mCardSuit = card.GetCardSuit();
        this.mCardRank = card.GetCardRank();
        this.CardImageComponent.sprite = card.GetCardSprite();

        //Debug.Log($"{this.gameObject.name} was set successfully.");
    }

    // Sets all the fields of the card's CardScript component. 
    public void SetCardScript(CardScript card)
    {   
        if(this.CardImageComponent == null)
        {
            Debug.Log("Can not find card object. Check if cards objects is SetActive(false)");
            return;
        }

        this.mCardSuit = card.GetCardSuit();
        this.mCardRank = card.GetCardRank();
        this.CardImageComponent.sprite = card.GetCardImage();

        //Debug.Log($"{this.gameObject.name} was set successfully.");
    }

    public void SetCardOwner(PlayerScript player)
    {
        this.cardOwner = player;
    }
    
    public PlayerScript GetCardOwner()
    {
        return this.cardOwner;
    }

    public Suit GetCardSuit()
    {
        return this.mCardSuit;
    }

    public Rank GetCardRank()
    {
        return this.mCardRank;
    }

    public Sprite GetCardImage()
    {
        return this.CardImageComponent.sprite;
    }   

}



