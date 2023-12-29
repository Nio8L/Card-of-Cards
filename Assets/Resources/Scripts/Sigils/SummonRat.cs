using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/ActiveSigil/SummonRatExample")]
public class SummonRat : ActiveSigil
{
    public Card rat;

    public override void ActiveEffect(CardInCombat card, List<CardSlot> targets)
    {
        CardSlot targetSlot = targets[0];
        CombatManager.combatManager.PlayCard(Instantiate(rat).ResetCard(), targetSlot, false);
        canBeUsed = false;
    }
    public override List<CardSlot> GetPossibleTargets(CardInCombat card)
    {
        List<CardSlot> targets = new List<CardSlot>();
        CardSlot thisSlot = card.GetSlot();

        for (int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
            CardSlot slot;
            if (card.playerCard){
                slot = CombatManager.combatManager.playerBenchSlots [i].GetComponent<CardSlot>();
                if (CombatManager.combatManager.GetCardAtSlot(slot) == null) targets.Add(slot);

                slot = CombatManager.combatManager.playerCombatSlots[i].GetComponent<CardSlot>();
                if (CombatManager.combatManager.GetCardAtSlot(slot) == null) targets.Add(slot);

            }else{
                slot = CombatManager.combatManager.enemyBenchSlots  [i].GetComponent<CardSlot>();
                if (CombatManager.combatManager.GetCardAtSlot(slot) == null) targets.Add(slot);

                slot = CombatManager.combatManager.enemyCombatSlots [i].GetComponent<CardSlot>();
                if (CombatManager.combatManager.GetCardAtSlot(slot) == null) targets.Add(slot);

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
