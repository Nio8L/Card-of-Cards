using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntManager : MonoBehaviour
{
    public GameObject huntShield;
    CardInCombat[] playerCards = new CardInCombat[5];
    bool[] enemyCards = new bool[5];
    public Card[] strongCards;
    public ScriptedEnemy.Turn PlayStrongCards(){
        ScriptedEnemy.Turn turn = new ScriptedEnemy.Turn();
        playerCards = CombatManager.combatManager.playerCombatCards;
        // Check if there were cards at all player combat slots to see if they have directly hit last turn
        for (int i = 0; i < playerCards.Length; i++){
            if (CheckColumb(i)){
                // Place a card in turn
                turn.benchCards[i] = PickCard();

                // Instantiate hunt shield
                Instantiate(huntShield, CombatManager.combatManager.enemyBenchSlots[i].transform.position, Quaternion.identity);
            }
        }
        return turn;
    }

    Card PickCard(){
        // Returns a random card from the strong card array
        return strongCards[(int)(Random.value * strongCards.Length)];
    }

    bool CheckColumb(int col){
        // Returns true if the columb col has a player card in the combat slot that has more than 0 damage and if there is no enemy
        if (playerCards[col] != null && playerCards[col].card.attack > 0 && CombatManager.combatManager.enemyBenchCards[col] == null && CombatManager.combatManager.enemyCombatCards[col] == null){
            return true;
        }
        return false;
    }
}
