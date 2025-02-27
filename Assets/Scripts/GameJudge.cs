using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum HandRank {HIGHCARD, PAIR, COLOR, RUN, COLORRUN, TREY};

public class GameJudge : MonoBehaviour
{   
    // HandRank-wise card set Dictionary used in sorting functions. 
    private Dictionary<HandRank, List<List<CardScript>>> trackingDic = new Dictionary<HandRank, List<List<CardScript>>>
                                                        {
                                                            {HandRank.HIGHCARD, new List<List<CardScript>>()},
                                                            {HandRank.PAIR, new List<List<CardScript>>()},
                                                            {HandRank.COLOR, new List<List<CardScript>>()},
                                                            {HandRank.RUN, new List<List<CardScript>>()},
                                                            {HandRank.COLORRUN, new List<List<CardScript>>()},
                                                            {HandRank.TREY, new List<List<CardScript>>()}
                                                        };

    // Finds the best card set from a given list of card sets, and returns it.  
    public List<CardScript> FindWinner(List<List<CardScript>> cardSetsOnTable)
    {
        ClearTrackingDictionary();
        
        HandRank bestRank = HandRank.HIGHCARD;
        
        foreach(var set in cardSetsOnTable)
        {
            SortByHandRank(set);

            if(GetHandRankValue(GetSetRank(set)) > GetHandRankValue(bestRank))
            {
                bestRank = GetSetRank(set);
            }
        }

        Debug.Log($"Best Hand Rank is: {bestRank}.");

        if(trackingDic[bestRank].Count > 1)
        {
            List<CardScript> bestCardSet = trackingDic[bestRank][0];

            if(bestRank == HandRank.PAIR || bestRank == HandRank.HIGHCARD)
            {
                foreach(var set in trackingDic[bestRank])
                {   
                    if(GetRankValueOfFirstCard(set) >= GetRankValueOfFirstCard(bestCardSet))
                    {
                        if(GetRankValueOfFirstCard(set) == GetRankValueOfFirstCard(bestCardSet))
                        {
                            if(GetCumulativeRankOfCards(set) >= GetCumulativeRankOfCards(bestCardSet))
                            {
                                bestCardSet = set;
                            }
                        }
                        else
                        {
                            bestCardSet = set;
                        }
                    }
                }
            }
            else if(bestRank == HandRank.COLORRUN || bestRank == HandRank.RUN)
            {
                foreach(var set in trackingDic[bestRank])
                {
                    if(GetRankValueOfFirstCard(set) >= GetRankValueOfFirstCard(bestCardSet))
                    {
                        if(GetRankValueOfFirstCard(set) == GetRankValueOfFirstCard(bestCardSet))
                        {
                            if(GetRankValueOfSecondCard(set) >= GetRankValueOfSecondCard(bestCardSet))
                            {
                                bestCardSet = set;
                            }
                        }
                        else
                        {
                            bestCardSet = set;
                        }
                    }
                }            
            }
            else
            {
                foreach(var set in trackingDic[bestRank])
                {
                    if(GetCumulativeRankOfCards(set) >= GetCumulativeRankOfCards(bestCardSet))
                    {
                        bestCardSet = set;
                    }
                }
            }

            Debug.Log("Best Card Set is:");
            PrintCardList(bestCardSet);
            
            //Debug.Log($"{cardSetsOnTable[cardSetsOnTable.IndexOf(bestCardSet)][0].GetCardOwner().gameObject.name} has won.");

            return new List<CardScript>(bestCardSet);
        }
        else
        {
            List<CardScript> bestCardSet = trackingDic[bestRank][0];
            Debug.Log("Best Card Set is:");
            PrintCardList(bestCardSet);

            //Debug.Log($"{cardSetsOnTable[cardSetsOnTable.IndexOf(bestCardSet)][0].GetCardOwner().gameObject.name} has won.");

            return new List<CardScript>(bestCardSet);
        }
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

    // Returns the HandRank of the given parameter cardSet.
    private HandRank GetSetRank(List<CardScript> cardSet)
    {
        if(CheckIfTray(cardSet))
        {            
            return HandRank.TREY;
        }
        else if(CheckIfColorRun(cardSet))
        {
            return HandRank.COLORRUN;
        }
        else if(CheckIfRun(cardSet))
        {
            return HandRank.RUN;
        }
        else if(CheckIfColor(cardSet))
        {
            return HandRank.COLOR;
        }
        else if(CheckIfPair(cardSet))
        {
            return HandRank.PAIR;
        }
        else
        {
            return HandRank.HIGHCARD;
        }
    }

    // Inserts the parameter cardSet into the trackingDic based on HandRank.
    private void SortByHandRank(List<CardScript> cardSet)
    {
        if(CheckIfTray(cardSet))
        {
            trackingDic[HandRank.TREY].Add(cardSet);
        }
        else if(CheckIfColorRun(cardSet))
        {
            trackingDic[HandRank.COLORRUN].Add(cardSet);
        }
        else if(CheckIfRun(cardSet))
        {
            trackingDic[HandRank.RUN].Add(cardSet);
        }
        else if(CheckIfColor(cardSet))
        {
            trackingDic[HandRank.COLOR].Add(cardSet);
        }
        else if(CheckIfPair(cardSet))
        {
            trackingDic[HandRank.PAIR].Add(cardSet);
        }
        else
        {
            trackingDic[HandRank.HIGHCARD].Add(cardSet); 
        }
    }

    // Checks if the given parameter cardSet is of the HandRank TRAY.
    private bool CheckIfTray(List<CardScript> cardSet)
    {
        Rank requiredRank = cardSet[0].GetCardRank();
        for(int i = 1; i < cardSet.Count; ++i)
        {
            if(cardSet[i].GetCardRank() != requiredRank)
            {
                return false;
            }
        }
        return true;
    }

    // Checks if the given parameter cardSet is of the HandRank COLOR RUN.
    private bool CheckIfColorRun(List<CardScript> cardSet)
    {
        if(CheckIfColor(cardSet))
        {
            if(CheckIfRun(cardSet))
            {
                return true;
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    // Checks if the given parameter cardSet is of the HandRank RUN.
    private bool CheckIfRun(List<CardScript> cardSet)
    {
        if(cardSet[0].GetCardRank() == Rank.ACE && cardSet[1].GetCardRank() == Rank.THREE && cardSet[2].GetCardRank() == Rank.TWO)
        {
            return true;
        }
        
        int rankValue = GetRankValue(cardSet[0].GetCardRank());
        for(int i = 1; i < cardSet.Count; ++i)
        {   
            --rankValue;
            if(rankValue != GetRankValue(cardSet[i].GetCardRank()))
            {
                return false;
            }
        }
        return true;
    }

    // Checks if the given parameter cardSet is of the HandRank COLOR.
    private bool CheckIfColor(List<CardScript> cardSet)
    {
        Suit requiredSuit = cardSet[0].GetCardSuit();
        for(int i = 1; i < cardSet.Count; ++i)
        {
            if(cardSet[i].GetCardSuit() != requiredSuit)
            {
                return false;
            }
        }
        return true;
    }

    // Checks if the given parameter cardSet is of the HandRank PAIR.
    private bool CheckIfPair(List<CardScript> cardSet)
    {
        if(cardSet[0].GetCardRank() == cardSet[1].GetCardRank())
        {
            return true;
        }
        return false;
    }

    // Sorts the Main Player's card sets based on HandRank in Decending order. 
    public void SortMainPlayerCards(List<List<CardScript>> cardSetList)
    {   
        // Sorts or makes proper sets out of the sets made by the Main Player. 
        foreach(var set in cardSetList)
        {
            MakeProperSet(set);
        }
        
        Dictionary<HandRank, List<List<CardScript>>> tempDic = new Dictionary<HandRank, List<List<CardScript>>>
                                                        {
                                                            {HandRank.TREY, new List<List<CardScript>>()},
                                                            {HandRank.COLORRUN, new List<List<CardScript>>()},
                                                            {HandRank.RUN, new List<List<CardScript>>()},
                                                            {HandRank.COLOR, new List<List<CardScript>>()},
                                                            {HandRank.PAIR, new List<List<CardScript>>()},
                                                            {HandRank.HIGHCARD, new List<List<CardScript>>()}
                                                        };   
        
        for(int i = 0; i < cardSetList.Count; ++i)
        {
            if(CheckIfTray(cardSetList[i]))
            {
                tempDic[HandRank.TREY].Add(cardSetList[i]);
            }
            else if(CheckIfColorRun(cardSetList[i]))
            {
                tempDic[HandRank.COLORRUN].Add(cardSetList[i]);
            }
            else if(CheckIfRun(cardSetList[i]))
            {
                tempDic[HandRank.RUN].Add(cardSetList[i]);
            }
            else if(CheckIfColor(cardSetList[i]))
            {
                tempDic[HandRank.COLOR].Add(cardSetList[i]);
            }
            else if(CheckIfPair(cardSetList[i]))
            {
                tempDic[HandRank.PAIR].Add(cardSetList[i]);
            }
            else
            {
                tempDic[HandRank.HIGHCARD].Add(cardSetList[i]); 
            }
        }

        foreach(HandRank handRank in tempDic.Keys)
        {
            if(tempDic[handRank].Count > 1)
            {
                if(handRank == HandRank.PAIR || handRank == HandRank.HIGHCARD)
                {
                    tempDic[handRank].Sort((set1, set2) =>
                    {
                        int primaryComparison = GetRankValueOfFirstCard(set2).CompareTo(GetRankValueOfFirstCard(set1));

                        if (primaryComparison == 0)
                        {
                            return GetCumulativeRankOfCards(set2).CompareTo(GetCumulativeRankOfCards(set1));
                        }

                        return primaryComparison;
                    });
                }
                else if(handRank == HandRank.COLORRUN || handRank == HandRank.RUN)
                {
                    tempDic[handRank].Sort((set1, set2) =>
                    {
                        int primaryComparison = GetRankValueOfFirstCard(set2).CompareTo(GetRankValueOfFirstCard(set1));

                        if (primaryComparison == 0)
                        {
                            return GetRankValueOfSecondCard(set2).CompareTo(GetRankValueOfSecondCard(set1));
                        }

                        return primaryComparison;
                    });
                }
                else
                {
                    tempDic[handRank].Sort((set1, set2) => 
                        GetCumulativeRankOfCards(set2).CompareTo(GetCumulativeRankOfCards(set1)));
                }
            }
        }

        cardSetList.Clear();

        foreach(HandRank handRank in ((HandRank[])System.Enum.GetValues(typeof(HandRank))).Reverse())
        {
            foreach(var cardSet in tempDic[handRank])
            {
                cardSetList.Add(cardSet);
            }
        }
    }

    // If Main player forgets to sort each set properly, then this funtion does it for him/her. 
    private void MakeProperSet(List<CardScript> cardSet)
    {
        if(SetHasPair(cardSet))
        {
            Rank pairRank = FindPairRank(cardSet);
            List<CardScript> pairSet = new List<CardScript>();

            foreach(var card in cardSet)
            {
                if(card.GetCardRank() == pairRank)
                {
                    pairSet.Add(card);
                }
            }

            foreach(var card in cardSet)
            {
                if(card.GetCardRank() != pairRank)
                {
                    pairSet.Add(card);
                }
            }

            cardSet.Clear();
            cardSet.AddRange(pairSet); 
        }
        else
        {
            cardSet.Sort((set1, set2) => 
                GetRankValue(set2.GetCardRank()).CompareTo(GetRankValue(set1.GetCardRank())));
        }
    }

    //Checks of a given parameter cardSet has two cards of the same card rank. 
    bool SetHasPair(List<CardScript> cardSet)
    {
        if(cardSet.Count != 3)
        {
            return false;
        }  

        Rank rank1 = cardSet[0].GetCardRank();
        Rank rank2 = cardSet[1].GetCardRank();
        Rank rank3 = cardSet[2].GetCardRank();
        
        return (rank1 == rank2) || (rank1 == rank3) || (rank2 == rank3);
    }

    // Returns the rank of the pair cards from the given paremeter cardSet.
    private Rank FindPairRank(List<CardScript> cardSet)
    {   
        if(cardSet == null || cardSet.Count != 3)
        {
            throw new ArgumentException("Card set must contain exactly 3 cards.");
        }
        
        Rank rank1 = cardSet[0].GetCardRank();
        Rank rank2 = cardSet[1].GetCardRank();
        Rank rank3 = cardSet[2].GetCardRank();
        
        if(rank1 == rank2)
        {
            return rank1;
        }
        if(rank1 == rank3)
        {
            return rank1;
        }
        if(rank2 == rank3)
        {
            return rank2;
        }
        
        throw new ArgumentException("Set does not countain pair");
    }

    // Returns the card rank value of the given paramater rank. 
    private int GetRankValue(Rank rank)
    {
        return (int)rank;
    }

    // Returns the HandRank value of the given paramater handRank. 
    private int GetHandRankValue(HandRank handRank)
    {
        return (int)handRank;
    }

    // Clears the trackingDic dictionary. 
    private void ClearTrackingDictionary()
    {
        foreach(HandRank handRank in (HandRank[])System.Enum.GetValues(typeof(HandRank)))
        {
            trackingDic[handRank].Clear();
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    public void PrintCardList(List<CardScript> cardList)
    {
        foreach(CardScript card in cardList)
        {
            Debug.Log($"{card.GetCardRank()} of {card.GetCardSuit()}.");
        }
    }

}
