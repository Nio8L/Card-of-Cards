using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/ActiveSigil/EarlyAttackExample")]
public class EarlyAttackExample : ActiveSigil
{
    public override void ActiveEffect(CardInCombat card, List<CardSlot> targets)
    {
        CardSlot targetSlot = targets[0];
        CardInCombat targetCard = CombatManager.combatManager.GetCardAtSlot(targetSlot);
        CombatManager.combatManager.CardCombat1Attacker(card, targetCard, card.card.attack);

        CombatManager.combatManager.deck.UpdateCardAppearance(card.transform      , card.card      );
        CombatManager.combatManager.deck.UpdateCardAppearance(targetCard.transform, targetCard.card);

        canBeUsed = false;
    }
    public override List<CardSlot> GetPossibleTargets(CardInCombat card)
    {
       List<CardSlot> targets = new List<CardSlot>();
        CardSlot thisSlot = card.GetSlot();

        for (int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
            CardSlot slot;
            if (!card.playerCard){
                slot = CombatManager.combatManager.playerBenchSlots [i].GetComponent<CardSlot>();
                if (CombatManager.combatManager.GetCardAtSlot(slot) != null) targets.Add(slot);

                slot = CombatManager.combatManager.playerCombatSlots[i].GetComponent<CardSlot>();
                if (CombatManager.combatManager.GetCardAtSlot(slot) != null) targets.Add(slot);

            }else{
                slot = CombatManager.combatManager.enemyBenchSlots  [i].GetComponent<CardSlot>();
                if (CombatManager.combatManager.GetCardAtSlot(slot) != null) targets.Add(slot);

                slot = CombatManager.combatManager.enemyCombatSlots [i].GetComponent<CardSlot>();
                if (CombatManager.combatManager.GetCardAtSlot(slot) != null) targets.Add(slot);

            }
        }
        targets.Remove(thisSlot);
        return targets;
    }
    public override void OnSummonEffects(CardInCombat card)
    {
        canBeUsed = true;
    }

    public override void PasiveEffect(CardInCombat card)
    {
        canBeUsed = true;
    }
}
