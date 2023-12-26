using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;


public class CombatManager : MonoBehaviour, IDataPersistence
{
    public static CombatManager combatManager;

    public bool inCombat;

    public EnemyBase enemy;

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
    public int round = 1;

    void Awake(){
        combatManager = this;
    }
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

        StartGame();
    }

    public void StartGame() 
    {
        if (enemy.huntAI)
        {
            roundText.text = "Round " + round + "/" + enemy.huntRounds;
            for (int i = 0; i < battleReward.Count; i++) roundText.text += battleReward[i] + "\n";
        }
        else roundText.text = "Round " + round;

        enemyCardPileText.text = enemyDeck.cards.Count.ToString();

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
        
        DataPersistenceManager.DataManager.currentCombatAI = null;
            
        if(endCombatText.text == "You won!") {
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
        graveText.text = playerCardsLost.ToString();
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
  
  
    #region Game Phases
    public void StartEnemyTurn()
    {
        if (gamePhase > 0) return;
        gamePhase = 1;

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
        for (int i = 0; i < 3; i++)
        {
            if (playerCombatCards[i] != null) playerCombatCards[i].card.ActivateOnBattleStartEffects(playerCombatCards[i]);
            if (playerBenchCards[i] != null) playerBenchCards[i].card.ActivateOnBattleStartEffects(playerBenchCards[i]);
            if (enemyCombatCards[i] != null) enemyCombatCards[i].card.ActivateOnBattleStartEffects(enemyCombatCards[i]);
            if (enemyBenchCards[i] != null) enemyBenchCards[i].card.ActivateOnBattleStartEffects(enemyBenchCards[i]);
        }

        gamePhase = 2;
        SoundManager.soundManager.Play("CardCombat");
        for (int i = 0; i < 3; i++)
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

        if (enemy.huntAI)
        {
            roundText.text = "Round " + round + "/" + enemy.huntRounds;
            for (int i = 0; i < battleReward.Count; i++) roundText.text += "\n" + battleReward[i].name;
        }
        else roundText.text = "Round " + round;

        if (enemy.huntAI && round == enemy.huntRounds + 1)
        {
            GameObject.Find("EndTurnButton").SetActive(false);
            Invoke("WinGame", 2f);
        }
        
        if (gamePhase == 2)
        {
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
            for (int i = 0; i < 3; i++)
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
            enemyCardPileText.text = enemyNonSoulCards.ToString();

            playerCombatCards.CopyTo(playerCombatCardsAtStartOfTurn, 0);
            playerBenchCards.CopyTo(playerBenchCardsAtStartOfTurn, 0);
            gamePhase = 0;
        }
    }
    #endregion


    #region Attacks
    public void DirectHit(CardInCombat card)
    {
        DirectHit(card, card.card.attack);
    }
    public void DirectHit(CardInCombat card, int damage)
    {
        if (card.benched) return;
        card.MoveAnimationStarter(0.5f, new Vector3(card.transform.position.x, 1f, 0f), true);

        if (card.playerCard)
        {
            enemyHealth -= damage;
            if (enemyHealth <= 0)
            {
                GameObject.Find("EndTurnButton").SetActive(false);
                Invoke("WinGame", 2f);
            }
        }
        else
        {
            playerHealth -= damage;
            if (playerHealth <= 0)
            {
                Invoke("LoseGame", 2f);
            }
        }
        UpdateHPText();
    } 
    public void CardCombat2Attackers(CardInCombat playerCard, CardInCombat enemyCard)
    {
        int oldPlayerHp = playerCard.card.health;
        int oldEnemyHp = enemyCard.card.health;

        if (playerCard.benched && enemyCard.benched) return;
        else if (playerCard.benched) { DirectHit(enemyCard); return;}
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

        if(playerCard.PerformAtackAnimation) playerCard.MoveAnimationStarter(0.5f, new Vector3(playerCard.transform.position.x, 1f, 0f), true);
        if(enemyCard.PerformAtackAnimation)  enemyCard. MoveAnimationStarter(0.5f, new Vector3(enemyCard .transform.position.x, 1f, 0f), true);

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

        if(attacker.PerformAtackAnimation) attacker.MoveAnimationStarter(0.5f, new Vector3(attacker.transform.position.x, 1f, 0f), true);

        attacker.PerformAtackAnimation = true;
    }

    public void UpdateHPText() 
    {
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

        for (int i = 0; i < 3; i++)
        {
            if (playerCombatCards[i] != null) playerCombatCards[i].card.ActivateOnBattleEndEffects(playerCombatCards[i]);
            if (playerBenchCards[i] != null) playerBenchCards[i].card.ActivateOnBattleEndEffects(playerBenchCards[i]);
            if (enemyCombatCards[i] != null) enemyCombatCards[i].card.ActivateOnBattleEndEffects(enemyCombatCards[i]);
            if (enemyBenchCards[i] != null) enemyBenchCards[i].card.ActivateOnBattleEndEffects(enemyBenchCards[i]);
        }

        TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
        endCombatMenu.SetActive(true);
        Time.timeScale = 0;
        endCombatText.text = "You won!";
        deck.cards.AddRange(battleReward);
    }

    void LoseGame()
    {
        timerToNextTurn = 1000f;
     
        for (int i = 0; i < 3; i++)
        {
            if (playerCombatCards[i] != null) playerCombatCards[i].card.ActivateOnBattleEndEffects(playerCombatCards[i]);
            if (playerBenchCards[i] != null) playerBenchCards[i].card.ActivateOnBattleEndEffects(playerBenchCards[i]);
            if (enemyCombatCards[i] != null) enemyCombatCards[i].card.ActivateOnBattleEndEffects(enemyCombatCards[i]);
            if (enemyBenchCards[i] != null) enemyBenchCards[i].card.ActivateOnBattleEndEffects(enemyBenchCards[i]);
        }

        TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
        endCombatMenu.SetActive(true);
        Time.timeScale = 0;
        endCombatText.text = "You lost...";
    }

    void TutorialWin()
    {
        TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
        endCombatMenu.SetActive(true);
        Time.timeScale = 0;
        endCombatText.text = "You beat the tutorial";
        deck.cards.AddRange(battleReward);
    }
    #endregion


    public void ClickOnDialogueBox()
    {
        if (enemy.dialogue != null && enemy.dialogue.NextLineAtClick) enemy.dialogue.NextLine(); 
    }
}
