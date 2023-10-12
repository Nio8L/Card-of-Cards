using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExchangeShop : MonoBehaviour
{
    public int tokens;
    public Card[] price1Choices;
    public Card[] price2Choices;
    public Card[] price3Choices;
    void Start()
    {
        if (!MapManager.mapManager.deckDisplay.deckDisplay.activeSelf)
        {
            MapManager.mapManager.deckDisplay.deckDisplay.GetComponent<RectTransform>().localPosition = new Vector3(450, 0, 0);
            MapManager.mapManager.deckDisplay.ShowDeck();
        }

        for (int i = 0; i < 3; i++)
        {
            Card card = PickCard(i);
            CardDisplay cardDisplay = transform.GetChild(1).GetChild(i).GetComponent<CardDisplay>();
            cardDisplay.card = card;
            cardDisplay.UpdateCardAppearance();
        }
    }

    Card PickCard(int value)
    {
        Card cardToAdd;
        if (value == 0)      cardToAdd = price1Choices[Random.Range(0, price1Choices.Length)];
        else if (value == 1) cardToAdd = price2Choices[Random.Range(0, price2Choices.Length)];
        else                 cardToAdd = price3Choices[Random.Range(0, price3Choices.Length)];


        string cardName = cardToAdd.name;
        cardToAdd = Instantiate(cardToAdd).ResetCard();
        cardToAdd.name = cardName;
        return cardToAdd;
    }
}
