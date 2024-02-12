using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ExchangeShop : MonoBehaviour, IEvent
{   
    public Card[] price1Choices;
    public Card[] price2Choices;
    public Card[] price3Choices;

    public GameObject[] shopSlots;

    public EventCardSlotHandler cardSlotHandler;

    public GameObject selectedCardBorder;
    public Card selectedCard;
    public GameObject selectedCardDisplay;

    [Header("Buttons")]
    public Button exchangeButton;
    public Button regenerateButton;

    public DeckDisplay deckDisplay;
    void Start()
    {
        //Add the correct cards into the deck display
        deckDisplay.cards = CombatManager.combatManager.deck.cards;

        //Generate offers
        GenerateOffers();
    }

    public void GenerateOffers(){
        for (int i = 0; i < 3; i++)
        {
            if (shopSlots[i] == null) continue;
            Card card = PickCard(i);
            CardDisplay cardDisplay = shopSlots[i].GetComponent<CardDisplay>();
            cardDisplay.card = card;
            cardDisplay.UpdateCardAppearance();
        }
    }

    public void RegenerateOffers(){
        //Generate new offers
        GenerateOffers();

        //Unselect the card and disable the border
        //This is done so when you regenerate the offers there is no selected card
        //If there is a selected card the exchange will break
        selectedCardBorder.SetActive(false);
        selectedCard = null;
        selectedCardDisplay = null;

        //Remove the Lost Soul
        cardSlotHandler.cardSlots[1].DropCard();
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
        selectedCardBorder.SetActive(false);

        selectedCard = null;
        selectedCardDisplay = null;

        deckDisplay.ShowCards(MapManager.mapManager.mapDeck.cards);

        //Leave the event if there are no offered cards left
        if(FindObjectsOfType<CardOffered>().Length == 1){
            LeaveEvent();
        }
    }

    public void SelectOfferedCard(GameObject offeredCard, Card card){
        selectedCardBorder.transform.position = offeredCard.transform.position;
        selectedCardBorder.SetActive(true);

        selectedCard = card;
        selectedCardDisplay = offeredCard;
        
        cardSlotHandler.DisableAllSlots();

        if (offeredCard.GetComponent<CardOffered>().price == 1){
            SelectExchange1();
        }else if (offeredCard.GetComponent<CardOffered>().price == 2){
            SelectExchange2();
        }else{
            SelectExchange3();
        }
    }

    private void SelectExchange1(){
        cardSlotHandler.DisableSlot(0);
        cardSlotHandler.EnableSlot(1);
        cardSlotHandler.DisableSlot(2);
    }

    private void SelectExchange2(){
        cardSlotHandler.EnableSlot(0);
        cardSlotHandler.DisableSlot(1);
        cardSlotHandler.EnableSlot(2);
    }

    private void SelectExchange3(){
        cardSlotHandler.EnableSlot(0);
        cardSlotHandler.EnableSlot(1);
        cardSlotHandler.EnableSlot(2);
    }

    //Should be moved to the MapManager so it can work for all events
    public void LeaveEvent(){
        for(int i = 0; i < cardSlotHandler.allCardSlots.Length; i++){
            cardSlotHandler.allCardSlots[i].RemoveCard();
        }
        
        MapManager.mapManager.currentEvent = null;
        deckDisplay.CloseDisplay();
        
        Destroy(gameObject);
    }

    public void LostSoulCase()
    {
        exchangeButton.gameObject.SetActive(false);
        regenerateButton.gameObject.SetActive(true);

        SelectExchange1();

        for(int i = 0; i < MapManager.mapManager.mapDeck.cards.Count; i++){
            if(MapManager.mapManager.mapDeck.cards[i].name == "LostSoul"){
                cardSlotHandler.cardSlots[1].PlaceCard(MapManager.mapManager.mapDeck.cards[i]);
                break;
            }
        }
    }

    public void RevertLostSoulCase()
    {
        exchangeButton.gameObject.SetActive(true);
        regenerateButton.gameObject.SetActive(false);
    }

    public void SelectCard(CardDisplay card)
    {
        SelectOfferedCard(card.gameObject, card.card);
    }

}
