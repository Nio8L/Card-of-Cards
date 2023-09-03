using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckDisplay : MonoBehaviour
{
    public GameObject cardDisplay;
    public List<Card> cards;
    public GameObject deckDisplay;

    private List<GameObject> cardDisplays;

    private void Start() {
        cardDisplays = new();

        if(SceneManager.GetActiveScene().name == "SampleScene"){
            Deck deck = GameObject.Find("Deck").GetComponent<Deck>();
            cards = deck.CopyCardList(deck.cards);
        }else if(SceneManager.GetActiveScene().name == "Map"){
            MapDeck deck = GameObject.Find("Deck").GetComponent<MapDeck>();
            cards = deck.CopyCardList(deck.cards);
        }
    }

    private void Update() {
        if(Input.GetKeyUp(KeyCode.D)){
            ShowDeck();
        }    
    }

    private void ShowDeck(){
        if(cardDisplays.Count > 0){
            foreach(GameObject cardDisplay in cardDisplays){
                Destroy(cardDisplay);
            }
        }
        cardDisplays.Clear();
        
        deckDisplay.SetActive(!deckDisplay.activeSelf);

        foreach (Card card in cards)
        {   
            GameObject newCardDisplay = Instantiate(cardDisplay, new Vector3(cardDisplays.Count % 5 * 2 - 4,  -cardDisplays.Count / 5 * 3 + 3, transform.position.z), Quaternion.identity);
            newCardDisplay.GetComponent<CardDisplay>().card = card;
            newCardDisplay.transform.SetParent(deckDisplay.transform);
            newCardDisplay.transform.localScale = Vector3.one;

            cardDisplays.Add(newCardDisplay);
        }
    }
}
