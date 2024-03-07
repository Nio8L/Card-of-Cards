using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Active Sigil/Hopper")]
public class Hopper : ActiveSigil
{
    public override void ActiveEffect(CardInCombat card, List<CardSlot> targets)
    {
        canBeUsed = false;
        CardSlot targetSlot = targets[0];
        CardSlot thisSlot = card.GetSlot();

        CombatManager.combatManager.SetCardAtSlot(targetSlot, card);
        CombatManager.combatManager.SetCardAtSlot(thisSlot, null);

        if(!targetSlot.bench){
            if(card.playerCard){
                if(CombatManager.combatManager.enemyCombatCards[card.slot] != null){
                    AnimationUtilities.CancelAnimations(card.gameObject);
                    CombatManager.combatManager.CardCombat1Attacker(card, CombatManager.combatManager.enemyCombatCards[card.slot], card.card.attack);
                    if(CombatManager.combatManager.enemyCombatCards[card.slot].card.health <= 0){
                        canBeUsed = true;
                    }
                }
            }else{
                card.MoveAnimationStarter(0.5f, targetSlot.transform.position, false, 0f);
                if(CombatManager.combatManager.playerCombatCards[card.slot] != null){
                    CombatManager.combatManager.CardCombat1Attacker(card, CombatManager.combatManager.playerCombatCards[card.slot], card.card.attack);
                    if(CombatManager.combatManager.playerCombatCards[card.slot].card.health <= 0){
                        canBeUsed = true;
                    }
                }
            }
            CombatManager.combatManager.enemyCombatCards[card.slot].deck.UpdateCardAppearance(card.transform, card.card);
        }

        SoundManager.soundManager.Play("CardSlide");
    }

    public override List<CardSlot> GetPossibleTargets(CardInCombat card){
        List<CardSlot> targets = new List<CardSlot>();

        for(int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
            if(card.playerCard){
                if(CombatManager.combatManager.playerCombatCards[i] == null){
                    targets.Add(CombatManager.combatManager.playerCombatSlots[i].GetComponent<CardSlot>());
                }
            }else{
                if(CombatManager.combatManager.enemyCombatCards[i] == null){
                    targets.Add(CombatManager.combatManager.enemyCombatSlots[i].GetComponent<CardSlot>());
                }
            }
        }

        return targets;
    }

    public override void OnTurnStartEffect(CardInCombat card)
    {
        canBeUsed = true;
    }

    public override void OnSummonEffect(CardInCombat card)
    {
       canBeUsed = true;
    }
}
