using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SigilOffer : MonoBehaviour, IEvent
{
    public Sigil[] possibleSigilOffers;

    public SigilDisplay[] offeredSigils;

    public GameObject selectedSigilDisplay;
    public Sigil selectedSigil;

    public EventCardSlotHandler cardSlotHandler;

    public GameObject sigilSelector;

    [Header("Buttons")]
    public Button infuseButton;
    public Button regenerateButton;
    
    private void Start() {
        //Reposition the deck display and open it
        if (!MapManager.mapManager.deckDisplay.deckDisplay.activeSelf)
        {
            MapManager.mapManager.deckDisplay.ShowDeck(4, 250);
            MapManager.mapManager.deckDisplay.canClose = false;
            MapManager.mapManager.deckDisplay.deckDisplay.GetComponent<RectTransform>().localPosition = new Vector3(400, 0, 0);
        }

        GenerateOffers();
    }

    private void GenerateOffers(){
        for(int i = 0; i < offeredSigils.Length; i++){
            Sigil newOfferedSigil;

            //Select a random sigil
            //If the sigil has already been offered generate a new sigil
            do
            {
                newOfferedSigil = PickSigil();
            }while(AlreadyOffered(newOfferedSigil));

            offeredSigils[i].SetSigilDisplay(newOfferedSigil);
        }
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

    //Returns true if the given sigil has already been picked
    private bool AlreadyOffered(Sigil sigil){
        for(int i = 0; i < offeredSigils.Length; i++){
            if(offeredSigils[i].sigil != null && sigil == offeredSigils[i].sigil){
                return true;
            }
        }

        return false;
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
        MapManager.mapManager.deckDisplay.canClose = true;
        MapManager.mapManager.deckDisplay.ShowDeck(4, 250);
        
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
}
