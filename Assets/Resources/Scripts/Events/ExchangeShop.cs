using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ExchangeShop : MonoBehaviour
{   
    public Card[] price1Choices;
    public Card[] price2Choices;
    public Card[] price3Choices;

    public EventCardSlotHandler cardSlotHandler;

    public GameObject SelectedCardBorder;
    public Card selectedCard;
    public GameObject selectedCardDisplay;

    void Start()
    {
        //Reposition the deck display and open it
        if (!MapManager.mapManager.deckDisplay.deckDisplay.activeSelf)
        {
            MapManager.mapManager.deckDisplay.deckDisplay.GetComponent<RectTransform>().localPosition = new Vector3(450, 0, 0);
            MapManager.mapManager.deckDisplay.ShowDeck();
            MapManager.mapManager.deckDisplay.canClose = false;
        }

        for (int i = 0; i < 3; i++)
        {
            Card card = PickCard(i);
            CardDisplay cardDisplay = transform.GetChild(1).GetChild(i).GetComponent<CardDisplay>();
            cardDisplay.card = card;
            cardDisplay.UpdateCardAppearance();
        }
    }

    Card PickCard(int value)
    {
        Card cardToAdd;
        if (value == 0)      cardToAdd = price1Choices[UnityEngine.Random.Range(0, price1Choices.Length)];
        else if (value == 1) cardToAdd = price2Choices[UnityEngine.Random.Range(0, price2Choices.Length)];
        else                 cardToAdd = price3Choices[UnityEngine.Random.Range(0, price3Choices.Length)];


        string cardName = cardToAdd.name;
        cardToAdd = Instantiate(cardToAdd).ResetCard();
        cardToAdd.name = cardName;
        return cardToAdd;
    }

    public void Exchange(){
        if(selectedCardDisplay == null){
            return;
        }

        int price = selectedCardDisplay.GetComponent<CardOffered>().price;

        if(price > cardSlotHandler.FilledSlotsAmount()){
            return;
        }
        
        for (int i = 0; i < cardSlotHandler.cardSlots.Length; i++){
            if (cardSlotHandler.cardSlots[i] != null){
                /*if (cardSlotHandler.cardSlots[i].card == null){
                    price++; 
                    continue;
                }*/
             
                cardSlotHandler.cardSlots[i].card = null;
                cardSlotHandler.cardSlots[i].cardDisplay.gameObject.SetActive(false);
                cardSlotHandler.cardSlots[i].image.enabled = true;
            }
        }

        MapManager.mapManager.mapDeck.AddCard(selectedCard);
        Destroy(selectedCardDisplay);
        SelectedCardBorder.SetActive(false);

        selectedCard = null;
        selectedCardDisplay = null;

        MapManager.mapManager.deckDisplay.UpdateDisplay();

        //Leave the event if there are no offered cards left
        if(FindObjectsOfType<CardOffered>().Length == 1){
            LeaveEvent();
        }
    }

    public void SelectOfferedCard(GameObject offeredCard, Card card){
        SelectedCardBorder.transform.position = offeredCard.transform.position;
        SelectedCardBorder.SetActive(true);

        selectedCard = card;
        selectedCardDisplay = offeredCard;
        
        cardSlotHandler.DisableAllSlots();

        if (offeredCard.GetComponent<CardOffered>().price == 1){
            cardSlotHandler.DisableSlot(0);
            cardSlotHandler.EnableSlot(1);
            cardSlotHandler.DisableSlot(2);
        }else if (offeredCard.GetComponent<CardOffered>().price == 2){
            cardSlotHandler.EnableSlot(0);
            cardSlotHandler.DisableSlot(1);
            cardSlotHandler.EnableSlot(2);
        }else{
            cardSlotHandler.EnableSlot(0);
            cardSlotHandler.EnableSlot(1);
            cardSlotHandler.EnableSlot(2);
        }
    }

    //Should be moved to the MapManager so it can work for all events
    public void LeaveEvent(){
        for(int i = 0; i < cardSlotHandler.allCardSlots.Length; i++){
            cardSlotHandler.allCardSlots[i].RemoveCard();
        }
        
        MapManager.mapManager.currentEvent = null;
        MapManager.mapManager.deckDisplay.canClose = true;
        MapManager.mapManager.deckDisplay.ShowDeck();
        
        Destroy(gameObject);
    }
}
