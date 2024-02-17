using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAcceptor : MonoBehaviour
{
    public Card lastCardAccepted;
    public List<Card> cardsAccepted = new List<Card>();

    public void AcceptCard(Card card){
        if (lastCardAccepted == card) return; 
        lastCardAccepted = card;
        
        cardsAccepted.Add(lastCardAccepted);
    }

    public void ReturnCards(List<Card> returnTo){
        returnTo.AddRange(cardsAccepted);
        cardsAccepted.Clear();
    }
}
