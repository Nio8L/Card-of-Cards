using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System;
using Mono.Cecil;


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
    public float timerAfterEnemyTurn = 0f;
    float resetTimerTo = 2f;
    float resetTimerAfterEnemyTurnTo = 0.75f;
    bool startCombatPhase = false;
    bool startPlayerTurn = false;

    public Deck deck;
    public Deck enemyDeck;

    public CombatUI combatUI;

    public List<Card> playerCardsLost;
    public int round = 1;

    public GameObject fireExplosionPrefab;

    [Header("Notifications")]
    public Notification noDrawNotification;
    public Notification noDiscardNotification;
    public Notification noGraveNotification;

    void Awake(){
        combatManager = this;

        // Clear all existing displays
        DeckUtilities.CloseAllDisplays();
        
        // Setup tutorial
        if(ScenePersistenceManager.scenePersistence.inTutorial)
        {
            // Find the correct ai
            ScenePersistenceManager.scenePersistence.currentCombatAI = Resources.Load<EnemyBase>("Enemies/" + ScenePersistenceManager.scenePersistence.tutorialCombats[ScenePersistenceManager.scenePersistence.tutorialStage].ReturnPath());

            deck.cards.AddRange(ScenePersistenceManager.scenePersistence.tutorialDeck);
            
            ListWrapper tutorialDeckToLoad = ScenePersistenceManager.scenePersistence.tutorialCardsToAdd[ScenePersistenceManager.scenePersistence.tutorialStage];

            // Load the correct deck
            for (int i = 0; i < tutorialDeckToLoad.list.Count; i++){
                string card = tutorialDeckToLoad.list[i];
                Card cardToAdd = Instantiate(Resources.Load<Card>("Cards/" + card)).ResetCard();
                cardToAdd.name = card;
                deck.cards.Add(cardToAdd);
            }
        }
        
        // Find the enemy ai for this combat
        if (ScenePersistenceManager.scenePersistence.currentCombatAI != null)
        {
            enemy = ScenePersistenceManager.scenePersistence.currentCombatAI;
        }
        // Initialize it
        enemy.Initialize();

        // Begin camera animation and load ui
        AnimationUtilities.ChangeFOV(Camera.main.transform, 1f, 0, 5);
        combatUI.BeginCombat();

        // Give a small delay at the start of the combat to prevent early clicking on the end turn button
        timerToNextTurn = resetTimerTo;

        SoundManager.soundManager.Play("ButtonClick");
        inCombat = true;
        Time.timeScale = 1;

        
        
        // Start the turn zero function in 1 second so the cards are placed correctly
        Invoke("TurnZero", 1f);
    }
    void Start(){
        // Use start if something needs to know the players hp
        // Update the hp text
        combatUI.UpdateHPText();

        // Check if the player has died
        if (playerHealth <= 0){
            LoseGame();
            return;
        }

        deck.Shuffle();
        enemyDeck.Shuffle();
        deck.DrawCard(5);

        // Activate OnNotDrawn Effects
        for (int i = 0; i < deck.drawPile.Count; i++){
            Card target = deck.drawPile[i];
            target.ActivateOnNotDrawnEffects();
        }
    }
    private void OnEnable() {
        EventManager.CardDeath += CardDeath;
    }
    private void OnDisable() {
        EventManager.CardDeath -= CardDeath;
    }
    public void EndGame()
    {
        SoundManager.soundManager.Play("ButtonClick");
        inCombat = false;
        combatUI.LoadOutOfCombat();
        Time.timeScale = 1;
        
        ScenePersistenceManager.scenePersistence.currentCombatAI = null;
            
        if(ScenePersistenceManager.scenePersistence.inTutorial){
            ScenePersistenceManager.scenePersistence.tutorialStage++;
            ScenePersistenceManager.scenePersistence.tutorialDeck.Clear();
            ScenePersistenceManager.scenePersistence.tutorialDeck.AddRange(deck.cards);
        }

        if(combatUI.endCombatText.text == "You won!") {
            if (enemy.isHunter){
                // Switch floor or go to end screen
                if (ScenePersistenceManager.scenePersistence.stages.Count - 1 == ScenePersistenceManager.scenePersistence.currentStage){
                    SceneManager.LoadSceneAsync("End Screen");
                }else{
                    ScenePersistenceManager.scenePersistence.currentStage++;
                    ScenePersistenceManager.scenePersistence.resetMap = true;
                    SceneManager.LoadSceneAsync("Map");
                }
            }
            else{
                SceneManager.LoadSceneAsync("Map");
            }
        }else{
            if (!ScenePersistenceManager.scenePersistence.inTutorial)
            {
                DataPersistenceManager.DataManager.DeleteMostRecentProfileData();
            }
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

        if (gamePhase == 0 && Input.GetKeyUp(KeyCode.E) && enemy != null && (!enemy.huntAI || round <= enemy.huntRounds)){
            StartEnemyTurn();
        }
        if (Input.GetKeyUp(KeyCode.A)){
            DrawPileButton();
        }
        if (Input.GetKeyUp(KeyCode.S)){
            DiscardPileButton();
        }
        if (Input.GetKeyUp(KeyCode.F)){
            GraveButton();
        }

        combatUI.UpdateGraveText();
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

        card.playerCard = cardInCombat.deck.playerDeck;

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

        cardInCombat.card.ActivateOnSummonEffects(cardInCombat);
    }

    #region Game Phases
    public void StartEnemyTurn()
    {
        // Return if the end turn button is pressed too early
        if (timerToNextTurn > 0) return;

        // This is here just in case
        if (gamePhase > 0) return;
        // Invoke the next turn event
        EventManager.NextTurn?.Invoke();
        // Progress the game to the next phase
        gamePhase = 1;

        // Close the active ability menu
        ActiveAbilityManager.activeAbilityManager.Deselect();

        //Close all open displays
        DeckUtilities.CloseAllDisplays();

        // Force draw all cards that are still in draw animation and discard all cards
        deck.ForceDraw();
        deck.DiscardHand();

        // The enemy draws cards
        enemyDeck.DiscardHand();
        enemyDeck.DrawCard(5);
        
        // Activate OnNotDrawn Effects
        for (int i = 0; i < deck.drawPile.Count; i++){
            Card target = deck.drawPile[i];
            target.ActivateOnNotDrawnEffects();
        }

        // The enemy starts its turn
        enemy.StartTurn();
        timerAfterEnemyTurn = 0;
        // Check for active abilities
        if (enemy.canUseActiveAbilities) StartCoroutine(enemy.GetEnemyAI().CheckForActiveSigils());

        startCombatPhase = true;
        timerAfterEnemyTurn += resetTimerAfterEnemyTurnTo;
    }
    void StartCombatPhase()
    {
        // Invoke Next Turn
        EventManager.NextTurn?.Invoke();

        // Activate pre fight sigils
        for (int i = 0; i < 5; i++)
        {
            if (playerCombatCards[i] != null) playerCombatCards[i].card.ActivateOnFightStartEffects(playerCombatCards[i]);
            if (playerBenchCards [i] != null) playerBenchCards [i].card.ActivateOnFightStartEffects(playerBenchCards [i]);
            if (enemyCombatCards [i] != null) enemyCombatCards [i].card.ActivateOnFightStartEffects(enemyCombatCards [i]);
            if (enemyBenchCards  [i] != null) enemyBenchCards  [i].card.ActivateOnFightStartEffects(enemyBenchCards  [i]);
        }

        // Switch the game phase
        gamePhase = 2;

        // Start card combat
        for (int i = 0; i < 5; i++)
        {
            if (playerCombatCards[i] != null && enemyCombatCards[i] != null) CardCombat2Attackers(playerCombatCards[i], enemyCombatCards[i]);
            else if (playerCombatCards[i] != null) DirectHit(playerCombatCards[i]);
            else if (enemyCombatCards[i] != null) DirectHit(enemyCombatCards[i]);
        }

        // Start the next turn timer
        timerToNextTurn = resetTimerTo;
        startPlayerTurn = true;
    }
    void StartPlayerTurn()
    {
        // Increase the round
        round++;
        combatUI.UpdateRoundText();

        // End the combat if its a hunt and the round limit has been reached
        if (enemy.huntAI)
        {
            if (round == enemy.huntRounds + 1){
                GameObject.Find("EndTurnButton").SetActive(false);
                Invoke("WinGame", 2f);
            }
        }

        // Start the turn
        if (gamePhase == 2)
        {
            // Reset the players energy and draw cards
            deck.energy = 3;
            deck.DrawCard(5);

            // Reset the passive text
            for(int i = 0; i < 5 ;i++)
            {
                // Player
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
                // Enemy
                if (enemyBenchCards[i] != null)
                {
                    enemyBenchCards[i].passivesTurnedOnThisTurn = false;
                }
                if (enemyCombatCards[i] != null)
                {
                    enemyCombatCards[i].passivesTurnedOnThisTurn = false;
                }
            }

            // Activate passive sigils
            if (enemyHealth > 0 && (!enemy.huntAI || round <= enemy.huntRounds)){
                for (int i = 0; i < 5; i++)
                {
                    // Player
                    if (playerBenchCards[i] != null && playerBenchCards[i].passivesTurnedOnThisTurn == false && playerBenchCards[i].card.health > 0f) 
                    {
                        playerBenchCards[i].passivesTurnedOnThisTurn = true;
                        playerBenchCards[i].card.ActivateOnTurnStartEffects(playerBenchCards[i]);
                        playerBenchCards[i].UpdateCardAppearance();
                    }
                    if(playerCombatCards[i] != null && playerCombatCards[i].passivesTurnedOnThisTurn == false && playerCombatCards[i].card.health > 0f)
                    {
                        playerCombatCards[i].passivesTurnedOnThisTurn = true;
                        playerCombatCards[i].card.ActivateOnTurnStartEffects(playerCombatCards[i]);
                        playerCombatCards[i].UpdateCardAppearance();
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    // Enemy
                    if (enemyBenchCards[i] != null && enemyBenchCards[i].passivesTurnedOnThisTurn == false && enemyBenchCards[i].card.health > 0f)
                    {
                        enemyBenchCards[i].passivesTurnedOnThisTurn = true;
                        enemyBenchCards[i].card.ActivateOnTurnStartEffects(enemyBenchCards[i]);
                        enemyBenchCards[i].UpdateCardAppearance();
                    }
                    if (enemyCombatCards[i] != null && enemyCombatCards[i].passivesTurnedOnThisTurn == false && enemyCombatCards[i].card.health > 0f)
                    {
                        enemyCombatCards[i].passivesTurnedOnThisTurn = true;
                        enemyCombatCards[i].card.ActivateOnTurnStartEffects(enemyCombatCards[i]);
                        enemyCombatCards[i].UpdateCardAppearance();
                    }
                }
            }

            // Activate OnNotDrawn Effects
            for (int i = 0; i < deck.drawPile.Count; i++){
                Card target = deck.drawPile[i];
                target.ActivateOnNotDrawnEffects();
            }

            // Find the non lost soul cards in the enemies deck
            int enemyNonSoulCards = 0;
            for (int i = 0; i < enemyDeck.cards.Count; i++)
            {
                if (enemyDeck.cards[i].name != "Lost Soul")
                {
                    enemyNonSoulCards++;
                }
            }
            combatUI.UpdateEnemyCardPileText();
            
            // Find the board state of the player and pre turn 
            playerCombatCards.CopyTo(playerCombatCardsAtStartOfTurn, 0);
            playerBenchCards.CopyTo(playerBenchCardsAtStartOfTurn, 0);

            // Switch the phase
            gamePhase = 0;
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
        /*
            Used to start a fight between 2 cards (both cards attack)
        */

        // If neither of the cards has damage return, if only 1 of them has call CardCOmbat1Attacker instead
        if (playerCard.card.attack == 0 && enemyCard.card.attack == 0) return;
        else if (playerCard.card.attack == 0) {CardCombat1Attacker(enemyCard , playerCard, enemyCard.card.attack ); return;}
        else if (enemyCard.card.attack == 0)  {CardCombat1Attacker(playerCard, enemyCard , playerCard.card.attack); return;}

        // Save the old health of the cards
        int oldPlayerHp = playerCard.card.health;
        int oldEnemyHp = enemyCard.card.health;

        // Check if the cards are benched, if only 1 of them is call DirectHit with it
        if (playerCard.benched && enemyCard.benched) return;
        else if (playerCard.benched) { DirectHit(enemyCard ); return;}
        else if (enemyCard.benched)  { DirectHit(playerCard); return;}

        // Reduce health of cards
        playerCard.card.health -= enemyCard.card.attack;
       
        enemyCard.card.health -= playerCard.card.attack;
        enemyCard.lastTypeOfDamage = playerCard.card.typeOfDamage;

        // Generate inaccurate battle data
        playerCard.card.lastBattle = new BattleData(playerCard.card, enemyCard.card, oldPlayerHp, oldEnemyHp, enemyCard);
        enemyCard.card.lastBattle = new BattleData(enemyCard.card, playerCard.card, oldEnemyHp, oldPlayerHp, playerCard);

        // Activate OnTakeDamageEffects
        playerCard.card.ActivateOnTakeDamageEffects(playerCard);
        enemyCard.card.ActivateOnTakeDamageEffects(enemyCard);

        // Switch lastTypeOfDamage with the attacker's attack type
        playerCard.lastTypeOfDamage = enemyCard.card.typeOfDamage;

        // Generate accurate battle data
        playerCard.card.lastBattle = new BattleData(playerCard.card, enemyCard.card, oldPlayerHp, oldEnemyHp, enemyCard);
        enemyCard.card.lastBattle = new BattleData(enemyCard.card, playerCard.card, oldEnemyHp, oldPlayerHp, playerCard);
        
        // Activate OnHitEffects
        playerCard.card.ActivateOnHitEffects(playerCard);
        enemyCard.card.ActivateOnHitEffects(enemyCard);

        // Start the attack animation for the cards
        playerCard.MoveAnimationStarter(0.5f, new Vector3(playerCard.transform.position.x, 1f, 0f), true, playerCard.slot * 0.2f);
        enemyCard. MoveAnimationStarter(0.5f, new Vector3(enemyCard .transform.position.x, 1f, 0f), true, enemyCard.slot  * 0.2f);
    } 
    public void CardCombat1Attacker(CardInCombat attacker, CardInCombat defender, int damage){
        /*
           Used when 1 cards has to attack another (only 1 card attacks not both)
        */

        // Save the old health of the cards
        int oldAttackerHp = attacker.card.health;
        int oldDefenderHp = defender.card.health;
       
       // Reduce health of defender and switch lastTypeOfDamage with the attacker's attack type
        defender.card.health     -= damage;
        defender.lastTypeOfDamage = attacker.card.typeOfDamage;

        // Generate inaccurate battle data
        attacker.card.lastBattle = new BattleData(attacker.card, defender.card, oldAttackerHp, oldDefenderHp, defender);
        defender.card.lastBattle = new BattleData(defender.card, attacker.card, oldDefenderHp, oldAttackerHp, attacker);

        // Activate OnTakeDamageEffects
        defender.card.ActivateOnTakeDamageEffects(defender);

        // Generate accurate battle data
        attacker.card.lastBattle = new BattleData(attacker.card, defender.card, oldAttackerHp, oldDefenderHp, defender);
        defender.card.lastBattle = new BattleData(defender.card, attacker.card, oldDefenderHp, oldAttackerHp, attacker);

        // Activate OnHitEffects
        attacker.card.ActivateOnHitEffects(attacker);

        // Start the attack animation for the attacker
        float animationDelay = 0.5f;
        if (gamePhase == 2) animationDelay = attacker.slot * 0.2f;
        attacker.MoveAnimationStarter(0.5f, new Vector3(attacker.GetSlot().transform.position.x, 1f, 0f), true, animationDelay);
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
        if (timerToNextTurn > resetTimerTo) return;
        timerToNextTurn = 1000f;

        EventManager.CombatEnd?.Invoke();

        if (enemy.isTutorialEnemy) { TutorialWin(); return; }

        for (int i = 0; i < 5; i++)
        {
            if (playerCombatCards[i] != null) playerCombatCards[i].card.ActivateOnBattleEndEffects(playerCombatCards[i]);
            if (playerBenchCards[i] != null) playerBenchCards[i].card.ActivateOnBattleEndEffects(playerBenchCards[i]);
            if (enemyCombatCards[i] != null) enemyCombatCards[i].card.ActivateOnBattleEndEffects(enemyCombatCards[i]);
            if (enemyBenchCards[i] != null) enemyBenchCards[i].card.ActivateOnBattleEndEffects(enemyBenchCards[i]);
        }

        // Clear all existing displays
        DeckUtilities.CloseAllDisplays();
        TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
        combatUI.EndCombat(true);
        deck.cards.AddRange(battleReward);
    }

    void LoseGame()
    {
        if (timerToNextTurn > resetTimerTo) return;
        timerToNextTurn = 1000f;
     
        EventManager.CombatEnd?.Invoke();

        for (int i = 0; i < 5; i++)
        {
            if (playerCombatCards[i] != null) playerCombatCards[i].card.ActivateOnBattleEndEffects(playerCombatCards[i]);
            if (playerBenchCards[i] != null) playerBenchCards[i].card.ActivateOnBattleEndEffects(playerBenchCards[i]);
            if (enemyCombatCards[i] != null) enemyCombatCards[i].card.ActivateOnBattleEndEffects(enemyCombatCards[i]);
            if (enemyBenchCards[i] != null) enemyBenchCards[i].card.ActivateOnBattleEndEffects(enemyBenchCards[i]);
        }

        // Clear all existing displays
        DeckUtilities.CloseAllDisplays();
        TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
        combatUI.EndCombat(false);
        
    }

    void TutorialWin()
    {
        TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
        combatUI.EndCombat(true);
        
        deck.cards.AddRange(battleReward);

        // Clear all existing displays
        DeckUtilities.CloseAllDisplays();
    }
    #endregion
    
    public void CardDeath(Card card){
        if (card.playerCard)
        {
           Card deadCard = ScriptableObject.CreateInstance<Card>();
           deadCard.CopyFrom(card);
           deadCard.ResetCard(); 
           playerCardsLost.Add(deadCard);
        }
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
        // Sets a card to be in a slot
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

    void TurnZero(){
        // Turn zero is used by scripted enemies to play a turn before the next turn button is clicked by the player
        ScriptedEnemy scriptedEnemy = enemy.GetScriptedEnemy();
        if (scriptedEnemy != null){
            if (scriptedEnemy.turnZero){
                scriptedEnemy.PlayTurn(scriptedEnemy.turns[0]);
            }
        }
    }

    public void DrawPileButton(){
        if (deck.drawPile.Count != 0 || DeckUtilities.GetDisplayWithName("deck") != null){
            DeckUtilities.SingularDisplay("deck", deck.drawPile);
        }else{
            NotificationManager.notificationManager.NotifyAutoEnd(noDrawNotification, new Vector3(-700, -90, 0), 2);
        }
    }

    public void DiscardPileButton(){
        if (deck.discardPile.Count != 0 || DeckUtilities.GetDisplayWithName("deck") != null){
            DeckUtilities.SingularDisplay("deck", deck.discardPile);
        }else{
            NotificationManager.notificationManager.NotifyAutoEnd(noDiscardNotification, new Vector3(-700, -90, 0), 2);
        }
    }

    public void GraveButton(){
        if (playerCardsLost.Count != 0 || DeckUtilities.GetDisplayWithName("deck") != null){
            DeckUtilities.SingularDisplay("deck", playerCardsLost);
        }else{
            NotificationManager.notificationManager.NotifyAutoEnd(noGraveNotification, new Vector3(765, -190, 0), 2);
        }
    }
}
