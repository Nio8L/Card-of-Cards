using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;

public class CombatUI : MonoBehaviour
{
    public GameObject endCombatMenu;
    public TextMeshProUGUI endCombatText;

    public GameObject playerHealthRect;
    public GameObject playerHealthDash;
    public TextMeshProUGUI playerHealthText;   
    public GameObject enemyHealthRect;
    public GameObject enemyHealthDash;
    public TextMeshProUGUI enemyHealthText;
    public TextMeshProUGUI graveText;
    public TextMeshProUGUI roundText;
    public TextMeshProUGUI enemyCardPileText;
    public TextMeshProUGUI drawPileText;
    public TextMeshProUGUI discardPileText;

    public void BeginCombat(){
        endCombatMenu.SetActive(false);

        TotemManager.LoadScene();
        
        UpdateRoundText();

        UpdateEnemyCardPileText();
    }

    public void LoadOutOfCombat(){
        endCombatMenu.SetActive(false);
    }

    public void EndCombat(bool victory){
        endCombatMenu.SetActive(true);

        if(victory){
            if(CombatManager.combatManager.enemy.isTutorialEnemy && ScenePersistenceManager.scenePersistence.tutorialStage == 3){
                endCombatText.text = "You beat the tutorial!";
            }else{
                endCombatText.text = "You won!";
            }
        }else{
            endCombatText.text = "You lost...";
        }
    }

    public void UpdateHPText() 
    {
        float playerVal = CombatManager.combatManager.playerHealth / 20f;
        float enemyVal = CombatManager.combatManager.enemyHealth / (float)CombatManager.combatManager.enemy.maxHealth;

        if (playerVal <= 0) playerVal = 0;
        if (enemyVal <= 0) enemyVal = 0;

        playerHealthRect.transform.localScale =    new Vector3(playerVal, 1, 1);
        playerHealthDash.transform.localPosition = new Vector3(playerVal * 200 - 150, 1, 1);
        enemyHealthRect.transform.localScale =    new Vector3(enemyVal, 1, 1);
        enemyHealthDash.transform.localPosition = new Vector3(enemyVal * 200 - 150, 1, 1);

        playerHealthText.text = CombatManager.combatManager.playerHealth + "/" + 20;
        enemyHealthText.text = CombatManager.combatManager.enemyHealth + "/" + CombatManager.combatManager.enemy.maxHealth;
    }

    public void UpdateGraveText(){
        graveText.text = CombatManager.combatManager.playerCardsLost.Count.ToString();
    }

    public void UpdateRoundText(){
        if (CombatManager.combatManager.enemy.huntAI)
        {
            if (CombatManager.combatManager.round == CombatManager.combatManager.enemy.huntRounds + 1)
            {
                roundText.text = "Round " + (CombatManager.combatManager.round - 1) + "/" + CombatManager.combatManager.enemy.huntRounds;
            }else{
                roundText.text = "Round " + CombatManager.combatManager.round + "/" + CombatManager.combatManager.enemy.huntRounds;
            }
            for (int i = 0; i < CombatManager.combatManager.battleReward.Count; i++){
                roundText.text += "\n" + CombatManager.combatManager.battleReward[i].name;
            }
        }
        else roundText.text = "Round " + CombatManager.combatManager.round;
    }

    public void UpdateEnemyCardPileText(){
        enemyCardPileText.text = CombatManager.combatManager.enemyDeck.cards.Count.ToString();
    }

    public void UpdateHuntText(Card card){
        UpdateRoundText();
    }
    
    public void UpdatePileNumbers()
    {
        drawPileText.text    = CombatManager.combatManager.deck.drawPile.Count + "";
        discardPileText.text = CombatManager.combatManager.deck.discardPile.Count + "";
    }

    void OnEnable(){
        EventManager.CardInjured += UpdateHuntText;
    }

    void OnDisable(){
        EventManager.CardInjured -= UpdateHuntText;
    }
}
