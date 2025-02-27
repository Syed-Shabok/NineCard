using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class HandRankChecker : MonoBehaviour
{   
    // HandRank-wise card set Dictionary used in sorting functions. 
    private Dictionary<HandRank, List<List<CardScript>>> handRankWiseDec = new Dictionary<HandRank, List<List<CardScript>>>
                                                        {
                                                            {HandRank.TREY, new List<List<CardScript>>()},
                                                            {HandRank.COLORRUN, new List<List<CardScript>>()},
                                                            {HandRank.RUN, new List<List<CardScript>>()},
                                                            {HandRank.COLOR, new List<List<CardScript>>()},
                                                            {HandRank.PAIR, new List<List<CardScript>>()},
                                                            {HandRank.HIGHCARD, new List<List<CardScript>>()}
                                                        };
    private List<List<CardScript>> listOfCardSets = new List<List<CardScript>>();  // List of card sets used for storing player's card sets.
    private List<CardScript> currentCardList = new List<CardScript>(); // Used as a copy of the player's cards list.

    // Creates appropriate card sets for Bot players and returns the card sets. 
    public List<List<CardScript>> GetCardSets(PlayerScript player, List<CardScript> cardList)
    {
        ClearCardRecords();
        
        foreach(CardScript card in cardList)
        {
            currentCardList.Add(card);
        }

        SortCardList(currentCardList);

        //Debug.Log("Sorted Player Cards are:");
        //PrintCardList(currentCardList);

        CheckForTraySets();
        CheckForColorRunSpecialCase();
        CheckForColorRunSets();
        CheckForRunSpecialCase();
        CheckForRunSets();
        CheckForColorSets();
        CheckForPairs();
        CheckForHighCardSet();

        SortCardSets();
        PopulateListOfCardSets();

        Debug.Log($"{player.gameObject.name} sets found: ");
        
        PrintListOfCardSets();
        return new List<List<CardScript>>(listOfCardSets);
    }
    
    // Creates TRAY card sets from available bot player's cards.  
    private void CheckForTraySets()
    {
        int traySetCounter = 0;

        foreach(Rank rankGroup in ((Rank[])System.Enum.GetValues(typeof(Rank))).Reverse())
        {
            List<CardScript> traySet = new List<CardScript>();

            foreach(CardScript card in currentCardList)
            {
                if(card.GetCardRank() == rankGroup)
                {
                    traySet.Add(card);
                }
            }
            
            if(traySet.Count == 4)
            { 
                Debug.Log($"{traySet[0].GetCardOwner().gameObject.name} has got 4 a kind.");
                return;
            }

            if(traySet.Count == 3)
            {
                //Debug.Log("A Tray Set has been found.");

                handRankWiseDec[HandRank.TREY].Add(traySet);

                ++traySetCounter;
                RemoveCardsFromList(traySet);
            }
        }

        //Debug.Log($"{traySetCounter} Tray Sets found.");
    }

    // Creates the Best or Secret COLOR RUN card set from available bot-player's cards.  
    // (A,3,2) of the same rank is a secret COLOR RUN Set, it is the 2nd strongest COLOR RUN set, only surpassed by (A,K,Q).
    private void CheckForColorRunSpecialCase()
    {
        // 3 loops are done to account for the extremely rare case of having 3 sets of (A,K,Q).  
        for(int i = 0 ; i < 3; ++i)
        {
            CheckForColorRunBestSet();
        }

        // 3 loops are done to account for the extremely rare case of having 3 sets of (A,3,2).
        for(int i = 0 ; i < 3; ++i)
        {
            CheckForSecretColorRun();  
        }
    }

    // Creates the best COLOR RUN card set from available bot-player's cards (i.e. A,K,Q).  
    private void CheckForColorRunBestSet()
    {
        CardScript aceCard = currentCardList.Find(card => card.GetCardRank() == Rank.ACE);
        CardScript kingCard = currentCardList.Find(card => card.GetCardRank() == Rank.KING);
        CardScript queenCard = currentCardList.Find(card => card.GetCardRank() == Rank.QUEEN);

        if(aceCard != null && kingCard != null & queenCard != null)
        {
            List<CardScript> colorRunBestSet = new List<CardScript>{aceCard, kingCard, queenCard};

            Suit requiredSuit = colorRunBestSet[0].GetCardSuit();

            foreach(CardScript card in colorRunBestSet)
            {
                if(card.GetCardSuit() != requiredSuit)
                {
                    return;
                }
            }

            handRankWiseDec[HandRank.COLORRUN].Add(colorRunBestSet);
            RemoveCardsFromList(colorRunBestSet);
        }
    }

    // Creates the secret COLOR RUN card set from available bot-player's cards (i.e. A,3,2). 
    private void CheckForSecretColorRun()
    {
        CardScript aceCard = currentCardList.Find(card => card.GetCardRank() == Rank.ACE);
        CardScript threeCard = currentCardList.Find(card => card.GetCardRank() == Rank.THREE);
        CardScript twoCard = currentCardList.Find(card => card.GetCardRank() == Rank.TWO);

        if(aceCard != null && threeCard != null & twoCard != null)
        {
            List<CardScript> colorRunSecretSet = new List<CardScript>{aceCard, threeCard, twoCard};
            
            Suit requiredSuit = colorRunSecretSet[0].GetCardSuit();

            foreach(CardScript card in colorRunSecretSet)
            {
                if(card.GetCardSuit() != requiredSuit)
                {
                    return;
                }
            }

            handRankWiseDec[HandRank.COLORRUN].Add(colorRunSecretSet);
            RemoveCardsFromList(colorRunSecretSet);
        }
    }

    // Creates COLOR RUN card sets from available bot-player's cards. 
    private void CheckForColorRunSets()
    {
        //Debug.Log("Checking for Color Run Sets:");

        for(int i = 0; i < currentCardList.Count - 2; ++i)
        {
            //Debug.Log($"i={i}, Card: {cardList[i].GetCardRank()} of {cardList[i].GetCardSuit()}.");
            List<CardScript> colorRunSet = new List<CardScript>{currentCardList[i]};
            
            for(int j = i + 1; j < currentCardList.Count; ++j)
            {
                if(GetRankValue(currentCardList[j].GetCardRank()) == GetRankValue(colorRunSet[colorRunSet.Count - 1].GetCardRank()) - 1
                    && currentCardList[j].GetCardSuit() == colorRunSet[0].GetCardSuit()) 
                {
                    //Debug.Log($"Match with card: {cardList[j].GetCardRank()} of {cardList[j].GetCardSuit()}.");
                    colorRunSet.Add(currentCardList[j]);
                }

                if(colorRunSet.Count == 3)
                {
                    //Debug.Log("A Color Run Set has been found.");
                    handRankWiseDec[HandRank.COLORRUN].Add(colorRunSet);

                    RemoveCardsFromList(colorRunSet);
                    --i;
                    break; 
                }
            }
        }

        //Debug.Log($"{colorRunSetCounter} Color Run Sets found.");
    }
    
    // Creates the Best or Secret RUN card set from available bot-player's cards.
    // (A,3,2) is a Secret Color Rank Set, it is the 2nd stringest Color Run set, only surpassed by (A,K,Q).
    private void CheckForRunSpecialCase()
    {
        // 3 loops are done to account for the extremely rare case of having 3 sets of (A,K,Q).  
        for(int i = 0 ; i < 3; ++i)
        {
            CheckForRunBestSet();
        }

        // 3 loops are done to account for the extremely rare case of having 3 sets of (A,3,2).
        for(int i = 0 ; i < 3; ++i)
        {
            CheckForSecretRun();  
        }
    }

    // Creates Best RUN card set from available bot-player's cards (i.e. A,K,Q).    
    private void CheckForRunBestSet()
    {
        CardScript aceCard = currentCardList.Find(card => card.GetCardRank() == Rank.ACE);
        CardScript kingCard = currentCardList.Find(card => card.GetCardRank() == Rank.KING);
        CardScript queenCard = currentCardList.Find(card => card.GetCardRank() == Rank.QUEEN);

        if(aceCard != null && kingCard != null & queenCard != null)
        {
            List<CardScript> colorRunBestSet = new List<CardScript>{aceCard, kingCard, queenCard};
            handRankWiseDec[HandRank.RUN].Add(colorRunBestSet);
            RemoveCardsFromList(colorRunBestSet);
        }
    }

    // Creates secret RUN card set from available bot-player's cards (i.e. A,3,2).    
    private void CheckForSecretRun()
    {
        CardScript aceCard = currentCardList.Find(card => card.GetCardRank() == Rank.ACE);
        CardScript threeCard = currentCardList.Find(card => card.GetCardRank() == Rank.THREE);
        CardScript twoCard = currentCardList.Find(card => card.GetCardRank() == Rank.TWO);

        if(aceCard != null && threeCard != null & twoCard != null)
        {
            List<CardScript> colorRunSecretSet = new List<CardScript>{aceCard, threeCard, twoCard};
            handRankWiseDec[HandRank.RUN].Add(colorRunSecretSet);
            RemoveCardsFromList(colorRunSecretSet);
        }
    }

    // Creates RUN card sets from available bot-player's cards.  
    private void CheckForRunSets()
    {   
        //Debug.Log("Checking for Run Sets:");

        for(int i = 0; i < currentCardList.Count - 2; ++i)
        {
            //Debug.Log($"i={i}, Card: {cardList[i].GetCardRank()} of {cardList[i].GetCardSuit()}.");
            List<CardScript> runSet = new List<CardScript>{currentCardList[i]};

            for(int j = i + 1; j < currentCardList.Count; ++j)
            {
                if(GetRankValue(currentCardList[j].GetCardRank()) == GetRankValue(runSet[runSet.Count - 1].GetCardRank()) - 1)
                {
                    //Debug.Log($"Match with card: {cardList[j].GetCardRank()} of {cardList[j].GetCardSuit()}.");
                    runSet.Add(currentCardList[j]);
                }

                if(runSet.Count == 3)
                {
                    //Debug.Log("A Run Set has been found.");

                    handRankWiseDec[HandRank.RUN].Add(runSet);

                    RemoveCardsFromList(runSet);
                    --i;
                    break; 
                }
            }
        }

        //Debug.Log($"{runSetCounter} Run Sets found.");
    }

    // Creates COLOR card sets from available bot-player's cards.  
    private void CheckForColorSets()
    {
        foreach(Suit suitGroup in (Suit[])System.Enum.GetValues(typeof(Suit)))
        {   
            List<CardScript>colorSet = new List<CardScript>();

            foreach(CardScript card in currentCardList)
            {
                if(card.GetCardSuit() == suitGroup)
                {
                    colorSet.Add(card);
                }

                if(colorSet.Count == 3)
                {
                    //Debug.Log("A Color Set has been found.");
                    
                    handRankWiseDec[HandRank.COLOR].Add(colorSet);

                    RemoveCardsFromList(colorSet);
                    break;
                }
            }
        }
        
        ///Debug.Log($"{colorSetCounter} Color Sets have been found");
    }

    // Creates PAIR card sets from available bot player's cards.  
    private void CheckForPairs()
    {

        foreach(Rank rankGroup in ((Rank[])System.Enum.GetValues(typeof(Rank))).Reverse())
        {
            List<CardScript> pairSet = new List<CardScript>();

            foreach(CardScript card in currentCardList)
            {
                if(card.GetCardRank() == rankGroup)
                {
                    pairSet.Add(card);
                }
            }
            
            if(pairSet.Count == 2)
            {
                RemoveCardsFromList(pairSet);

                CardScript lowestRankCard = GetLowestRankCard();
                pairSet.Add(lowestRankCard);

                //Debug.Log("A Pair Set has been found.");
                
                handRankWiseDec[HandRank.PAIR].Add(pairSet);

                RemoveCardsFromList(pairSet);
            }
        }

        ///Debug.Log($"{pairSetCounter} Pair Sets found.");
    }
    
    // Creates HIGH CARD card sets from available bot player's cards.  
    private void CheckForHighCardSet()
    {
        int topCardCount = currentCardList.Count / 3;
        List<CardScript> topCards = new List<CardScript>();

        for(int i = 0; i < topCardCount; ++i)
        {
            topCards.Add(currentCardList[i]);
        }

        List<CardScript> otherCards = new List<CardScript>();

        for(int i = topCardCount; i < currentCardList.Count; ++i)
        {
            otherCards.Add(currentCardList[i]);
        }

        int highCardSetCounter = 0;
        
        foreach(CardScript card in topCards)
        {
            List<CardScript> highCardSet = new List<CardScript> {card};

            for(int i = 0; i < 2; ++i)
            {
                highCardSet.Add(otherCards[0]);
                otherCards.RemoveAt(0);

                if(highCardSet.Count == 3)
                {
                    //Debug.Log("A High Card Set has been found.");
                    
                    handRankWiseDec[HandRank.HIGHCARD].Add(highCardSet);

                    ++highCardSetCounter;
                    
                    RemoveCardsFromList(highCardSet);
                }
            }
        }

        //Debug.Log($"{highCardSetCounter} High Card Sets found.");
    }

    // Sorts bot-players card sets from highest to lowest in Hand Rank. 
    private void SortCardSets()
    {
        foreach(HandRank handRank in handRankWiseDec.Keys)
        {   
            //Checks if there are multiple sets of the same Hand Rank.
            if(handRankWiseDec[handRank].Count > 1)
            {   
                // For RAIR and HIGH CARD : Compares rank of first card in set, then looks into cumulative rank of all cards in the set. 
                if(handRank == HandRank.PAIR || handRank == HandRank.HIGHCARD)
                {   
                    handRankWiseDec[handRank].Sort((set1, set2) =>
                    {
                        int primaryComparison = GetRankValueOfFirstCard(set2).CompareTo(GetRankValueOfFirstCard(set1));

                        if (primaryComparison == 0)
                        {
                            return GetCumulativeRankOfCards(set2).CompareTo(GetCumulativeRankOfCards(set1));
                        }

                        return primaryComparison;
                    });
                }
                // For COLOR RUN and RUN : Compares rank of first card in set, then compares rank of second card in the set.
                // This is done to account for the secret card set of COLOR RUN and RUN.   
                else if(handRank == HandRank.COLORRUN || handRank == HandRank.RUN)
                {
                    handRankWiseDec[handRank].Sort((set1, set2) =>
                    {
                        int primaryComparison = GetRankValueOfFirstCard(set2).CompareTo(GetRankValueOfFirstCard(set1));

                        if (primaryComparison == 0)
                        {
                            return GetRankValueOfSecondCard(set2).CompareTo(GetRankValueOfSecondCard(set1));
                        }

                        return primaryComparison;
                    });
                }
                // For other Hand Ranks cumulative card rank of all cards in the set is compared. 
                else
                {   
                    handRankWiseDec[handRank].Sort((set1, set2) => 
                        GetCumulativeRankOfCards(set2).CompareTo(GetCumulativeRankOfCards(set1)));
                }
            }
        }
    }

    // Populates the listOfCardSets with the card sets created. 
    private void PopulateListOfCardSets()
    {
        listOfCardSets.Clear();
        
        foreach(HandRank handRank in ((HandRank[])System.Enum.GetValues(typeof(HandRank))).Reverse())
        {
            foreach(var cardSet in handRankWiseDec[handRank])
            {
                listOfCardSets.Add(cardSet);
            }
        }
    }

    // Returns the lowest ranked card from the available bot-player cards.
    private CardScript GetLowestRankCard()
    {
        return currentCardList[currentCardList.Count - 1];
    }

    // Removes the cards in the given parameter cardSet from the currentCardList. 
    private void RemoveCardsFromList(List<CardScript> cardSet)
    {
        foreach(CardScript card in cardSet)
        {
            currentCardList.Remove(card);
        }
    }

    // Sorts a given card list in Decending order. 
    private void SortCardList(List<CardScript> cardList)
    {
        cardList.Sort((card1, card2) => GetRankValue(card2.GetCardRank()).CompareTo(GetRankValue(card1.GetCardRank())));
    }

    // Returns the rank value of a card. 
    private int GetRankValue(Rank rank)
    {
        return (int)rank;
    }

    // Returns the combined card rank value of all cards in the given cardSet parameter. 
    private int GetCumulativeRankOfCards(List<CardScript> cardSet)
    {
        int cumulativeRank = 0;

        foreach(CardScript card in cardSet)
        {
            cumulativeRank += GetRankValue(card.GetCardRank());
        }

        return cumulativeRank;
    }

    // Returns the rank of the first card in the given cardSet parameter.
    private int GetRankValueOfFirstCard(List<CardScript> cardSet)
    {
        return (GetRankValue(cardSet[0].GetCardRank()));
    }

    // Returns the rank of the second card in the given cardSet parameter. 
    private int  GetRankValueOfSecondCard(List<CardScript> cardSet)
    {
        return (GetRankValue(cardSet[1].GetCardRank()));
    }

    // Removes all cards from handRankWiseDec, listOfCardSets and currentCardList.
    private void ClearCardRecords()
    {
        foreach(HandRank handRank in (HandRank[])System.Enum.GetValues(typeof(HandRank)))
        {
            handRankWiseDec[handRank].Clear();
        }

        listOfCardSets.Clear();
        currentCardList.Clear();
    }


    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////


    private void PrintListOfCardSets()
    {   
        Debug.Log($"{this.gameObject.name} card sets are:");

        for(int i = 0; i < listOfCardSets.Count; ++i)
        {
            Debug.Log($"Set {i}:");

            for(int j = 0; j < listOfCardSets[i].Count; ++j)
            {
                Debug.Log($"{listOfCardSets[i][j].GetCardRank()} of {listOfCardSets[i][j].GetCardSuit()}");
            }
        }
    }
    
    private void PrintCardList(List<CardScript> cardList)
    {
        foreach(CardScript card in cardList)
        {
            Debug.Log($"{card.GetCardRank()} of {card.GetCardSuit()}");
        }
    }

}
