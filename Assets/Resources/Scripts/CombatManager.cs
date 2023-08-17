using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatManager : MonoBehaviour
{

    public EnemyAI enemy;

    public int gamePhase = 0; // 0 - player turn, 1 - enemy turn -> combat phase

    public int playerHealth = 20;
    public int enemyHealth = 20;

    public CardInCombat[] playerCards = new CardInCombat[3];
    public CardInCombat[] enemyCards = new CardInCombat[3];

    public GameObject[] enemyCombatSlots = new GameObject[3];
    public GameObject[] enemyBenchSlots = new GameObject[3];

    public GameObject[] playerCombatSlots = new GameObject[3];
    public GameObject[] playerBenchSlots = new GameObject[3];

    float timerToNextTurn = 0f;
    float resetTimerTo = 2f;
    bool startPlayerTurn = false;

    public Deck deck;
    public Deck enemyDeck;
    private void Start()
    {
        deck = GetComponent<Deck>();
        enemyDeck = GameObject.Find("EnemyDeck").GetComponent<Deck>();
        enemy.Initialize();
    }

    private void Update()
    {
        if (timerToNextTurn > 0)
        {
            timerToNextTurn -= Time.deltaTime;
        }
        else if (startPlayerTurn)
        {
            foreach (CardInCombat card in playerCards) if(card != null) card.PutOnOrOffTheBench();
            foreach (CardInCombat card in enemyCards) if (card != null) card.PutOnOrOffTheBenchEnemyCards();
            StartPlayerTurn();
            startPlayerTurn = false;
        }
    }

    #region Game Phases
    //--------------------------------//
    public void StartEnemyTurn()
    {
        if (gamePhase == 1) return;
        gamePhase = 1;

        enemyDeck.DiscardHand();
        enemyDeck.energy = 3;
        enemyDeck.DrawCard(5);

        enemy.StartTurn();

        StartCombatPhase();
        BenchMovement();
    }

    void BenchMovement()
    {
        FindCardsToSwap(playerCards);
        FindCardsToSwap(enemyCards);

        ResetMovedCards();

        timerToNextTurn = resetTimerTo;
        startPlayerTurn = true;
    }

    void FindCardsToSwap(CardInCombat[] colection) 
    {
        foreach (CardInCombat card in colection)
        {
            if (card == null || card.moved) continue;

            int slot = card.slot;
            if (card.benched && colection.Length > slot + 1 && (colection[slot + 1] == null || !colection[slot + 1].benched))
            {
                card.moved = true;
                if (colection[slot + 1] != null) colection[slot + 1].moved = true;
                SwapCards(slot, slot + 1, colection, playerBenchSlots);
            }
            else if (card.benched && slot - 1 >= 0 && (colection[slot - 1] == null || !colection[slot - 1].benched))
            {
                card.moved = true;
                if (colection[slot - 1] != null) colection[slot - 1].moved = true;
                SwapCards(slot, slot - 1, colection, playerBenchSlots);
            }
        }
    }

    void SwapCards(int card1, int card2, CardInCombat[] collection, GameObject[] benchSlots) 
    {
        CardInCombat temp = collection[card1];
        collection[card1] = collection[card2];
        collection[card2] = temp;

        collection[card2].MoveAnimationStarter(0.5f, benchSlots[card2].transform.position);
        collection[card2].slot = card2;

        if (collection[card1] != null)
        {
            collection[card1].MoveAnimationStarter(0.5f, benchSlots[card1].transform.position);
            collection[card1].slot = card1;
        }
    }

    void ResetMovedCards() 
    {
        foreach (CardInCombat card in playerCards) if (card != null)card.moved = false;
        foreach (CardInCombat card in enemyCards) if (card != null) card.moved = false;
    }

    void StartCombatPhase()
    {
        Debug.Log("Start combat");

        for (int i = 0; i < 3; i++)
        {
            if (playerCards[i] != null && enemyCards[i] != null) Skirmish(playerCards[i], enemyCards[i]);
            else if (playerCards[i] != null) DirectHit(playerCards[i]);
            else if (enemyCards[i] != null) DirectHit(enemyCards[i]);
        }
    }
    void StartPlayerTurn()
    {
        if (gamePhase == 1)
        {
            deck.DiscardHand();
            deck.energy = 3;
            deck.DrawCard(5);

            foreach (CardInCombat activeCard in playerCards)
            {
                if (activeCard != null)
                {
                    activeCard.passivesTurnedOnThisTurn = false;
                }

            }
            foreach (CardInCombat activeCard in enemyCards)
            {
                if (activeCard != null)
                {
                    activeCard.passivesTurnedOnThisTurn = false;
                }
            }

            foreach (CardInCombat activeCard in playerCards)
            {
                if (activeCard != null && activeCard.passivesTurnedOnThisTurn == false) 
                {
                    activeCard.passivesTurnedOnThisTurn = true;
                    activeCard.card.ActivatePasiveEffects(activeCard);
                    deck.UpdateCardAppearance(activeCard.transform, activeCard.card);
                }
            }
            foreach (CardInCombat activeCard in enemyCards)
            {
                if (activeCard != null && activeCard.passivesTurnedOnThisTurn == false)
                {
                    activeCard.passivesTurnedOnThisTurn = true;
                    activeCard.card.ActivatePasiveEffects(activeCard);
                    deck.UpdateCardAppearance(activeCard.transform, activeCard.card);
                }
            }
            
            gamePhase = 0;
        }
    }
    //--------------------------------//
    #endregion
    public void EnemyPlayCard(Card card, int slotNumber)
    {
        GameObject cardToCreate = Instantiate(deck.cardInCombatPrefab, enemyCombatSlots[slotNumber].transform.position, Quaternion.identity);
        cardToCreate.transform.SetParent(deck.CardsInCombatParent);
        cardToCreate.transform.localScale = Vector3.one * 0.75f;

        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = card.ResetCard();
        cardInCombat.deck = deck;
        cardInCombat.slot = slotNumber;
        cardInCombat.playerCard = false;
        cardInCombat.benched = false;

        enemyCards[slotNumber] = cardInCombat;
        enemyDeck.energy -= card.cost;
        if (enemyDeck.cardsInHandAsCards.Contains(card)) enemyDeck.cardsInHandAsCards.Remove(card);
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
        int oldPlayerHp = playerCard.card.health;
        int oldEnemyHp = enemyCard.card.health;

        if (playerCard.benched && enemyCard.benched) return;
        else if (playerCard.benched) { DirectHit(enemyCard); return;}
        else if (enemyCard.benched)  { DirectHit(playerCard); return;}

        // Generate inaccurate battle data
        playerCard.card.lastBattle = new BattleData(playerCard.card, enemyCard.card, oldPlayerHp, oldEnemyHp);
        enemyCard.card.lastBattle = new BattleData(enemyCard.card, playerCard.card, oldEnemyHp, oldPlayerHp);

        playerCard.card.health -= enemyCard.card.attack;
        playerCard.lastTypeOfDamage = enemyCard.card.typeOfDamage;
        playerCard.card.ActivateOnTakeDamageEffects(playerCard);
       
        enemyCard.card.health -= playerCard.card.attack;
        enemyCard.lastTypeOfDamage = playerCard.card.typeOfDamage;
        enemyCard.card.ActivateOnTakeDamageEffects(enemyCard);

        // Generate accurate battle data
        playerCard.card.lastBattle = new BattleData(playerCard.card, enemyCard.card, oldPlayerHp, oldEnemyHp);
        enemyCard.card.lastBattle = new BattleData(enemyCard.card, playerCard.card, oldEnemyHp, oldPlayerHp);

        playerCard.card.ActivateOnHitEffects(playerCard);
        enemyCard.card.ActivateOnHitEffects(enemyCard);

        playerCard.PerformShortAttackAnimation();
        enemyCard.PerformShortAttackAnimation();
    }
    //--------------------------------//
    #endregion
}
