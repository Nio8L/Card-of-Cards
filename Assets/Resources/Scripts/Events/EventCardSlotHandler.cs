using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCardSlotHandler : MonoBehaviour
{
    public EventCardSlot[] cardSlots;

    public void AddCardOnSlot(int index, Card card){
        cardSlots[index].AddCard(card);
    }

    public void AddCardOnAvailableSlot(Card card){
        foreach (EventCardSlot cardSlot in cardSlots)
        {
            if(cardSlot.card == null){
                cardSlot.AddCard(card);
                return;
            }
        }
    }
}
