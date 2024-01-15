using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Active Sigil/Ignite")]
public class Ignite : ActiveSigil
{
    public int ignitionDuration;
    public override void ActiveEffect(CardInCombat card, List<CardSlot> targets)
    {
        for (int i = 0; i < targets.Count; i++){
            CardSlot targetSlot = targets[i];

            targetSlot.IgniteSlot(ignitionDuration);
        }
        canBeUsed = false;
    }

    public override List<CardSlot> GetPossibleTargets(CardInCombat card)
    {
        List<CardSlot> targets = new List<CardSlot>();

        for(int i = 0; i < CombatManager.combatManager.playerBenchSlots.Length; i++)
        {
            targets.Add(CombatManager.combatManager.playerBenchSlots[i].GetComponent<CardSlot>());
            targets.Add(CombatManager.combatManager.playerCombatSlots[i].GetComponent<CardSlot>());
            targets.Add(CombatManager.combatManager.enemyBenchSlots[i].GetComponent<CardSlot>());
            targets.Add(CombatManager.combatManager.enemyCombatSlots[i].GetComponent<CardSlot>());
        }
        
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
