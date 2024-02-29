using System.Collections.Generic;
using System.Linq;
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

    //Adds a card to the slot without requiering a deck
    public void PlaceCard(Card cardToPlace){
        card = Instantiate(cardToPlace).ResetCard();
        card.name = cardToPlace.name;

        cardDisplay.card= card;
        cardDisplay.UpdateCardAppearance();
        cardDisplay.gameObject.SetActive(true);

        image.enabled = false;
    }

    //Takes a card from the deck and adds it to the slot
    public void AddCard(Card cardToAdd){
        PlaceCard(cardToAdd);
        MapManager.mapManager.mapDeck.RemoveCard(cardToAdd);

        DeckUtilities.UpdateAllDisplays();
    }

    //Removes the card from the slot
    public void DropCard(){
        card = null;
        image.enabled = true;

        cardDisplay.gameObject.SetActive(false);
    }

    //Removes the card from the slot and returns it to the deck
    public void RemoveCard(){
        if(card == null) return;

        MapManager.mapManager.mapDeck.AddCard(card);
        DeckUtilities.UpdateAllDisplays();
        
        if(card.name == "Lost Soul"){
            IEnumerable<IEvent> events = FindObjectsOfType<MonoBehaviour>().OfType<IEvent>();
            foreach(IEvent ievent in events){
                ievent.RevertLostSoulCase();
            }
        }

        DropCard();
        
    }
}
