using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;


public class CombatManager : MonoBehaviour, IDataPersistence
{
    public static CombatManager combatManager;

    public bool inCombat;

    public EnemyBase enemy;

    public int gamePhase = 0; // 0 - player turn, 1 - enemy turn, 2 - combat phase

    public int playerHealth = 20;
    public int enemyHealth = 20;

    public CardInCombat[] playerCombatCards = new CardInCombat[5];
    public CardInCombat[] playerBenchCards = new CardInCombat[5];

    public CardInCombat[] playerCombatCardsAtStartOfTurn = new CardInCombat[5];
    public CardInCombat[] playerBenchCardsAtStartOfTurn = new CardInCombat[5];

    public CardInCombat[] enemyCombatCards = new CardInCombat[5];
    public CardInCombat[] enemyBenchCards = new CardInCombat[5];

    public GameObject[] enemyCombatSlots = new GameObject[5];
    public GameObject[] enemyBenchSlots = new GameObject[5];

    public GameObject[] playerCombatSlots = new GameObject[5];
    public GameObject[] playerBenchSlots = new GameObject[5];

    public List<Card> battleReward = new List<Card>();

    float timerToNextTurn = 0f;
    float timerAfterEnemyTurn = 0f;
    float resetTimerTo = 2f;
    float resetTimerAfterEnemyTurnTo = 0.5f;
    float cameraZoomTimer = 1f;
    bool startCombatPhase = false;
    bool startPlayerTurn = false;

    public Deck deck;
    public Deck enemyDeck;

    public CombatUI combatUI;

    public int playerCardsLost = 0;
    public int round = 1;


    void Awake(){
        combatManager = this;
    }
    
    private void OnEnable() {
        EventManager.CardDeath += CardDeath;
    }

    private void OnDisable() {
        EventManager.CardDeath -= CardDeath;
    }
    
    private void Start()
    {
        Time.timeScale = 0;
        inCombat = false;

        StartGame();
    }

    public void StartGame() 
    {
        combatUI.BeginCombat();

        SoundManager.soundManager.Play("ButtonClick");
        inCombat = true;
        Time.timeScale = 1;
    }

    public void EndGame()
    {
        SoundManager.soundManager.Play("ButtonClick");
        inCombat = false;
        combatUI.LoadOutOfCombat();
        Time.timeScale = 1;
        
        DataPersistenceManager.DataManager.currentCombatAI = null;
            
        if(combatUI.endCombatText.text == "You won!") {
            if (enemy.isHunter) SceneManager.LoadSceneAsync("End Screen");
            else                SceneManager.LoadSceneAsync("Map");
        }else{
            DataPersistenceManager.DataManager.DeleteMostRecentProfileData();
            SceneManager.LoadSceneAsync("Main Menu");
        }

        enemy = null;
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
        combatUI.UpdateGraveText();
        if (inCombat && cameraZoomTimer > 0)
        {
            Camera.main.orthographicSize = Mathf.Lerp(8, 5, Mathf.SmoothStep(1f, 0f, cameraZoomTimer));
            cameraZoomTimer -= Time.deltaTime;
        }
        else if (inCombat && cameraZoomTimer > -10)
        {
            Camera.main.orthographicSize = 5;
            // This line is here to make it set the camera size to 5 only once 
            cameraZoomTimer = -100;
        }
    }
  
    public void PlayCard(Card card, CardSlot slot, bool useDeck){
        /*
            Card card     - The card scriptable object to play the card with
            CardSlot slot - The slot at which the card will be played
            bool useDeck  - Whether or not the card has to be removed from its owners hand and its cost has to be deducted from the owners energy
        */
        
        // Remove the card from its owners hand
        if (useDeck){
            if (slot.playerSlot){
                deck.energy -= card.cost;
                deck.cardsInHandAsCards.Remove(card);
            }else{
                enemyDeck.energy -= card.cost;
            enemyDeck.cardsInHandAsCards.Remove(card);
            }
        }

        // Instantiating a Card in combat prefab
        GameObject cardToCreate = Instantiate(deck.cardInCombatPrefab, slot.transform.position, Quaternion.identity);
        // Fixings its order in the hierarchy
        cardToCreate.transform.SetParent(deck.CardsInCombatParent);
        // Fixings its scale
        cardToCreate.transform.localScale = Vector3.one * 0.75f;
        cardToCreate.transform.rotation = Quaternion.Euler(0, 0, UnityEngine.Random.Range(-5, 5f) );
        // Initializing CardInCombat component with necesery values
        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = card;
        cardInCombat.slot = slot.slot;
        cardInCombat.playerCard = slot.playerSlot;
        
        // Find the card's owner's deck so it has a reference to it
        if (slot.playerSlot) cardInCombat.deck = deck;
        else                 cardInCombat.deck = enemyDeck;

        // Place the card in the correct list of cards so the Combat Manager (this) can track it
        if (slot.bench)
        {
            cardInCombat.benched = true;
            if (slot.playerSlot) playerBenchCards[slot.slot] = cardInCombat;
            else                 enemyBenchCards[slot.slot] = cardInCombat;
            
        }
        else
        {
            cardInCombat.benched = false;
            if (slot.playerSlot) playerCombatCards[slot.slot] = cardInCombat;
            else                 enemyCombatCards[slot.slot] = cardInCombat;
        }
    }


    #region Game Phases
    public void StartEnemyTurn()
    {
        if (gamePhase > 0) return;
        gamePhase = 1;

        ActiveAbilityManager.activeAbilityManager.Deselect();

        deck.ForceDraw();
        deck.DiscardHand();

        enemyDeck.DiscardHand();
        enemyDeck.DrawCard(5);

        enemy.StartTurn();

        startCombatPhase = true;
        timerAfterEnemyTurn = resetTimerAfterEnemyTurnTo;
    }
    void StartCombatPhase()
    {
        //Debug.Log("Start combat");
        for (int i = 0; i < 5; i++)
        {
            if (playerCombatCards[i] != null) playerCombatCards[i].card.ActivateOnBattleStartEffects(playerCombatCards[i]);
            if (playerBenchCards[i] != null) playerBenchCards[i].card.ActivateOnBattleStartEffects(playerBenchCards[i]);
            if (enemyCombatCards[i] != null) enemyCombatCards[i].card.ActivateOnBattleStartEffects(enemyCombatCards[i]);
            if (enemyBenchCards[i] != null) enemyBenchCards[i].card.ActivateOnBattleStartEffects(enemyBenchCards[i]);
        }

        gamePhase = 2;
        for (int i = 0; i < 5; i++)
        {
            if (playerCombatCards[i] != null && enemyCombatCards[i] != null) CardCombat2Attackers(playerCombatCards[i], enemyCombatCards[i]);
            else if (playerCombatCards[i] != null) DirectHit(playerCombatCards[i]);
            else if (enemyCombatCards[i] != null) DirectHit(enemyCombatCards[i]);
        }

        timerToNextTurn = resetTimerTo;
        startPlayerTurn = true;
    }
    void StartPlayerTurn()
    {
        round++;

        combatUI.UpdateRoundText();

        if (enemy.huntAI && round == enemy.huntRounds + 1)
        {
            GameObject.Find("EndTurnButton").SetActive(false);
            Invoke("WinGame", 2f);
        }
        
        if (gamePhase == 2)
        {
            deck.energy = 3;
            deck.DrawCard(5);

            for(int i = 0; i < 5 ;i++)
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
            for(int i = 0; i < 5 ;i++)
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

            for (int i = 0; i < 5; i++)
            {
                if (playerBenchCards[i] != null && playerBenchCards[i].passivesTurnedOnThisTurn == false && playerBenchCards[i].card.health > 0f) 
                {
                    playerBenchCards[i].passivesTurnedOnThisTurn = true;
                    playerBenchCards[i].card.ActivatePasiveEffects(playerBenchCards[i]);
                    deck.UpdateCardAppearance(playerBenchCards[i].transform, playerBenchCards[i].card);
                }
                if(playerCombatCards[i] != null && playerCombatCards[i].passivesTurnedOnThisTurn == false && playerCombatCards[i].card.health > 0f)
                {
                    playerCombatCards[i].passivesTurnedOnThisTurn = true;
                    playerCombatCards[i].card.ActivatePasiveEffects(playerCombatCards[i]);
                    deck.UpdateCardAppearance(playerCombatCards[i].transform, playerCombatCards[i].card);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (enemyBenchCards[i] != null && enemyBenchCards[i].passivesTurnedOnThisTurn == false && enemyBenchCards[i].card.health > 0f)
                {
                    enemyBenchCards[i].passivesTurnedOnThisTurn = true;
                    enemyBenchCards[i].card.ActivatePasiveEffects(enemyBenchCards[i]);
                    deck.UpdateCardAppearance(enemyBenchCards[i].transform, enemyBenchCards[i].card);
                }
                if (enemyCombatCards[i] != null && enemyCombatCards[i].passivesTurnedOnThisTurn == false && enemyCombatCards[i].card.health > 0f)
                {
                    enemyCombatCards[i].passivesTurnedOnThisTurn = true;
                    enemyCombatCards[i].card.ActivatePasiveEffects(enemyCombatCards[i]);
                    deck.UpdateCardAppearance(enemyCombatCards[i].transform, enemyCombatCards[i].card);
                }
            }

            int enemyNonSoulCards = 0;
            for (int i = 0; i < enemyDeck.cards.Count; i++)
            {
                if (enemyDeck.cards[i].name != "LostSoul")
                {
                    enemyNonSoulCards++;
                }
            }
            combatUI.UpdateEnemyCardPileText();

            playerCombatCards.CopyTo(playerCombatCardsAtStartOfTurn, 0);
            playerBenchCards.CopyTo(playerBenchCardsAtStartOfTurn, 0);
            gamePhase = 0;

            //Activate end of turn effects

            for (int i = 0; i < 5; i++)
            {
                if (playerBenchCards[i] != null && playerBenchCards[i].card.health > 0f) 
                {
                    playerBenchCards[i].card.ActivaeOnEndOfTurnEffects(playerBenchCards[i]);
                    deck.UpdateCardAppearance(playerBenchCards[i].transform, playerBenchCards[i].card);
                }
                if(playerCombatCards[i] != null && playerCombatCards[i].card.health > 0f)
                {
                    playerCombatCards[i].card.ActivaeOnEndOfTurnEffects(playerCombatCards[i]);
                    deck.UpdateCardAppearance(playerCombatCards[i].transform, playerCombatCards[i].card);
                }
            }
            for (int i = 0; i < 5; i++)
            {
                if (enemyBenchCards[i] != null && enemyBenchCards[i].card.health > 0f)
                {
                    enemyBenchCards[i].card.ActivaeOnEndOfTurnEffects(enemyBenchCards[i]);
                    deck.UpdateCardAppearance(enemyBenchCards[i].transform, enemyBenchCards[i].card);
                }
                if (enemyCombatCards[i] != null && enemyCombatCards[i].card.health > 0f)
                {
                    enemyCombatCards[i].card.ActivaeOnEndOfTurnEffects(enemyCombatCards[i]);
                    deck.UpdateCardAppearance(enemyCombatCards[i].transform, enemyCombatCards[i].card);
                }
            }
        }
    }
    #endregion


    #region Attacks
    public void DirectHit(CardInCombat card)
    {
        // Just calls its alternate functions with int damage set as the cards attack
        DirectHit(card, card.card.attack);
    }
    public void DirectHit(CardInCombat card, int damage)
    {
        /*
            CardInCombat card - The card that is dealing damage
            int damage        - The amount of damage it will do
        */
        // Hits the opponent directly

        // Return if the card is benched since benched cards can't attack
        if (card.benched || damage == 0) return;
        // Starts the attack animation
        card.MoveAnimationStarter(0.5f, new Vector3(card.transform.position.x, 2f + -2f * Convert.ToInt32(!card.playerCard), 0f), true, card.slot * 0.2f);

        // Checks who the owner of the cards is and deals damage to them
        if (card.playerCard)
        {
            enemyHealth -= damage;
            // Win if the enemies health drops to or below 0
            if (enemyHealth <= 0)
            {
                if (GameObject.Find("EndTurnButton") != null) GameObject.Find("EndTurnButton").SetActive(false);
                Invoke("WinGame", 2f);
            }
        }
        else
        {
            playerHealth -= damage;
            // Lose if the players health drops to or below 0
            if (playerHealth <= 0)
            {
                Invoke("LoseGame", 2f);
            }
        }
        // Updates the health bars
        combatUI.UpdateHPText();
    } 
    public void CardCombat2Attackers(CardInCombat playerCard, CardInCombat enemyCard)
    {
        if (playerCard.card.attack == 0 && enemyCard.card.attack == 0) return;
        else if (playerCard.card.attack == 0) {CardCombat1Attacker(enemyCard , playerCard, enemyCard.card.attack ); return;}
        else if (enemyCard.card.attack == 0)  {CardCombat1Attacker(playerCard, enemyCard , playerCard.card.attack); return;}

        int oldPlayerHp = playerCard.card.health;
        int oldEnemyHp = enemyCard.card.health;

        if (playerCard.benched && enemyCard.benched) return;
        else if (playerCard.benched) { DirectHit(enemyCard ); return;}
        else if (enemyCard.benched)  { DirectHit(playerCard); return;}


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

        if(playerCard.PerformAtackAnimation) playerCard.MoveAnimationStarter(0.5f, new Vector3(playerCard.transform.position.x, 1f, 0f), true, playerCard.slot * 0.2f);
        if(enemyCard.PerformAtackAnimation)  enemyCard. MoveAnimationStarter(0.5f, new Vector3(enemyCard .transform.position.x, 1f, 0f), true, enemyCard.slot  * 0.2f);

        enemyCard.PerformAtackAnimation = true;
        playerCard.PerformAtackAnimation = true;
    } 
    public void CardCombat1Attacker(CardInCombat attacker, CardInCombat defender, int damage){
        int oldAttackerHp = attacker.card.health;
        int oldDefenderHp = defender.card.health;
       
        defender.card.health     -= damage;
        defender.lastTypeOfDamage = attacker.card.typeOfDamage;

        // Generate inaccurate battle data
        attacker.card.lastBattle = new BattleData(attacker.card, defender.card, oldAttackerHp, oldDefenderHp);
        defender.card.lastBattle = new BattleData(defender.card, attacker.card, oldDefenderHp, oldAttackerHp);

        defender.card.ActivateOnTakeDamageEffects(defender);

        // Generate accurate battle data
        attacker.card.lastBattle = new BattleData(attacker.card, defender.card, oldAttackerHp, oldDefenderHp);
        defender.card.lastBattle = new BattleData(defender.card, attacker.card, oldDefenderHp, oldAttackerHp);

        attacker.card.ActivateOnHitEffects(attacker);

        if(attacker.PerformAtackAnimation) attacker.MoveAnimationStarter(0.5f, new Vector3(attacker.transform.position.x, 1f, 0f), true, attacker.slot * 0.2f);

        attacker.PerformAtackAnimation = true;
    }
    #endregion


    #region Saving system
    public void LoadData(GameData data)
    {
        playerHealth = data.playerHealth;

        enemy = Resources.Load<EnemyBase>("Enemies/" + data.enemyAI);
    }
    public void SaveData(GameData data)
    {
        data.playerHealth = playerHealth;

        if(enemy != null){
            data.enemyAI = enemy.ReturnPath();
        }else{
            data.enemyAI = "";
        }
    }
    #endregion


    #region EndConditions
    void WinGame()
    {
        timerToNextTurn = 1000f;
        if (enemy.isTutorialEnemy) { TutorialWin(); return; }

        for (int i = 0; i < 5; i++)
        {
            if (playerCombatCards[i] != null) playerCombatCards[i].card.ActivateOnBattleEndEffects(playerCombatCards[i]);
            if (playerBenchCards[i] != null) playerBenchCards[i].card.ActivateOnBattleEndEffects(playerBenchCards[i]);
            if (enemyCombatCards[i] != null) enemyCombatCards[i].card.ActivateOnBattleEndEffects(enemyCombatCards[i]);
            if (enemyBenchCards[i] != null) enemyBenchCards[i].card.ActivateOnBattleEndEffects(enemyBenchCards[i]);
        }

        TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
        combatUI.EndCombat(true);
        Time.timeScale = 0;
        
        deck.cards.AddRange(battleReward);
    }

    void LoseGame()
    {
        timerToNextTurn = 1000f;
     
        for (int i = 0; i < 5; i++)
        {
            if (playerCombatCards[i] != null) playerCombatCards[i].card.ActivateOnBattleEndEffects(playerCombatCards[i]);
            if (playerBenchCards[i] != null) playerBenchCards[i].card.ActivateOnBattleEndEffects(playerBenchCards[i]);
            if (enemyCombatCards[i] != null) enemyCombatCards[i].card.ActivateOnBattleEndEffects(enemyCombatCards[i]);
            if (enemyBenchCards[i] != null) enemyBenchCards[i].card.ActivateOnBattleEndEffects(enemyBenchCards[i]);
        }

        TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
        combatUI.EndCombat(false);
        Time.timeScale = 0;
        
    }

    void TutorialWin()
    {
        TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
        combatUI.EndCombat(true);
        Time.timeScale = 0;
        
        deck.cards.AddRange(battleReward);
    }
    #endregion


    public void ClickOnDialogueBox()
    {
        if (enemy.dialogue != null && enemy.dialogue.NextLineAtClick) enemy.dialogue.NextLine(); 
    }
    
    public void CardDeath(){
        playerCardsLost++;
    }

    public CardInCombat GetCardAtSlot(CardSlot slot){
        // Find cardInCombat via cardSlot
        if (slot.playerSlot){
            if (slot.bench) return playerBenchCards [slot.slot];
            else            return playerCombatCards[slot.slot];
        }else{
            if (slot.bench) return enemyBenchCards  [slot.slot];
            else            return enemyCombatCards [slot.slot];
        }
    }

    public void SetCardAtSlot(CardSlot slot, CardInCombat cardInCombat){
        if (cardInCombat != null){
            cardInCombat.slot    = slot.slot;
            cardInCombat.benched = slot.bench;
        }

        if (slot.playerSlot){
            if (slot.bench) playerBenchCards [slot.slot] = cardInCombat;
            else            playerCombatCards[slot.slot] = cardInCombat;
        }else{
            if (slot.bench) enemyBenchCards  [slot.slot] = cardInCombat;
            else            enemyCombatCards [slot.slot] = cardInCombat;
        }
    }
}
