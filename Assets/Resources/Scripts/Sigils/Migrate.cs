using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/ActiveSigil/Migrate")]
public class Migrate : ActiveSigil
{
    public override void ActiveEffect(CardInCombat card, List<CardSlot> targets)
    {
        CardSlot targetSlot = targets[0];
        CardInCombat targetCard = CombatManager.combatManager.GetCardAtSlot(targetSlot);

        CardSlot thisSlot = card.GetSlot();

        CombatManager.combatManager.SetCardAtSlot(targetSlot, card);
        CombatManager.combatManager.SetCardAtSlot(thisSlot  , targetCard);

        if (targetCard != null) targetCard.MoveAnimationStarter(0.5f, thisSlot.transform.position, false, 0f);

        SoundManager.soundManager.Play("CardSlide");
        canBeUsed = false;
    }


    public override List<CardSlot> GetPossibleTargets(CardInCombat card)
    {
        List<CardSlot> targets = new List<CardSlot>();
        CardSlot thisSlot = card.GetSlot();

        for (int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
            if (card.playerCard){
                targets.Add(CombatManager.combatManager.playerBenchSlots [i].GetComponent<CardSlot>());
                targets.Add(CombatManager.combatManager.playerCombatSlots[i].GetComponent<CardSlot>());
            }else{
                targets.Add(CombatManager.combatManager.enemyBenchSlots  [i].GetComponent<CardSlot>());
                targets.Add(CombatManager.combatManager.enemyCombatSlots [i].GetComponent<CardSlot>());
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
