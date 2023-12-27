using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Migrate")]
public class Migrate : Sigil
{
    CardInCombat cardToMove;
    bool secondUse = false;
    public override void PasiveEffect(CardInCombat card)
    {
        canUseAbility = true;
        card.ShowSigilStar(this);
        if (card.card.captain) secondUse = true;
    }

    public override bool ActiveSigilStart(CardInCombat card) 
    {
        if (!canUseAbility) return false;
        cardToMove = card;
        return true;
    }

    public override bool TryToEndActiveSigil(CardSlot slot) 
    {
        if (!canUseAbility) return false;
        CardInCombat[] firstCardCollection;//collection of cardToMove
        CardInCombat[] secondCardCollection;//collection of clicked card

        if (slot.playerSlot)
        {
            firstCardCollection = cardToMove.benched ? CombatManager.combatManager.playerBenchCards : CombatManager.combatManager.playerCombatCards;
            secondCardCollection = slot.bench ? CombatManager.combatManager.playerBenchCards : CombatManager.combatManager.playerCombatCards;
        }
        else
        {
            firstCardCollection = cardToMove.benched ? CombatManager.combatManager.enemyBenchCards : CombatManager.combatManager.enemyCombatCards;
            secondCardCollection = slot.bench ? CombatManager.combatManager.enemyBenchCards : CombatManager.combatManager.enemyCombatCards;
        }

        if (secondCardCollection[slot.slot] != cardToMove) 
        {
            firstCardCollection[cardToMove.slot] = secondCardCollection[slot.slot];
            secondCardCollection[slot.slot] = cardToMove;

            if (firstCardCollection[cardToMove.slot] != null)
            {
                firstCardCollection[cardToMove.slot].slot = cardToMove.slot;
                firstCardCollection[cardToMove.slot].MoveAnimationStarter(0.5f, cardToMove.transform.position, false, 0f);
                firstCardCollection[cardToMove.slot].benched = cardToMove.benched;
            }

            cardToMove.slot = slot.slot;
            cardToMove.MoveAnimationStarter(0.5f, slot.transform.position, false, 0f);
            cardToMove.benched = slot.bench;


            if (!secondUse) canUseAbility = false;
            else secondUse = false;


            cardToMove.ShowSigilStar(this);
            return true;
        }

        return false;
    }
}
