using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Active Sigil/Ignite")]
public class Ignite : ActiveSigil
{
    public int ignitionDuration;
    public override void ActiveEffect(CardInCombat card, List<CardSlot> targets)
    {
        for (int i = 0; i < neededTargets; i++){
            CardSlot targetSlot = targets[i];

            targetSlot.IgniteSlot(ignitionDuration);
        }

        SoundManager.soundManager.Play("Ignite");

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

    public override void OnSummonEffect(CardInCombat card)
    {
        canBeUsed = true;
    }

    public override void OnTurnStartEffect(CardInCombat card)
    {
        canBeUsed = true;
    }
}
