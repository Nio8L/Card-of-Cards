using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Active Sigil/Death Clock")]
public class DeathClock : ActiveSigil
{
    public override void OnSummonEffects(CardInCombat card)
    {
        canBeUsed = true;
    }

    public override void ActiveEffect(CardInCombat card, List<CardSlot> targets)
    {
        CardInCombat target = CombatManager.combatManager.GetCardAtSlot(targets[0]);
        if(target.card.injuries.Count == 1){
            
            if(target.card.injuries[0] == Card.TypeOfDamage.Poison){
                
                target.card.injuries.Add(Card.TypeOfDamage.Scratch);
            
            }else if(target.card.injuries[0] == Card.TypeOfDamage.Scratch){
                
                target.card.injuries.Add(Card.TypeOfDamage.Bite);
            
            }else{
                
                target.card.injuries.Add(Card.TypeOfDamage.Poison);
            }
            target.card.injuries.RemoveAt(0);
        
        }else if(target.card.injuries.Count == 2){
            if(target.card.injuries.Contains(Card.TypeOfDamage.Poison) && card.card.injuries.Contains(Card.TypeOfDamage.Scratch)){
                target.card.injuries = new()
                {
                    Card.TypeOfDamage.Scratch,
                    Card.TypeOfDamage.Bite
                };
            }else if(target.card.injuries.Contains(Card.TypeOfDamage.Poison) && card.card.injuries.Contains(Card.TypeOfDamage.Bite)){
                target.card.injuries = new()
                {
                    Card.TypeOfDamage.Scratch,
                    Card.TypeOfDamage.Poison
                };
            }else{
                target.card.injuries = new()
                {
                    Card.TypeOfDamage.Bite,
                    Card.TypeOfDamage.Poison
                };
            }
        }

        SoundManager.soundManager.Play("DeathClock");

        target.deck.UpdateCardAppearance(target.transform, target.card);

        canBeUsed = false;
    }

    public override List<CardSlot> GetPossibleTargets(CardInCombat card)
    {
        List<CardSlot> targets = new List<CardSlot>();

        for (int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
            if (card.playerCard){
                if (CombatManager.combatManager.playerBenchCards[i] != null){
                    if (CombatManager.combatManager.playerBenchCards[i].GetComponent<CardInCombat>().card.injuries.Count > 0){
                        targets.Add(CombatManager.combatManager.playerBenchSlots [i].GetComponent<CardSlot>());
                    }
                }
                if (CombatManager.combatManager.playerCombatCards[i] != null){
                    if (CombatManager.combatManager.playerCombatCards[i].GetComponent<CardInCombat>().card.injuries.Count > 0){
                        targets.Add(CombatManager.combatManager.playerCombatSlots[i].GetComponent<CardSlot>());
                    }
                }
                if (CombatManager.combatManager.enemyBenchCards[i] != null){
                    if (CombatManager.combatManager.enemyBenchCards[i].GetComponent<CardInCombat>().card.injuries.Count > 0){
                        targets.Add(CombatManager.combatManager.enemyBenchSlots  [i].GetComponent<CardSlot>());
                    }
                }
                if (CombatManager.combatManager.enemyCombatCards[i] != null){
                    if (CombatManager.combatManager.enemyCombatCards[i].GetComponent<CardInCombat>().card.injuries.Count > 0){
                        targets.Add(CombatManager.combatManager.enemyCombatSlots [i].GetComponent<CardSlot>());
                    }
                } 
            }
        }
        return targets;
    }
}
