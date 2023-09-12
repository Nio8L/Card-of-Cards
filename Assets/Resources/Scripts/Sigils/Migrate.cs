using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Migrate")]
public class Migrate : Sigil
{
    CardInCombat cardToMove;

    public override void PasiveEffect(CardInCombat card)
    {
        canUseAbility = true;
        card.deck.ShowSigilStar(card, this);
    }

    public override bool ActiveSigilStart(CardInCombat card) 
    {
        if (!canUseAbility) return false;
        cardToMove = card;
        return true;
    }

    public override bool TryToEndActiveSigil(CardSlot slot, CombatManager combatManager) 
    {
        if (!canUseAbility) return false;
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

            canUseAbility = false;

            combatManager.deck.ShowSigilStar(cardToMove, this);
            return true;
        }

        return false;
    }
}
