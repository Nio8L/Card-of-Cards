using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Deck : MonoBehaviour
{
    public List<Card> cards = new();

    public void AddCard(Card card){
        Card newCard = Instantiate(card);
        newCard.name = card.name;
        cards.Add(newCard);
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

    public TMP_Text Text;
    public void PrintDeck(){
        string cardsInDeck = "";
        foreach (Card card in cards)
        {
            cardsInDeck += card.name + " - Health: " + card.hp.ToString() + ", Damage: " + card.attack.ToString() + "\n";
        }
        //Debug.Log(cardsInDeck);
        Text.text = cardsInDeck;
    }


    //Shuffle the deck using the Fisher-Yates shuffle
    System.Random random = new System.Random();
    public void Shuffle(){
        for(int i = cards.Count - 1; i > 0; i--){
            int k = random.Next(i + 1);
            (cards[i], cards[k]) = (cards[k], cards[i]);
        }
        PrintDeck();
    }
}
