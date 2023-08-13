using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public int gamePhase = 0; // 0 - player turn, 1 - enemy turn -> combat phase

    public CardInCombat[] playerCards = new CardInCombat[3];
    public CardInCombat[] enemyCards = new CardInCombat[3];
    public CardSlot[] enemySlots = new CardSlot[3];
    Deck deck;
    private void Start()
    {
        deck = GetComponent<Deck>();
    }

    #region Game Phases
    //--------------------------------//
    public void StartEnemyTurn()
    {
        gamePhase = 1;
        Card card = deck.randomCardSelection[Random.Range(0, deck.randomCardSelection.Count)];
        Card cardToPlay = Instantiate(card);
        cardToPlay.name = card.name;

        EnemyPlayCard(cardToPlay, enemySlots[Random.Range(0, 3)]);
        StartCombatPhase();
    }
    void StartCombatPhase()
    {

        for (int i = 0; i < 3; i++)
        {
            if (playerCards[i] != null && enemyCards[i] != null) Skirmish(playerCards[i], enemyCards[i]);
            else if (playerCards[i] != null) DirectHit(playerCards[i]);
            else if (enemyCards[i] != null) DirectHit(enemyCards[i]);
        }

        StartPlayerTurn();
    }
    void StartPlayerTurn()
    {
        deck.energy = 3;
        gamePhase = 0;
    }
    //--------------------------------//
    #endregion
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

    #region Attacks
    //--------------------------------//
    void DirectHit(CardInCombat card)
    {
        card.PerformShortAttackAnimation();
        //to do
    }
    void Skirmish(CardInCombat playerCard, CardInCombat enemyCard)
    {
        playerCard.card.health -= enemyCard.card.attack;
        playerCard.lastTypeOfDamage = enemyCard.card.typeOfDamage;

        enemyCard.card.health -= playerCard.card.attack;
        enemyCard.lastTypeOfDamage = playerCard.card.typeOfDamage;

        playerCard.UpdateCardAppearance();
        enemyCard.UpdateCardAppearance();

        playerCard.PerformShortAttackAnimation();
        enemyCard.PerformShortAttackAnimation();
    }
    //--------------------------------//
    #endregion
}
