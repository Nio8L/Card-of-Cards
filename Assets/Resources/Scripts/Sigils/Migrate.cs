using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Active Sigil/Migrate")]
public class Migrate : ActiveSigil
{
    public override void ActiveEffect(CardInCombat card, List<CardSlot> targets)
    {
        CardSlot targetSlot = targets[0];
        CardInCombat targetCard = CombatManager.combatManager.GetCardAtSlot(targetSlot);

        CardSlot thisSlot = card.GetSlot();

        CombatManager.combatManager.SetCardAtSlot(targetSlot, card);
        CombatManager.combatManager.SetCardAtSlot(thisSlot  , targetCard);

        if (!card.playerCard) card.MoveAnimationStarter(0.5f, targetSlot.transform.position, false, 0f);
        if (targetCard != null){
            AnimationUtilities.CancelAnimations(targetCard.gameObject);
            targetCard.MoveAnimationStarter(0.5f, thisSlot.transform.position, false, 0f);
        }

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

    public override void OnSummonEffect(CardInCombat card)
    {
        canBeUsed = true;
    }

    public override void OnTurnStartEffect(CardInCombat card)
    {
        canBeUsed = true;
    }
}
