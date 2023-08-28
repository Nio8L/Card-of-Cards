using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CombatManager : MonoBehaviour
{
    public static bool inCombat;

    public EnemyAI enemy;

    public int gamePhase = 0; // 0 - player turn, 1 - enemy turn -> combat phase

    public int playerHealth = 20;
    public int enemyHealth = 20;

    public CardInCombat[] playerCards = new CardInCombat[3];
    public CardInCombat[] playerCardsAtStartOfTurn = new CardInCombat[3];
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

    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public GameObject startCombatMenu;
    public GameObject endCombatMenu;
    public TextMeshProUGUI endCombatText;

    private void Start()
    {
        Time.timeScale = 0;
        inCombat = false;
        startCombatMenu.SetActive(true);
        endCombatMenu.SetActive(false);

        deck = GetComponent<Deck>();
        enemyDeck = GameObject.Find("EnemyDeck").GetComponent<Deck>();
        enemy.Initialize();
    }

    public void StartGame() 
    {
        SoundManager.soundManager.Play("ButtonClick");
        inCombat = true;
        Time.timeScale = 1;
        startCombatMenu.SetActive(false);
    }

    public void EndGame()
    {
        SoundManager.soundManager.Play("ButtonClick");
        inCombat = false;
        endCombatMenu.SetActive(false);
        Time.timeScale = 1;
        //END THE GAME HERE
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
    }

    void BenchMovement()
    {
        FindCardsToSwap(playerCards);
        FindCardsToSwap(enemyCards);

        ResetMovedCards();
    }

    void FindCardsToSwap(CardInCombat[] colection) 
    {
        foreach (CardInCombat card in colection)
        {
            if (card == null || card.moved || !card.benched) continue;

            int slot = card.slot;
            int direction = card.direction;

            if (slot+direction < colection.Length && slot + direction>=0 && (colection[slot + direction] == null || !colection[slot + direction].benched))
            {
                SwapCards(slot, slot + direction,colection);
                continue;
            }

            direction *= -1;
            if ((slot + direction < colection.Length && slot + direction >= 0) && (colection[slot + direction] == null || !colection[slot + direction].benched))
            {
                card.direction = direction;
                SwapCards(slot, slot + direction, colection);
                continue;
            }
        }
    }

    void SwapCards(int card1, int card2, CardInCombat[] collection) 
    {
        CardInCombat temp = collection[card1];
        collection[card1] = collection[card2];
        collection[card2] = temp;

        if (collection[card2].playerCard) collection[card2].MoveAnimationStarter(0.5f, playerBenchSlots[card2].transform.position);
        else collection[card2].MoveAnimationStarter(0.5f, enemyBenchSlots[card2].transform.position);
        collection[card2].slot = card2;
        collection[card2].moved = true;

        if (collection[card1] != null)
        {
            if(collection[card1].playerCard) collection[card1].MoveAnimationStarter(0.5f, playerCombatSlots[card1].transform.position);
            else collection[card1].MoveAnimationStarter(0.5f, enemyCombatSlots[card1].transform.position);
            collection[card1].slot = card1;
            collection[card1].moved = true;
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
        timerToNextTurn = resetTimerTo;
        startPlayerTurn = true;
    }
    void StartPlayerTurn()
    {
        BenchMovement();
        
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
            playerCards.CopyTo(playerCardsAtStartOfTurn, 0);
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
        SoundManager.soundManager.Play("CardHit");
        if (card.benched) return;
        card.PerformShortAttackAnimation();

        if (card.playerCard)
        {
            enemyHealth -= card.card.attack;
            if(enemyHealth <= 0) 
            {
                TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
                endCombatMenu.SetActive(true);
                Time.timeScale = 0;
                endCombatText.text = "you won";
            }
        }
        else
        {
            playerHealth -= card.card.attack;
            if (playerHealth <= 0)
            {
                TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
                endCombatMenu.SetActive(true);
                Time.timeScale = 0;
                endCombatText.text = "you lost";
            }
        }
        updateHPText();
        //to do
    }
    public void DirectHit(CardInCombat card, int damage)
    {
        if (card.benched) return;
        card.PerformShortAttackAnimation();

        if (card.playerCard)
        {
            enemyHealth -= damage;
            if (enemyHealth <= 0)
            {
                TooltipSystem.Hide();
                endCombatMenu.SetActive(true);
                endCombatText.text = "you won";
            }
        }
        else
        {
            playerHealth -= damage;
            if (playerHealth <= 0)
            {
                TooltipSystem.Hide();
                endCombatMenu.SetActive(true);
                endCombatText.text = "you lost";
            }
        }
        updateHPText();
        //to do
    }

    void updateHPText() 
    {
        playerHPText.text = "Player HP: " + playerHealth;
        enemyHPText.text = "Enemy HP: " + enemyHealth;
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
       
        enemyCard.card.health -= playerCard.card.attack;
        enemyCard.lastTypeOfDamage = playerCard.card.typeOfDamage;

        playerCard.card.ActivateOnTakeDamageEffects(playerCard);
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
