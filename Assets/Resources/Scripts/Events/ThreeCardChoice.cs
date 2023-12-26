using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreeCardChoice : MonoBehaviour
{
    public Card[] cards;
    public Card[] possibleRandomCardRewards;
    void Start()
    {
        cards = new Card[3];
        for (int i = 0; i < 3; i++)
        {
            Card card = PickCard();
            cards[i] = card;
            CardDisplay cardDisplay = transform.GetChild(1 + i).GetComponent<CardDisplay>();
            cardDisplay.card = card;
            cardDisplay.UpdateCardAppearance();
        }
    }

    Card PickCard()
    {
        Card cardToAdd = possibleRandomCardRewards[Random.Range(0, possibleRandomCardRewards.Length)];
        string cardName = cardToAdd.name;
        cardToAdd = Instantiate(cardToAdd).ResetCard();
        cardToAdd.name = cardName;
        for (int x = 0; x < cards.Length; x++)
        {
            if (cards[x] != null && cardToAdd.name == cards[x].name)
            {
                cardToAdd = PickCard();
            }
        }
        return cardToAdd;
    }

}
