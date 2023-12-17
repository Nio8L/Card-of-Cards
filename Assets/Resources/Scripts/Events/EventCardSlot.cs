using System.Collections;
using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using Unity.VisualScripting;
using UnityEngine;


public class EventCardSlot : MonoBehaviour
{
    public Card card;

    public CardDisplay cardDisplay;

    public UnityEngine.UI.Image image;

    private void Start() {
        CardSelected cardSelected = cardDisplay.gameObject.AddComponent<CardSelected>();
        cardSelected.eventCardSlot = this;
    }

    public void AddCard(Card cardToAdd){
        card = Instantiate(cardToAdd).ResetCard();
        card.name = cardToAdd.name;

        cardDisplay.card = card;
        cardDisplay.UpdateCardAppearance();
        cardDisplay.gameObject.SetActive(true);

        image.enabled = false;

        MapManager.mapManager.mapDeck.RemoveCard(cardToAdd);

        MapManager.mapManager.deckDisplay.UpdateDisplay(); 
    }

    public void RemoveCard(){

        MapManager.mapManager.mapDeck.AddCard(card);
        MapManager.mapManager.deckDisplay.UpdateDisplay(); 
        
        card = null;
        image.enabled = true;

        cardDisplay.gameObject.SetActive(false);
    }

    public void UpdateCardAppearance(){

    }
}
