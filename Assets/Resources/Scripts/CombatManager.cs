using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{
    public int gamePhase = 0; // 0 - player turn, 1 - enemy turn -> combat phase

    public int playerHealth = 20;
    public int enemyHealth = 20;

    public CardInCombat[] playerCards = new CardInCombat[3];
    public CardInCombat[] enemyCards = new CardInCombat[3];
    public CardSlot[] enemySlots = new CardSlot[3];
    public GameObject[] playerCombatSlots = new GameObject[3];
    public GameObject[] playerBenchSlots = new GameObject[3];

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

        int rand, failEscape = 0;
        do
        {
            failEscape++;
            rand = Random.Range(0, 3);
        } while (enemyCards[rand] != null && failEscape < 20);

        if (failEscape < 20)
        {
            EnemyPlayCard(cardToPlay, enemySlots[rand]);
        }
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
        cardToCreate.transform.SetParent(deck.CardsInCombatParent);
        cardToCreate.transform.localScale = Vector3.one * 0.75f;

        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = card;
        cardInCombat.deck = deck;
        cardInCombat.slot = slot.slot;
        cardInCombat.playerCard = false;
        cardInCombat.benched = false;

        deck.combatManager.enemyCards[slot.slot] = cardInCombat;
    }

    #region Attacks
    //--------------------------------//
    public void DirectHit(CardInCombat card)
    {
        if (card.benched) return;
        card.PerformShortAttackAnimation();

        if (card.playerCard) enemyHealth -= card.card.attack;
        else playerHealth -= card.card.attack;
        //to do
    }

    public void DirectHit(CardInCombat card, int damage)
    {
        if (card.benched) return;
        card.PerformShortAttackAnimation();

        if (card.playerCard) enemyHealth -= damage;
        else playerHealth -= damage;
        //to do
    }
    public void Skirmish(CardInCombat playerCard, CardInCombat enemyCard)
    {
        
        int playerOldHp = playerCard.card.health;
        int enemyOldHp = enemyCard.card.health;

        if (playerCard.benched && enemyCard.benched) return;
        else if (playerCard.benched) { DirectHit(enemyCard); return;}
        else if (enemyCard.benched) {DirectHit(playerCard); return;}

        playerCard.card.health -= enemyCard.card.attack;
        playerCard.lastTypeOfDamage = enemyCard.card.typeOfDamage;
        playerCard.card.lastBattle = new BattleData(playerCard.card, enemyCard.card, playerOldHp, enemyOldHp);
        playerCard.card.ActivateOnTakeDamageEffects(playerCard);
       
        enemyCard.card.health -= playerCard.card.attack;
        enemyCard.lastTypeOfDamage = playerCard.card.typeOfDamage;
        enemyCard.card.lastBattle = new BattleData(enemyCard.card, playerCard.card, enemyOldHp, playerOldHp);
        enemyCard.card.ActivateOnTakeDamageEffects(enemyCard);

        playerCard.card.lastBattle.LogResult();

        playerCard.card.ActivateOnHitEffects(playerCard);
        enemyCard.card.ActivateOnHitEffects(enemyCard);




        playerCard.PerformShortAttackAnimation();
        enemyCard.PerformShortAttackAnimation();
    }
    //--------------------------------//
    #endregion
}
