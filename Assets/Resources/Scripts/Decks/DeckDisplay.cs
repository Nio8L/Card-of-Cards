using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckDisplay : MonoBehaviour
{
    public int defaultDisplacement = 250;
    public int defaultNumberOfCardsInARow = 5;

    public GameObject cardDisplay;
    public GameObject deckDisplay;
    
    public CanvasGroup canvasGroup; 

    public bool canClose = true;

    public List<GameObject> cardDisplays;

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
                ShowDeck(defaultNumberOfCardsInARow, defaultDisplacement);
            }    
        }
    }

    public void ShowDeck(int numOfCardsInARow, float xDisplacement)
    {
        SoundManager.soundManager.Play("DeckDisplaySlide");
        if (deckDisplay.activeSelf)
        {
            if (SceneManager.GetActiveScene().name == "SampleScene")
            {
                if (cardDisplays.Count != deck.cards.Count)
                {
                    StartCoroutine(ShowDisplay(0.4f, "deck", numOfCardsInARow, xDisplacement));
                }
            }

            AnimationUtilities.ChangeCanvasAlpha(transform.parent, 0.3f, 0, 0);

            StartCoroutine(CloseDisplay(0.3f));

        }
        else
        {
            if (deck != null)
            {

                foreach (Card card in deck.cards)
                {
                    GameObject newCardDisplay = Instantiate(cardDisplay, Vector3.zero, Quaternion.identity);
                    newCardDisplay.GetComponent<CardDisplay>().card = card;
                    newCardDisplay.transform.SetParent(deckDisplay.transform);
                    newCardDisplay.transform.localScale = Vector3.one;
                    newCardDisplay.transform.localPosition = new Vector3(cardDisplays.Count % numOfCardsInARow * xDisplacement, -cardDisplays.Count / numOfCardsInARow * 270, transform.position.z);
                    cardDisplays.Add(newCardDisplay);
                }
            }
            else
            {
                foreach (Card card in mapDeck.cards)
                {
                    GameObject newCardDisplay = Instantiate(cardDisplay, Vector3.zero, Quaternion.identity);
                    newCardDisplay.GetComponent<CardDisplay>().card = card;
                    newCardDisplay.transform.SetParent(deckDisplay.transform);
                    newCardDisplay.transform.localScale = Vector3.one;
                    newCardDisplay.transform.localPosition = new Vector3(cardDisplays.Count % numOfCardsInARow * xDisplacement, -cardDisplays.Count / numOfCardsInARow * 270, transform.position.z);
                    cardDisplays.Add(newCardDisplay);
                }
            }

            deckDisplay.SetActive(true);
            AnimationUtilities.ChangeCanvasAlpha(transform.parent, 0.3f, 0, 1);
        }
    }

    public void UpdateDisplay(int numberOfCardsInARow, int xDisplacement){
        foreach (GameObject cardDisplay in cardDisplays)
        {
            Destroy(cardDisplay);
        }

        cardDisplays.Clear();

        if(deck != null){ 
            foreach (Card card in deck.cards)
            {   
                GameObject newCardDisplay = Instantiate(cardDisplay, Vector3.zero, Quaternion.identity);
                newCardDisplay.GetComponent<CardDisplay>().card = card;
                newCardDisplay.transform.SetParent(deckDisplay.transform);
                newCardDisplay.transform.localScale = Vector3.one;
                newCardDisplay.transform.localPosition = new Vector3(cardDisplays.Count % numberOfCardsInARow * xDisplacement,  -cardDisplays.Count / numberOfCardsInARow * 270, transform.position.z);
                cardDisplays.Add(newCardDisplay);
            }
        }else{
            foreach (Card card in mapDeck.cards)
            {   
                GameObject newCardDisplay = Instantiate(cardDisplay, Vector3.zero, Quaternion.identity);
                newCardDisplay.GetComponent<CardDisplay>().card = card;
                newCardDisplay.transform.SetParent(deckDisplay.transform);
                newCardDisplay.transform.localScale = Vector3.one;
                newCardDisplay.transform.localPosition = new Vector3(cardDisplays.Count % numberOfCardsInARow * xDisplacement,  -cardDisplays.Count / numberOfCardsInARow * 270, transform.position.z);
                cardDisplays.Add(newCardDisplay);
            }
        }
    }

    public void ShowDrawPile(){
        if(deck.drawPile.Count > 0){
            SoundManager.soundManager.Play("DeckDisplaySlide");
            if(deckDisplay.activeSelf){
                
                if(cardDisplays.Count != deck.drawPile.Count){
                    StartCoroutine(ShowDisplay(0.4f, "draw", 0, 0));
                }

                AnimationUtilities.ChangeCanvasAlpha(transform.parent, 0.3f, 0, 0);

                StartCoroutine(CloseDisplay(0.3f));
            }
            else
            {
                foreach (Card card in deck.drawPile)
                {   
                    GameObject newCardDisplay = Instantiate(cardDisplay, Vector3.zero, Quaternion.identity);
                    newCardDisplay.GetComponent<CardDisplay>().card = card;
                    newCardDisplay.transform.SetParent(deckDisplay.transform);
                    newCardDisplay.transform.localScale = Vector3.one;
                    newCardDisplay.transform.localPosition = new Vector3(cardDisplays.Count % 5 * defaultDisplacement,  -cardDisplays.Count / 5 * 270, transform.position.z);
                    cardDisplays.Add(newCardDisplay);
                }
                deckDisplay.SetActive(true);
                AnimationUtilities.ChangeCanvasAlpha(transform.parent, 0.3f, 0, 1);
            }
        }
    }

    public void ShowDiscardPile(){
        if(deck.discardPile.Count > 0){
            SoundManager.soundManager.Play("DeckDisplaySlide");
            if(deckDisplay.activeSelf){
                
                if(cardDisplays.Count != deck.discardPile.Count){
                    StartCoroutine(ShowDisplay(0.4f, "discard", 0, 0));
                }

                AnimationUtilities.ChangeCanvasAlpha(transform.parent, 0.3f, 0, 0);

                StartCoroutine(CloseDisplay(0.3f));
            }else{
                foreach (Card card in deck.discardPile)
                {   
                    GameObject newCardDisplay = Instantiate(cardDisplay, Vector3.zero, Quaternion.identity);
                    newCardDisplay.GetComponent<CardDisplay>().card = card;
                    newCardDisplay.transform.SetParent(deckDisplay.transform);
                    newCardDisplay.transform.localScale = Vector3.one;
                    newCardDisplay.transform.localPosition = new Vector3(cardDisplays.Count % 5 * defaultDisplacement,  -cardDisplays.Count / 5 * 270, transform.position.z);
                    cardDisplays.Add(newCardDisplay);
                }
            deckDisplay.SetActive(true);
            AnimationUtilities.ChangeCanvasAlpha(transform.parent, 0.3f, 0, 1);
            }
        }
    }

    private IEnumerator CloseDisplay(float delay){
        yield return new WaitForSeconds(delay);
        deckDisplay.SetActive(false);
        
        if(cardDisplays.Count > 0){
            foreach(GameObject cardDisplay in cardDisplays){
                Destroy(cardDisplay);
            }
        }   
        cardDisplays.Clear();
    }

    private IEnumerator ShowDisplay(float delay, string type, int numOfCardsInARow, float xDisplacement){
        yield return new WaitForSeconds(delay);

        if(type == "deck"){
            ShowDeck(numOfCardsInARow, xDisplacement);
        }else if(type == "draw"){
            ShowDrawPile();
        }else{
            ShowDiscardPile();
        }
    }

}
