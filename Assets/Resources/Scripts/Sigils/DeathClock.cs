using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Active Sigil/Death Clock")]
public class DeathClock : ActiveSigil
{
    public override void OnSummonEffect(CardInCombat card)
    {
        canBeUsed = true;
    }

    public override void ActiveEffect(CardInCombat card, List<CardSlot> targets)
    {
        CardInCombat target = CombatManager.combatManager.GetCardAtSlot(targets[0]);
        bool poison = false, bite = false, scratch = false;
        for (int i = 0; i < target.card.injuries.Count; i++){
            if(target.card.injuries[i] == Card.TypeOfDamage.Poison){
                
                poison = true;
            
            }else if(target.card.injuries[i] == Card.TypeOfDamage.Scratch){
                
                scratch = true;
            
            }else{
                
                bite = true;
            }
        }
        if(target.card.injuries.Count == 1){
            
            if(poison){
                
                target.card.injuries.Add(Card.TypeOfDamage.Scratch);
            
            }else if(scratch){
                
                target.card.injuries.Add(Card.TypeOfDamage.Bite);
            
            }else{
                
                target.card.injuries.Add(Card.TypeOfDamage.Poison);
            }
            target.card.injuries.RemoveAt(0);
        
        }else if(target.card.injuries.Count == 2){
            if (poison && scratch){
                target.card.injuries = new()
                {
                    Card.TypeOfDamage.Scratch,
                    Card.TypeOfDamage.Bite
                };
            }else if(poison && bite){
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

    public override void OnTurnStartEffect(CardInCombat card)
    {
        canBeUsed = true;
    }
}
