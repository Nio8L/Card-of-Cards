using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/Active Decider/Ignite")]
public class IgniteDecider : Decider
{
    public override List<CardSlot> GetSlots(int neededTargets)
    {
        List<CardInCombat> targets = new List<CardInCombat>();
        for (int i = 0; i < 5; i++){
            CardInCombat bench = CombatManager.combatManager.playerBenchCards[i];
            if (bench != null && bench.GetSlot().status != CardSlot.Status.Ignited){
                targets.Add(bench);
            }
            CardInCombat combat = CombatManager.combatManager.playerCombatCards[i];
            if (combat != null && combat.GetSlot().status != CardSlot.Status.Ignited){
                targets.Add(combat);
            }
        }

        if (targets.Count < neededTargets) return null;

        SortList(targets);

        List<CardSlot> targetSlots = new List<CardSlot>();
        for (int i = 0; i < targets.Count; i++){
            targetSlots.Add(targets[i].GetSlot());
        }

        return targetSlots;
    }

    void SortList(List<CardInCombat> cards){
        // Sort the list of cards with DIRECT INSERTION
        for (int x = 0; x < cards.Count; x++){
            for (int y = x; y < cards.Count; y++){
                if (cards[x].card.health < cards[y].card.health){
                    CardInCombat store = cards[x];
                    cards[x] = cards[y];
                    cards[y] = store;
                }
            }
        }
    }
}
