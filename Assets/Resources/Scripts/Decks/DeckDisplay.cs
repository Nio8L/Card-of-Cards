using System.Collections.Generic;
using UnityEngine;


public class DeckDisplay : MonoBehaviour
{
    public GameObject cardDisplay;
    public List<Card> cards;
    public GameObject deckDisplay;

    private List<GameObject> cardDisplays;

    private void Start() {
        cardDisplays = new();
        cards = new();

        cards = CopyCardList(DeckHolder.deckHolder.cards);      
    }

    private void Update() {
        if(Input.GetKeyUp(KeyCode.D)){
            ShowDeck();
        }    
    }

    private void ShowDeck(){
        cards = CopyCardList(DeckHolder.deckHolder.cards);
        if(cardDisplays.Count > 0){
            foreach(GameObject cardDisplay in cardDisplays){
                Destroy(cardDisplay);
            }
        }
        cardDisplays.Clear();
        
        deckDisplay.SetActive(!deckDisplay.activeSelf);

        foreach (Card card in cards)
        {   
            Debug.Log(transform.position.x);
            GameObject newCardDisplay = Instantiate(cardDisplay, new Vector3(cardDisplays.Count % 5 * 2 - 4,  -cardDisplays.Count / 5 * 3 + 3, transform.position.z), Quaternion.identity);
            newCardDisplay.GetComponent<CardDisplay>().card = card;
            newCardDisplay.transform.SetParent(deckDisplay.transform);
            newCardDisplay.transform.localScale = Vector3.one;

            cardDisplays.Add(newCardDisplay);
        }
    }

    private List<Card> CopyCardList(List<Card> listToCopy) 
    {
        List<Card> returnList = new List<Card>();
        foreach (Card card in listToCopy) returnList.Add(card);
        return returnList;
    }
}
