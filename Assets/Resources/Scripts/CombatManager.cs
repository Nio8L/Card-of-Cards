using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class CombatManager : MonoBehaviour, IDataPersistence
{
    public static bool inCombat;

    public EnemyAI enemy;

    public int gamePhase = 0; // 0 - player turn, 1 - enemy turn, 2 - combat phase

    public int playerHealth = 20;
    public int enemyHealth = 20;

    public CardInCombat[] playerCombatCards = new CardInCombat[3];
    public CardInCombat[] playerBenchCards = new CardInCombat[3];

    public CardInCombat[] playerCombatCardsAtStartOfTurn = new CardInCombat[3];
    public CardInCombat[] playerBenchCardsAtStartOfTurn = new CardInCombat[3];

    public CardInCombat[] enemyCombatCards = new CardInCombat[3];
    public CardInCombat[] enemyBenchCards = new CardInCombat[3];

    public GameObject[] enemyCombatSlots = new GameObject[3];
    public GameObject[] enemyBenchSlots = new GameObject[3];

    public GameObject[] playerCombatSlots = new GameObject[3];
    public GameObject[] playerBenchSlots = new GameObject[3];

    float timerToNextTurn = 0f;
    float timerAfterEnemyTurn = 0f;
    float resetTimerTo = 2f;
    float resetTimerAfterEnemyTurnTo = 0.5f;
    bool startCombatPhase = false;
    bool startPlayerTurn = false;

    public Deck deck;
    public Deck enemyDeck;

    public TextMeshProUGUI playerHPText;
    public TextMeshProUGUI enemyHPText;
    public GameObject startCombatMenu;
    public GameObject endCombatMenu;
    public TextMeshProUGUI endCombatText;

    GameObject playerHealthRect;
    GameObject playerHealthDash;
    TextMeshProUGUI playerHealthText;   
    GameObject enemyHealthRect;
    GameObject enemyHealthDash;
    TextMeshProUGUI enemyHealthText;
    TextMeshProUGUI graveText;
    TextMeshProUGUI roundText;
    TextMeshProUGUI enemyCardPileText;

    public int playerCardsLost = 0;
    int round = 1;
    private void Start()
    {
        Time.timeScale = 0;
        inCombat = false;
        startCombatMenu.SetActive(true);
        endCombatMenu.SetActive(false);

        deck = GetComponent<Deck>();
        enemyDeck = GameObject.Find("EnemyDeck").GetComponent<Deck>();
        playerHealthRect = GameObject.Find("PlayerHealth");
        playerHealthDash = GameObject.Find("PlayerHealthDash");
        enemyHealthRect = GameObject.Find("EnemyHealth");
        enemyHealthDash = GameObject.Find("EnemyHealthDash");
        playerHealthText = GameObject.Find("PlayerHealthText").GetComponent<TextMeshProUGUI>();
        enemyHealthText = GameObject.Find("EnemyHealthText").GetComponent<TextMeshProUGUI>();
        graveText = GameObject.Find("GraveText").GetComponent<TextMeshProUGUI>();
        roundText = GameObject.Find("RoundText").GetComponent<TextMeshProUGUI>();
        enemyCardPileText = GameObject.Find("EnemyCardPileText").GetComponent<TextMeshProUGUI>();
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
        SceneManager.LoadSceneAsync("Map");
        //END THE GAME HERE
    }

    private void Update()
    {
        if (timerAfterEnemyTurn > 0)
        {
            timerAfterEnemyTurn -= Time.deltaTime;
        }
        else if (startCombatPhase) 
        {
            startCombatPhase = false;
            StartCombatPhase();
        }

        if (timerToNextTurn > 0)
        {
            timerToNextTurn -= Time.deltaTime;
        }
        else if (startPlayerTurn)
        {
            //if (HasInAnimationCard()) return;
            StartPlayerTurn();
            startPlayerTurn = false;
        }
        graveText.text = playerCardsLost.ToString();
        roundText.text = "Round " + round;
        enemyCardPileText.text = enemyDeck.cards.Count.ToString();
    }

    #region Game Phases
    //--------------------------------//
    public void StartEnemyTurn()
    {
        if (gamePhase > 0) return;
        gamePhase = 1;

        enemyDeck.DiscardHand();
        enemyDeck.energy = 3;
        enemyDeck.DrawCard(5);

        enemy.StartTurn();

        startCombatPhase = true;
        timerAfterEnemyTurn = resetTimerAfterEnemyTurnTo;
    }

    void BenchMovement()
    {
        //FindCardsToSwap(playerCards);
        //FindCardsToSwap(enemyCards);

        ResetMovedCards();
    }
    // Bench left of right movement
    /*void FindCardsToSwap(CardInCombat[] colection) 
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
    */
    void ResetMovedCards() 
    {
        foreach (CardInCombat card in playerBenchCards) if (card != null)card.moved = false;
        foreach (CardInCombat card in playerCombatCards) if (card != null) card.moved = false;
        foreach (CardInCombat card in enemyBenchCards) if (card != null) card.moved = false;
        foreach (CardInCombat card in enemyCombatCards) if (card != null) card.moved = false;
    }

    void StartCombatPhase()
    {
        //Debug.Log("Start combat");
        for (int i = 0; i < 3; i++)
        {
            if (playerCombatCards[i] != null) playerCombatCards[i].card.ActivateOnBattleStartEffects(playerCombatCards[i]);
            if (playerBenchCards[i] != null) playerBenchCards[i].card.ActivateOnBattleStartEffects(playerBenchCards[i]);
            if (enemyCombatCards[i] != null) enemyCombatCards[i].card.ActivateOnBattleStartEffects(enemyCombatCards[i]);
            if (enemyBenchCards[i] != null) enemyBenchCards[i].card.ActivateOnBattleStartEffects(enemyBenchCards[i]);
        }

        gamePhase = 2;

        for (int i = 0; i < 3; i++)
        {
            if (playerCombatCards[i] != null && enemyCombatCards[i] != null) Skirmish(playerCombatCards[i], enemyCombatCards[i]);
            else if (playerCombatCards[i] != null) DirectHit(playerCombatCards[i]);
            else if (enemyCombatCards[i] != null) DirectHit(enemyCombatCards[i]);
        }

        timerToNextTurn = resetTimerTo;
        startPlayerTurn = true;
    }
    void StartPlayerTurn()
    {
        round++;
        BenchMovement();
        
        if (gamePhase == 2)
        {
            deck.DiscardHand();
            deck.energy = 3;
            deck.DrawCard(5);

            for(int i = 0; i < 3 ;i++)
            {
                if (playerCombatCards[i] != null)
                {
                    playerCombatCards[i].passivesTurnedOnThisTurn = false;
                }
                if (playerBenchCards[i] != null)
                {
                    playerBenchCards[i].passivesTurnedOnThisTurn = false;
                }
            }
            for(int i = 0; i < 3 ;i++)
            {
                if (enemyBenchCards[i] != null)
                {
                    enemyBenchCards[i].passivesTurnedOnThisTurn = false;
                }
                if (enemyCombatCards[i] != null)
                {
                    enemyCombatCards[i].passivesTurnedOnThisTurn = false;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                if (playerBenchCards[i] != null && playerBenchCards[i].passivesTurnedOnThisTurn == false) 
                {
                    playerBenchCards[i].passivesTurnedOnThisTurn = true;
                    playerBenchCards[i].card.ActivatePasiveEffects(playerBenchCards[i]);
                    deck.UpdateCardAppearance(playerBenchCards[i].transform, playerBenchCards[i].card);
                }
                if(playerCombatCards[i] != null && playerCombatCards[i].passivesTurnedOnThisTurn == false)
                {
                    playerCombatCards[i].passivesTurnedOnThisTurn = true;
                    playerCombatCards[i].card.ActivatePasiveEffects(playerCombatCards[i]);
                    deck.UpdateCardAppearance(playerCombatCards[i].transform, playerCombatCards[i].card);
                }
            }
            for (int i = 0; i < 3; i++)
            {
                if (enemyBenchCards[i] != null && enemyBenchCards[i].passivesTurnedOnThisTurn == false)
                {
                    enemyBenchCards[i].passivesTurnedOnThisTurn = true;
                    enemyBenchCards[i].card.ActivatePasiveEffects(enemyBenchCards[i]);
                    deck.UpdateCardAppearance(enemyBenchCards[i].transform, enemyBenchCards[i].card);
                }
                if (enemyCombatCards[i] != null && enemyCombatCards[i].passivesTurnedOnThisTurn == false)
                {
                    enemyCombatCards[i].passivesTurnedOnThisTurn = true;
                    enemyCombatCards[i].card.ActivatePasiveEffects(enemyCombatCards[i]);
                    deck.UpdateCardAppearance(enemyCombatCards[i].transform, enemyCombatCards[i].card);
                }
            }
            playerCombatCards.CopyTo(playerCombatCardsAtStartOfTurn, 0);
            playerBenchCards.CopyTo(playerBenchCardsAtStartOfTurn, 0);
            gamePhase = 0;
        }
    }
    //--------------------------------//
    #endregion
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
                TooltipSystem.QuickHide();
                endCombatMenu.SetActive(true);
                endCombatText.text = "you won";
            }
        }
        else
        {
            playerHealth -= damage;
            if (playerHealth <= 0)
            {
                TooltipSystem.QuickHide();
                endCombatMenu.SetActive(true);
                endCombatText.text = "you lost";
            }
        }
        updateHPText();
        //to do
    }

    public void updateHPText() 
    {
        Debug.Log("Update health");

        float playerVal = playerHealth / 20f;
        float enemyVal = enemyHealth / (float)enemy.maxHealth;

        if (playerVal <= 0) playerVal = 0;
        if (enemyVal <= 0) enemyVal = 0;

        playerHealthRect.transform.localScale =    new Vector3(playerVal, 1, 1);
        playerHealthDash.transform.localPosition = new Vector3(playerVal * 200 - 150, 1, 1);
        enemyHealthRect.transform.localScale =    new Vector3(enemyVal, 1, 1);
        enemyHealthDash.transform.localPosition = new Vector3(enemyVal * 200 - 150, 1, 1);

        playerHealthText.text = playerHealth + "/" + 20;
        enemyHealthText.text = enemyHealth + "/" + enemy.maxHealth;
        //enemyHPText.text = "Enemy HP: " + enemyHealth;
    }

    public void Skirmish(CardInCombat playerCard, CardInCombat enemyCard)
    {
        int oldPlayerHp = playerCard.card.health;
        int oldEnemyHp = enemyCard.card.health;

        if (playerCard.benched && enemyCard.benched) return;
        else if (playerCard.benched) { DirectHit(enemyCard); return;}
        else if (enemyCard.benched)  { DirectHit(playerCard); return;}

        //Debug.Log("skirmish");



        playerCard.card.health -= enemyCard.card.attack;
        playerCard.lastTypeOfDamage = enemyCard.card.typeOfDamage;
       
        enemyCard.card.health -= playerCard.card.attack;
        enemyCard.lastTypeOfDamage = playerCard.card.typeOfDamage;

        // Generate inaccurate battle data
        playerCard.card.lastBattle = new BattleData(playerCard.card, enemyCard.card, oldPlayerHp, oldEnemyHp);
        enemyCard.card.lastBattle = new BattleData(enemyCard.card, playerCard.card, oldEnemyHp, oldPlayerHp);

        playerCard.card.ActivateOnTakeDamageEffects(playerCard);
        enemyCard.card.ActivateOnTakeDamageEffects(enemyCard);

        // Generate accurate battle data
        playerCard.card.lastBattle = new BattleData(playerCard.card, enemyCard.card, oldPlayerHp, oldEnemyHp);
        enemyCard.card.lastBattle = new BattleData(enemyCard.card, playerCard.card, oldEnemyHp, oldPlayerHp);

        playerCard.card.ActivateOnHitEffects(playerCard);
        enemyCard.card.ActivateOnHitEffects(enemyCard);

        if(playerCard.PerformeAtackAnimation) playerCard.PerformShortAttackAnimation();
        if(enemyCard.PerformeAtackAnimation) enemyCard.PerformShortAttackAnimation();

        enemyCard.PerformeAtackAnimation = true;
        playerCard.PerformeAtackAnimation = true;
    }

    public void LoadData(GameData data)
    {
        playerHealth = data.playerHealth;
    }

    public void SaveData(ref GameData data)
    {
        data.playerHealth = playerHealth;
    }
    //--------------------------------//

    #endregion
}
