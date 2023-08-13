using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public CardInCombat[] playerCards = new CardInCombat[3];
    public CardInCombat[] enemyCards = new CardInCombat[3];
    public CardSlot[] slots = new CardSlot[3];
    Deck deck;
    private void Start()
    {
        deck = GetComponent<Deck>();
    }

    public void EnemyPlayCard(Card card, CardSlot slot)
    {
        GameObject cardToCreate = Instantiate(deck.cardInCombatPrefab, slot.transform.position, Quaternion.identity);
        cardToCreate.transform.SetParent(deck.canvasTransform);
        cardToCreate.transform.localScale = Vector3.one;

        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = card;
        cardInCombat.deck = deck;
        cardInCombat.playerCard = false;

        deck.combatManager.enemyCards[slot.slot] = cardInCombat;
    }

    public void StartEnemyTurn()
    {
        Card cardToPlay = deck.randomCardSelection[Random.Range(0, deck.randomCardSelection.Count)];
        EnemyPlayCard(cardToPlay, slots[Random.Range(0, 3)]);
        StartCombatPhase();
    }
    void StartCombatPhase()
    {
        StartPlayerTurn();
    }
    void StartPlayerTurn()
    {
        deck.energy = 3;
        deck.gamePhase = 0;
    }
}
