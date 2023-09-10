using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckDisplay : MonoBehaviour
{
    public GameObject cardDisplay;
    public GameObject deckDisplay;

    public bool canClose = true;

    private List<GameObject> cardDisplays;

    private Deck deck;
    private MapDeck mapDeck;

    private void Start() {
        cardDisplays = new();

        if(SceneManager.GetActiveScene().name == "SampleScene"){
            deck = GameObject.Find("Deck").GetComponent<Deck>();
        }else if(SceneManager.GetActiveScene().name == "Map"){
            mapDeck = GameObject.Find("Deck").GetComponent<MapDeck>();
        }
    }

    private void Update() {
        if(Input.GetKeyUp(KeyCode.D)){
            if(canClose){
                ShowDeck();
            }    
        }
    }

    public void ShowDeck(){
        if(cardDisplays.Count > 0){
            foreach(GameObject cardDisplay in cardDisplays){
                Destroy(cardDisplay);
            }
        }
        cardDisplays.Clear();
        
        deckDisplay.SetActive(!deckDisplay.activeSelf);

        if(deck != null){
            foreach (Card card in deck.cards)
            {   
                GameObject newCardDisplay = Instantiate(cardDisplay, Vector3.zero, Quaternion.identity);
                newCardDisplay.GetComponent<CardDisplay>().card = card;
                newCardDisplay.transform.SetParent(deckDisplay.transform);
                newCardDisplay.transform.localScale = Vector3.one;
                newCardDisplay.transform.localPosition = new Vector3(cardDisplays.Count % 5 * 200,  -cardDisplays.Count / 5 * 270, transform.position.z);
                cardDisplays.Add(newCardDisplay);
            }
        }else{
            foreach (Card card in mapDeck.cards)
            {   
                GameObject newCardDisplay = Instantiate(cardDisplay, Vector3.zero, Quaternion.identity);
                newCardDisplay.GetComponent<CardDisplay>().card = card;
                newCardDisplay.transform.SetParent(deckDisplay.transform);
                newCardDisplay.transform.localScale = Vector3.one;
                newCardDisplay.transform.localPosition = new Vector3(cardDisplays.Count % 5 * 200,  -cardDisplays.Count / 5 * 270, transform.position.z);
                cardDisplays.Add(newCardDisplay);
            }
        }
    }
}
