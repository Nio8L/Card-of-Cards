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

    public List<Card> rerolledCards = new List<Card>();
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
        Vector2 point = new Vector2(Camera.main.pixelWidth * 1f/4f, 0);
        deckDisplay = DeckUtilities.CreateDisplay(point, Camera.main.pixelWidth/2, Camera.main.pixelHeight * 0.85f, "EventDisplay");
        deckDisplay.cards = MapManager.mapManager.mapDeck.cards;

        //Generate offers
        GenerateOffers();
        //Auto select offer 1
        SelectOfferedCard(shopSlots[0], shopSlots[0].GetComponent<CardDisplay>().card);
    }

    public void GenerateOffers(){
        //Clear the list of rerolled cards
        int lowestAmount = Math.Min(Math.Min(price1Choices.Length, price2Choices.Length), price3Choices.Length);
        if (lowestAmount - rerolledCards.Count / 3 < 1) rerolledCards.Clear();

        for (int i = 0; i < 3; i++)
        {
            if (shopSlots[i] == null) continue;
            Card card = PickCard(i);
            CardDisplay cardDisplay = shopSlots[i].GetComponent<CardDisplay>();
            cardDisplay.card = card;
            cardDisplay.UpdateCardAppearance();

            rerolledCards.Add(card);
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

        do{
            if (value == 0)      cardToAdd = price1Choices[UnityEngine.Random.Range(0, price1Choices.Length)];
            else if (value == 1) cardToAdd = price2Choices[UnityEngine.Random.Range(0, price2Choices.Length)];
            else                 cardToAdd = price3Choices[UnityEngine.Random.Range(0, price3Choices.Length)];
        }while(AlreadyOffered(cardToAdd));

        string cardName = cardToAdd.name;
        cardToAdd = Instantiate(cardToAdd).ResetCard();
        cardToAdd.name = cardName;
        return cardToAdd;
    }
    bool AlreadyOffered(Card card){
        for(int i = 0; i < rerolledCards.Count; i++){
            if(card.name == rerolledCards[i].name){
                return true;
            }
        }
        return false;
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

    public void LeaveEvent(){
        for(int i = 0; i < cardSlotHandler.allCardSlots.Length; i++){
            cardSlotHandler.allCardSlots[i].RemoveCard();
        }
        
        MapManager.mapManager.currentEvent = null;
        MapManager.mapManager.eventUsed = true;
        MapManager.mapManager.mapLegend.SetActive(true);
        MapManager.mapManager.playerHP.SetActive(true);

        deckDisplay.CloseDisplay();
        
        Destroy(gameObject);
    }

    public void LostSoulCase()
    {
        exchangeButton.gameObject.SetActive(false);
        regenerateButton.gameObject.SetActive(true);

        cardSlotHandler.DisableAllSlots();

        SelectExchange1();

        /*for(int i = 0; i < MapManager.mapManager.mapDeck.cards.Count; i++){
            if(MapManager.mapManager.mapDeck.cards[i].name == "Lost Soul"){
                cardSlotHandler.cardSlots[1].PlaceCard(MapManager.mapManager.mapDeck.cards[i]);
                break;
            }
        }*/
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
