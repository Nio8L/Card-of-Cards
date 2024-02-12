using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Graveyard : MonoBehaviour, IEvent
{
    public List<Card> offers;

    [Header("Card offer components")]
    public GameObject cardOfferHolder;
    public GameObject selectedCardBorder;
    public GameObject selectedCardDisplay;
    public Card selectedCard;

    public EventCardSlotHandler cardSlotHandler;

    [Header("Buttons")]
    public Button healButton;
    public Button getCardOffersButton;
    public Button pickCardButton;


    private bool healed = false;
    private bool canHeal = true; // Tracks whether the event can heal. The event can't heal after it has generated card offers

    private void Start() {
        //Reposition the deck display and open it
        //aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
    }

    public void HealCard(){
        //If we have already healed a card return, so we don't heal two or more cards
        if(healed) return;
        
        //Heal the card
        cardSlotHandler.cardSlots[0].card.AcceptLostSoul();
        cardSlotHandler.cardSlots[0].cardDisplay.UpdateCardAppearance();

        //Prevent more than one heal
        healed = true;

        //Play the healing animation
        AnimationUtilities.LostSoulAnimation(cardSlotHandler.cardSlots[0].transform);

        //Leave the event after the animation has finished
        StartCoroutine(DelayedLeave(2f));        
    }

    //Generates the card offers
    public void GenerateCardOffers(){

        //Store the already generated card offers so we don't offer the same card more than once
        List<Card> generatedCards = new();

        //Generate cards
        for(int i = 0; i < 3; i++){
            Card originalCard = PickCard(generatedCards);
            Card card = Instantiate(originalCard).ResetCard();
            card.name = originalCard.name;
            cardOfferHolder.transform.GetChild(i).GetComponent<CardDisplay>().card = card;
            cardOfferHolder.transform.GetChild(i).GetComponent<CardDisplay>().UpdateCardAppearance();
        }

        //Show the cards
        cardOfferHolder.SetActive(true);

        //Consume the lost soul
        cardSlotHandler.cardSlots[0].DropCard();

        //Prevent the event from being able to heal
        canHeal = false;

        getCardOffersButton.gameObject.SetActive(false);
        pickCardButton.gameObject.SetActive(true);
    }

    //Picks a card without repeating the cards in alreadyOffered
    private Card PickCard(List<Card> alreadyOffered){
        Card card = offers[Random.Range(0, offers.Count)];
        
        if(!alreadyOffered.Contains(card)){
            alreadyOffered.Add(card);
            return card;
        }

        do{
            card = offers[Random.Range(0, offers.Count)];
        }while(alreadyOffered.Contains(card));

        return card;
    }

    public void AddCard(){
        if(selectedCard == null) return;

        MapManager.mapManager.mapDeck.AddCard(selectedCard);
        Destroy(selectedCardDisplay);
        selectedCardBorder.SetActive(false);

        //aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa

        LeaveEvent();
    }

    private IEnumerator DelayedLeave(float delay){
        yield return new WaitForSeconds(delay);

        cardSlotHandler.cardSlots[0].RemoveCard();
        LeaveEvent();
    }

    public void LeaveEvent(){
        for(int i = 0; i < cardSlotHandler.allCardSlots.Length; i++){
            cardSlotHandler.allCardSlots[i].RemoveCard();
        }
        
        MapManager.mapManager.currentEvent = null;
        //aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa
        
        Destroy(gameObject);
    }

    public void LostSoulCase()
    {
        healButton.gameObject.SetActive(false);
        getCardOffersButton.gameObject.SetActive(true);
    }

    public void RevertLostSoulCase()
    {
        //Prevent the event from being able to heal after it has generated card offers
        if(!canHeal) return;

        healButton.gameObject.SetActive(true);
        getCardOffersButton.gameObject.SetActive(false);
    }

    public void SelectCard(CardDisplay card)
    {
        selectedCardDisplay = card.gameObject;

        selectedCardBorder.transform.position = card.transform.position;
        selectedCardBorder.SetActive(true);

        selectedCard = card.card;
    }
}
