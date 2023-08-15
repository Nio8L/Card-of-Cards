using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Migrate")]
public class Migrate : Sigil
{
    public int direction = 1;

    public override void PasiveEffect(CardInCombat card)
    {
        if (card.slot == 0 && direction == -1)
        {
            direction = 1;
        }
        else if (card.slot == 2 && direction == 1)
        {
            direction = -1;
        }

        if (card.deck.combatManager.playerCards[card.slot + direction] != null)
        {
            direction *= -1;
        }
        Move(card);

    }

    void Move(CardInCombat card)
    {
        if (!card.benched)
        {
            card.deck.combatManager.playerCards[card.slot] = null;
            card.deck.combatManager.playerCards[card.slot + direction] = card;
            card.slot += direction;
            card.transform.position = card.deck.combatManager.playerCombatSlots[card.slot].transform.position;
        }

    }
}
