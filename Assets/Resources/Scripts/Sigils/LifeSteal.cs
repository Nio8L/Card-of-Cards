using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Life Steal")]
public class LifeSteal : Sigil
{
    public GameObject particles;
    public int heal;
    public override void OnHitEffect(CardInCombat card)
    {

        Instantiate(particles, card.transform.position, Quaternion.identity);
        card.deck.PlaySigilAnimation(card.transform, card.card, this);
        if (card.playerCard){
            CombatManager.combatManager.playerHealth += heal;
            if (CombatManager.combatManager.playerHealth > 20) CombatManager.combatManager.playerHealth = 20;
        }else{
            CombatManager.combatManager.enemyHealth += heal;
            if (CombatManager.combatManager.enemyHealth > CombatManager.combatManager.enemy.maxHealth) CombatManager.combatManager.enemyHealth = CombatManager.combatManager.enemy.maxHealth;
        }
        CombatManager.combatManager.combatUI.UpdateHPText();
    }
}
