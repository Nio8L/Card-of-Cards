using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Migrate")]
public class Migrate : Sigil
{
    CardInCombat cardToMove;
    bool canMove = true;

    public override bool ActiveSigilStart(CardInCombat card) 
    {
        if (!canMove) return false;
        cardToMove = card;
        return true;
    }

    public override void OnDeadEffects(CardInCombat card)
    {
        canMove = true;
    }

    public override bool TryToEndActiveSigil(CardSlot slot, CombatManager combatManager) 
    {
        if (!canMove) return false;
        CardInCombat[] firstCardCollection;//collection of cardToMove
        CardInCombat[] secondCardCollection;//collection of clicked card

        if (slot.playerSlot)
        {
            firstCardCollection = cardToMove.benched ? combatManager.playerBenchCards : combatManager.playerCombatCards;
            secondCardCollection = slot.bench ? combatManager.playerBenchCards : combatManager.playerCombatCards;
        }
        else
        {
            firstCardCollection = cardToMove.benched ? combatManager.enemyBenchCards : combatManager.enemyCombatCards;
            secondCardCollection = slot.bench ? combatManager.enemyBenchCards : combatManager.enemyCombatCards;
        }

        if (secondCardCollection[slot.slot] != cardToMove) 
        {
            firstCardCollection[cardToMove.slot] = secondCardCollection[slot.slot];
            secondCardCollection[slot.slot] = cardToMove;

            if (firstCardCollection[cardToMove.slot] != null)
            {
                firstCardCollection[cardToMove.slot].slot = cardToMove.slot;
                firstCardCollection[cardToMove.slot].transform.position = cardToMove.transform.position;
                firstCardCollection[cardToMove.slot].benched = cardToMove.benched;
            }

            cardToMove.slot = slot.slot;
            cardToMove.transform.position = slot.transform.position;
            cardToMove.benched = slot.bench;

            canMove = false;

            return true;
        }

        return false;
    }
}
