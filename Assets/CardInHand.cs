using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardInHand : MonoBehaviour
{
    public Card card;
    public Deck deck;
    void Start()
    {
        UpdateCardAppearance();
    }

    #region Dragging functions
    //--------------------------------//
    public void OnDrag()
    {
        if (deck.selectedCard != transform) deck.selectedCard = transform; 
    }

    public void OnStopDrag()
    {
        CardSlot cardSlot = CheckForSlot();
        if (cardSlot != null && cardSlot.playerSlot && deck.combatManager.playerCards[cardSlot.slot] == null && deck.energy >= card.cost)
        {
            PlayCard(cardSlot);
            deck.selectedCard = null;
        }
        deck.selectedCard = null;
    }
    //--------------------------------//
    #endregion
    void UpdateCardAppearance()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = card.image;
        //transform.GetChild(2).GetComponent<Image>().sprite = ;
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = card.name;
        transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = card.cost.ToString();
        transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = card.health.ToString();
        transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = card.attack.ToString();
    }

    CardSlot CheckForSlot()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.forward, 100);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "CardSlot")
            {
                return hit.transform.GetComponent<CardSlot>();
            }
        }
        return null;
    }

    public void PlayCard(CardSlot slot)
    {
        GameObject cardToCreate =  Instantiate(deck.cardInCombatPrefab, slot.transform.position, Quaternion.identity);
        cardToCreate.transform.SetParent(deck.canvasTransform);
        cardToCreate.transform.localScale = Vector3.one;

        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = card;
        cardInCombat.deck = deck;

        deck.energy -= card.cost;
        deck.combatManager.playerCards[slot.slot] = cardInCombat;

        Destroy(gameObject);
    }
}
