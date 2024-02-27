using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SigilOffer : MonoBehaviour, IEvent
{
    public Sigil[] possibleSigilOffers;

    public SigilDisplay[] offeredSigils;

    List<Sigil> rerolledSigils = new List<Sigil>();
    

    public GameObject selectedSigilDisplay;
    public Sigil selectedSigil;

    public EventCardSlotHandler cardSlotHandler;

    public GameObject sigilSelector;

    [Header("Buttons")]
    public Button infuseButton;
    public Button regenerateButton;
    public DeckDisplay deckDisplay;
    
    private void Start() {
        //Add the correct cards into the deck display
        Vector2 point = new Vector2(Camera.main.pixelWidth * 1f/4f, 0);
        deckDisplay = DeckUtilities.CreateDisplay(point, Camera.main.pixelWidth/2, Camera.main.pixelHeight * 0.85f, "EventDisplay");
        deckDisplay.cards = MapManager.mapManager.mapDeck.cards;

        GenerateOffers();
    }

    private void GenerateOffers(){
        for(int i = 0; i < offeredSigils.Length; i++){
            Sigil newOfferedSigil = possibleSigilOffers[0];

            //Select a random sigil
            //If the sigil has already been offered generate a new sigil
            if (possibleSigilOffers.Length - rerolledSigils.Count < 3) rerolledSigils.Clear();

            do
            {
                newOfferedSigil = PickSigil();
            }while(AlreadyOffered(newOfferedSigil));

            rerolledSigils.Add(newOfferedSigil);
            offeredSigils[i].SetSigilDisplay(newOfferedSigil);
        }
    }

    //Returns true if the given sigil has already been picked
    private bool AlreadyOffered(Sigil sigil){
        for(int i = 0; i < rerolledSigils.Count; i++){
            if(sigil == rerolledSigils[i]){
                return true;
            }
        }

        return false;
    }

    public void RegenerateOffers(){
        //Generate new offers
        GenerateOffers();

        //Unselect the sigil and disable the selector
        //This is done so when you regenerate the offers there is no selected sigil
        //If there is a selected sigil the infusion will break
        selectedSigilDisplay = null;
        selectedSigil = null;
        sigilSelector.SetActive(false);

        //Remove the Lost Soul
        cardSlotHandler.cardSlots[0].DropCard();
    }

    //Picks a random sigil
    private Sigil PickSigil(){
        return possibleSigilOffers[Random.Range(0, possibleSigilOffers.Length)];
    }

    public void SelectSigil(SigilDisplay sigilDisplay){
        selectedSigilDisplay = sigilDisplay.gameObject;
        selectedSigil = sigilDisplay.sigil;

        sigilSelector.transform.position =  sigilDisplay.transform.position;
        sigilSelector.SetActive(true);
    }

    public void RemoveSigil(GameObject sigilDisplay){
        Destroy(sigilDisplay);
        sigilSelector.SetActive(false);
    }

    public void Infuse(){
        //Prevent infusing if there is no selected sigil or selected card
        if(selectedSigil == null || cardSlotHandler.cardSlots[0].card == null){
            return;
        }

        //Prevent infusing if the selected card has no available sigil slots
        if(cardSlotHandler.cardSlots[0].card.sigils.Count == 3) return;
        
        //Remove the selected sigil and return the card
        RemoveSigil(selectedSigilDisplay);
        cardSlotHandler.cardSlots[0].card.sigils.Add(selectedSigil);
        cardSlotHandler.cardSlots[0].RemoveCard();

        selectedSigilDisplay = null;
        selectedSigil = null;

        LeaveEvent();
    }

    public void LeaveEvent(){
        for(int i = 0; i < cardSlotHandler.allCardSlots.Length; i++){
            cardSlotHandler.allCardSlots[i].RemoveCard();
        }
        MapManager.mapManager.currentEvent = null;
        MapManager.mapManager.eventUsed = true;
        MapManager.mapManager.mapLegend.SetActive(true);
        deckDisplay.CloseDisplay();
        
        Destroy(gameObject);
    }

    public void LostSoulCase()
    {
        infuseButton.gameObject.SetActive(false);
        regenerateButton.gameObject.SetActive(true);

    }

    public void RevertLostSoulCase()
    {
        infuseButton.gameObject.SetActive(true);
        regenerateButton.gameObject.SetActive(false);
    }

    public void SelectCard(CardDisplay card)
    {
        //This event has no selectable cards
    }
}
