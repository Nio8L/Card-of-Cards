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

    public GameObject[] enemyCombatSlots = new GameObject[3];
    public GameObject[] enemyBenchSlots = new GameObject[3];

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
            EnemyPlayCard(cardToPlay, enemyBenchSlots[rand], rand);
        }

        foreach (CardInCombat cardToUnbench in enemyCards)
        {
            if (cardToUnbench != null)
            {
                cardToUnbench.benched = false;
                cardToUnbench.PutOnOrOffTheBenchEnemyCards();
            }
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
        deck.DiscardHand();
        deck.energy = 3;
        deck.DrawCard(5);
        gamePhase = 0;
    }
    //--------------------------------//
    #endregion
    public void EnemyPlayCard(Card card, GameObject slot, int slotNumber)
    {
        GameObject cardToCreate = Instantiate(deck.cardInCombatPrefab, slot.transform.position, Quaternion.identity);
        cardToCreate.transform.SetParent(deck.CardsInCombatParent);
        cardToCreate.transform.localScale = Vector3.one * 0.75f;

        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = card;
        cardInCombat.deck = deck;
        cardInCombat.slot = slotNumber;
        cardInCombat.playerCard = false;
        cardInCombat.benched = true;

        enemyCards[slotNumber] = cardInCombat;
    }

    #region Attacks
    //--------------------------------//
    public void DirectHit(CardInCombat card)
    {
        if (card.benched) return;
        card.PerformShortAttackAnimation();
        Debug.Log("Direct hit by " + card.card.name);

        if (card.playerCard) enemyHealth -= card.card.attack;
        else playerHealth -= card.card.attack;
        //to do
    }

    public void Skirmish(CardInCombat playerCard, CardInCombat enemyCard)
    {
        
        if (playerCard.benched && enemyCard.benched) return;
        else if (playerCard.benched) { DirectHit(enemyCard); return;}
        else if (enemyCard.benched) {DirectHit(playerCard); return;}

        playerCard.card.ActivateOnHitEffects(playerCard);
        enemyCard.card.ActivateOnHitEffects(enemyCard);

        playerCard.card.health -= enemyCard.card.attack;
        playerCard.lastTypeOfDamage = enemyCard.card.typeOfDamage;
        playerCard.card.ActivateOnTakeDamageEffects(playerCard);
       
        enemyCard.card.health -= playerCard.card.attack;
        enemyCard.lastTypeOfDamage = playerCard.card.typeOfDamage;
        enemyCard.card.ActivateOnTakeDamageEffects(enemyCard);


        playerCard.PerformShortAttackAnimation();
        enemyCard.PerformShortAttackAnimation();
    }
    //--------------------------------//
    #endregion
}
