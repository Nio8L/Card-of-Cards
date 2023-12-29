using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/ActiveSigil/Ignite")]
public class Ignite : ActiveSigil
{
    public int ignitionDuration;

    [Header("Possible ignitions")]
    public bool playerCombat = false;
    public bool playerBench = false;
    public bool enemyCombat = false;
    public bool enemyBench = false;

    public override void ActiveEffect(CardInCombat card, List<CardSlot> targets)
    {
        CardSlot targetSlot = targets[0];

        targetSlot.IgniteSlot(ignitionDuration);

        canBeUsed = false;
    }

    public override List<CardSlot> GetPossibleTargets(CardInCombat card)
    {
        List<CardSlot> targets = new List<CardSlot>();

        if (playerBench)
        {
            for(int i = 0; i < CombatManager.combatManager.playerBenchSlots.Length; i++)
            {
                targets.Add(CombatManager.combatManager.playerBenchSlots[i].GetComponent<CardSlot>());
            }
        }

        if (playerCombat)
        {
            for(int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++)
            {
                targets.Add(CombatManager.combatManager.playerCombatSlots[i].GetComponent<CardSlot>());
            }
        }

        if (enemyBench)
        {
            for(int i = 0; i < CombatManager.combatManager.enemyBenchSlots.Length; i++)
            {
                targets.Add(CombatManager.combatManager.enemyBenchSlots[i].GetComponent<CardSlot>());
            }
        }

        if (enemyCombat)
        {
            for(int i = 0; i < CombatManager.combatManager.enemyCombatSlots.Length; i++)
            {
                targets.Add(CombatManager.combatManager.enemyCombatSlots[i].GetComponent<CardSlot>());
            }
        }

        
        //targets.Remove(thisSlot);
        return targets;
    }
    public override void OnSummonEffects(CardInCombat card)
    {
        canBeUsed = true;

        description = "Ignite a slot for " + ignitionDuration + " turns.";
    }

    public override void PasiveEffect(CardInCombat card)
    {
        canBeUsed = true;
    }
}
