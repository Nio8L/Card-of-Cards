using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardAcceptor : MonoBehaviour
{
    public Card lastCardAccepted;
    public List<Card> cardsAccepted = new List<Card>();

    public void AcceptCard(Card card){
        lastCardAccepted = card;
        
        cardsAccepted.Add(lastCardAccepted);
    }

    public void ReturnCards(){
        
    }
}
