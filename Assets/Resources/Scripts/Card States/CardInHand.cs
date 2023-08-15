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
        deck.UpdateCardAppearance(transform, card);
    }

    #region Dragging functions
    //--------------------------------//
    public void OnDrag()
    {
        if (deck.selectedCard != transform) deck.selectedCard = transform; 
    }

    public void OnStopDrag()
    {
        BenchSlot benchSlot = CheckForSlot();
        if (benchSlot != null && benchSlot.playerSlot && deck.combatManager.playerCards[benchSlot.slot] == null && deck.energy >= card.cost)
        {
            PlayCard(benchSlot);
        }
        deck.selectedCard = null;
        deck.TidyHand();
    }
    //--------------------------------//
    #endregion
   

    //that is for deck.TidyHand()
    public void GetOnTop(Transform card)
    {
        if (deck.hoveredCard == null) 
        {
            deck.hoveredCard = card;
            deck.TidyHand();
        }
    }

    public void Unselect(Transform card) 
    {
        if (card == deck.hoveredCard)
        {
            deck.hoveredCard.localScale = new Vector2(1,1);
            deck.hoveredCard = null;
            deck.TidyHand();
        }
    }
    //that is for deck.TidyHand()

    BenchSlot CheckForSlot()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.forward, 100);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "BenchSlot")
            {
                return hit.transform.GetComponent<BenchSlot>();
            }
        }
        return null;
    }

    public void PlayCard(BenchSlot slot)
    {
        if (deck.combatManager.gamePhase == 1) return;

        GameObject cardToCreate = Instantiate(deck.cardInCombatPrefab, slot.transform.position, Quaternion.identity);
        cardToCreate.transform.SetParent(deck.CardsInCombatParent);
        cardToCreate.transform.localScale = Vector3.one * 0.75f;

        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = card;
        cardInCombat.deck = deck;
        cardInCombat.slot = slot.slot;

        deck.energy -= card.cost;
        deck.combatManager.playerCards[slot.slot] = cardInCombat;

        //maha go ot deck.cardsInHand
        if (deck.cardsInHand.Contains(gameObject)) deck.cardsInHand.Remove(gameObject);
        Destroy(gameObject);
    }
}
