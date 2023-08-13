using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> cards = new();
    public List<Card> cardsInHand = new();
    GameObject cardInHandPrefab;
    public GameObject cardInCombatPrefab;
    public Transform canvasTransform;

    public Transform selectedCard;

    private void Start()
    {
        cardInHandPrefab = Resources.Load<GameObject>("Prefabs/CardInHand");
        cardInCombatPrefab = Resources.Load<GameObject>("Prefabs/CardInCombat");
        canvasTransform = GameObject.Find("Canvas").transform;
    }

    private void Update()
    {
        if (selectedCard != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            selectedCard.position = new Vector3(mousePosition.x, mousePosition.y, 0);
        }

    }
    public void AddCard(Card card){
        cards.Add(card);
        PrintDeck();
    }

    //FOR TESTING
    public List<Card> randomCardSelection = new();
    public void AddCard(){
        AddCard(randomCardSelection[Random.Range(0, randomCardSelection.Count)]);
    }

    public void RemoveCard(Card card){
        cards.Remove(card);
    }

    public void RemoveCard(){
        cards.RemoveAt(cards.Count - 1);
        PrintDeck();
    }

    public void PrintDeck(){
        string cardsInDeck = "";
        foreach (Card card in cards)
        {
            cardsInDeck += card.name;
            cardsInDeck += ", ";
        }
        Debug.Log(cardsInDeck);
    }

    // Teglene na karti v //  
    public void DrawCard()
    {
        cardsInHand.Add(cards[0]);
        var card = Instantiate(cardInHandPrefab, new Vector3(cardsInHand.Count * 2, -3.5f, 0), Quaternion.identity);
        card.transform.SetParent(canvasTransform);
        card.transform.localScale = Vector3.one;
        CardInHand cardInHan = card.GetComponent<CardInHand>();
        cardInHan.card = cardsInHand[cardsInHand.Count - 1];
        cardInHan.deck = this;
        cards.RemoveAt(0);
    }

    //Shuffle the deck using the Fisher-Yates shuffle
    System.Random random = new System.Random();
    public void Shuffle(){
        for(int i = cards.Count - 1; i > 0; i--){
            int k = random.Next(i + 1);
            (cards[i], cards[k]) = (cards[k], cards[i]);
        }
    }
}
