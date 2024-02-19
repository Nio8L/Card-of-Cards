using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(menuName = "Sigil/Consuming Sigil/Explode")]
public class Explode : ConsumingSigil
{
    public int damageIncrement;

    public int damageToDeal = 0;

    public int cardsPerDamageIncrement = 1;

    public bool splashDamage = false;

    public GameObject thornsParticlesPrefab;
    public override void OnConsumeEffect(CardInCombat card, Card consumedCard)
    {
        base.OnConsumeEffect(card, consumedCard);
        

        cardAcceptor.AcceptCard(consumedCard);
        card.deck.PlaySigilAnimation(card.transform, card.card, this);

        damageToDeal = damageIncrement * (cardAcceptor.cardsAccepted.Count / cardsPerDamageIncrement);
        description = "Upon death deal " + damageToDeal + " damage. Increase this damage by feeding cards. (" + cardAcceptor.cardsAccepted.Count % cardsPerDamageIncrement + "/" + cardsPerDamageIncrement + ")";
    }

    public override void OnDeadEffect(CardInCombat card)
    {
        base.OnDeadEffect(card);
        SoundManager.soundManager.Play("Explode");

        Instantiate(thornsParticlesPrefab, card.transform.position, Quaternion.identity);
        
        if(!splashDamage){
            card.card.lastBattle.enemyCard.health -= damageToDeal;
        }else{
            // Deal damage to all non friendly cards
            if (card.playerCard){
                // Kill enemies
                for (int i = 0; i < CombatManager.combatManager.enemyCombatSlots.Length; i++){
                        if (CombatManager.combatManager.enemyCombatCards[i] != null){
                            CombatManager.combatManager.enemyCombatCards[i].card.health -= damageToDeal;
                        }
                        if (CombatManager.combatManager.enemyBenchCards[i] != null){
                            CombatManager.combatManager.enemyBenchCards[i].card.health -= damageToDeal;
                        }
                }
            }else{
                // Kill player
                for (int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
                    if (CombatManager.combatManager.playerCombatCards[i] != null){
                        CombatManager.combatManager.playerCombatCards[i].card.health -= damageToDeal;
                    }
                    if (CombatManager.combatManager.playerBenchCards[i] != null){
                        CombatManager.combatManager.playerBenchCards[i].card.health -= damageToDeal;
                    }
                }
            }
        }

        damageToDeal = 0;

        description = "Upon death deal " + damageToDeal + " damage. Increase this damage by feeding cards. (" + cardAcceptor.cardsAccepted.Count % cardsPerDamageIncrement + "/" + cardsPerDamageIncrement + ")";
        card.deck.PlaySigilAnimation(card.transform, card.card, this);
    }
}
