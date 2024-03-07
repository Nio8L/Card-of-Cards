using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sacrifice : MonoBehaviour, IEvent
{
    public EventCardSlotHandler cardSlotHandler;

    public List<Offer> offers;

    public Sigil blessSigil;

    public Notification eventUsedNotification;
    public Notification fullSoulNotification;

    public DeckDisplay deckDisplay;

    public GameObject offeredCard;

    private int timesUsed = 0;

    [Header("Buttons")]
    public Button sacrificeButton;
    public Button regenerateButton;

    [System.Serializable]
    public class Offer{
        public Card card;
        public int price;
    }

    private void Start() {
        //Add the correct cards into the deck display
        Vector2 point = new Vector2(Camera.main.pixelWidth * 1f/4f, 0);
        deckDisplay = DeckUtilities.CreateDisplay(point, Camera.main.pixelWidth/2, Camera.main.pixelHeight * 0.85f, "EventDisplay");
        deckDisplay.cards = MapManager.mapManager.mapDeck.cards;

    }

    public void SacrificeCards(){
            if(timesUsed == 3){
                //Set timeUsed to 4 so we don't enter this if again
                //This is done so we don't notify and leave multiple times
                timesUsed++;
                
                //Notify that the event cannot be used and leave it
                NotificationManager.notificationManager.NotifyAutoEnd(eventUsedNotification, new Vector3(-470, 250, 0), 2f);
                StartCoroutine(DelayedLeave(3f));
                return;
            }

            //Prevent sacrificing if there are no cards on the slot or the offer is still displayed
            if(cardSlotHandler.cardSlots[0].card == null || offeredCard.activeSelf) return;
            
            if(cardSlotHandler.cardSlots[0].card.name == "Lost Soul"){
                cardSlotHandler.cardSlots[0].DropCard();
                ActivateLostSoulButtons();
                return;
            }

            int value = CalculateCardWorth(cardSlotHandler.cardSlots[0].card);
        
            //Get the worst offer and give it to the player if the value isn't 
            GenerateReward(offers[^1].card);
            for(int i = 0; i < offers.Count; i++){
                if(value >= offers[i].price){
                    GenerateReward(offers[i].card);
                    break;
                }
            }
            
            cardSlotHandler.cardSlots[0].DropCard();
            StartCoroutine(RecieveReward(1f));
            timesUsed++;
    }

    //Add the generated reward to the player deck afer a delay
    IEnumerator RecieveReward(float delay){
        yield return new WaitForSeconds(delay);

        MapManager.mapManager.mapDeck.cards.Add(GetReward());
        DeckUtilities.UpdateAllDisplays();
    }

    //Leave the event after a delay
    IEnumerator DelayedLeave(float delay){
        yield return new WaitForSeconds(delay);
        LeaveEvent();
    }

    //Set the card of the card display to the generated reward
    public void GenerateReward(Card card){
        string cardName = card.name;
        card = Instantiate(card).ResetCard();
        card.name = cardName;
        
        CardDisplay cardDisplay = offeredCard.GetComponent<CardDisplay>();
        cardDisplay.card = card;
        cardDisplay.UpdateCardAppearance();
        cardDisplay.gameObject.SetActive(true);
    }

    //Get the reward and disable the display
    public Card GetReward(){
        CardDisplay cardDisplay = offeredCard.GetComponent<CardDisplay>();
        cardDisplay.gameObject.SetActive(false);
        return cardDisplay.card;
    }

    //Returns the value of a given card
    public int CalculateCardWorth(Card card){
        int worth = 0;
        
        worth += card.defaultAttack;
        worth += card.maxHealth;
        worth += card.cost * 2;
        
        foreach(Sigil sigil in card.sigils){
            if(sigil.negative){
                worth -= 3;
            }else{
                worth += 2;
            }
        }

        return worth;
    }
    
    public void LeaveEvent(){
        if(offeredCard.activeSelf) return;
        
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

    public void Bless(){
        Card blessedCard = cardSlotHandler.cardSlots[0].card;
        
        if(blessedCard == null) return;

        if(blessedCard.sigils.Count == 3) {
            NotificationManager.notificationManager.NotifyAutoEnd(fullSoulNotification, new Vector3(-470, 250, 0), 2f);
            return;
        }
        blessedCard.sigils.Add(blessSigil);
        cardSlotHandler.cardSlots[0].RemoveCard();

        StartCoroutine(DelayedLeave(1f));
    }

    private void ActivateLostSoulButtons(){
        sacrificeButton.gameObject.SetActive(false);
        regenerateButton.gameObject.SetActive(true);
    }

    public void LostSoulCase()
    {
        //This event implements the lost soul case by submitting a lost soul card
    }

    public void RevertLostSoulCase()
    {
        //This event doesn't implement the revert lost soul case
    }

    public void SelectCard(CardDisplay card)
    {
        //This event has no selectable cards
    }
}
