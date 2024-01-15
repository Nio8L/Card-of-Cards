using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventCardSlotHandler : MonoBehaviour
{
    public EventCardSlot[] cardSlots;
    public EventCardSlot[] allCardSlots;

    public void AddCardOnSlot(int index, Card card){
        cardSlots[index].AddCard(card);
    }

    public void AddCardOnAvailableSlot(Card card){
        foreach (EventCardSlot cardSlot in cardSlots)
        {
            if(cardSlot != null && cardSlot.card == null){
                cardSlot.AddCard(card);
                return;
            }
        }
    }

    public int FilledSlotsAmount(){
        int amount = 0;
        
        for(int i = 0; i < cardSlots.Length; i++){
            if (cardSlots[i] != null){
                if(cardSlots[i].card != null){
                    amount++;
                }
            }
        }

        return amount;
    }

    public void DisableSlot(int index){
        if(cardSlots[index] == null){
            return;
        }

        allCardSlots[index].RemoveCard();

        allCardSlots[index].image.enabled = false;
        cardSlots[index] = null;
    }

    public void EnableSlot(int index){
        cardSlots[index] = allCardSlots[index];
        
        if(cardSlots[index].card != null){
            return;
        }

        cardSlots[index].image.enabled = true;
    }

    public void DisableAllSlots(){
        for(int i = 0; i < cardSlots.Length; i++){
            DisableSlot(i);
        }
    }

    public void SwapCards(int originalIndex, int newIndex){
        if(cardSlots[originalIndex] == null || cardSlots[newIndex] == null){
            return;
        }

        if(cardSlots[originalIndex].card == null){
            return;
        }

        cardSlots[newIndex].PlaceCard(cardSlots[originalIndex].card);
        cardSlots[originalIndex].DropCard();
    }
}
