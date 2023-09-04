using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Migrate")]
public class Migrate : Sigil
{
    //public int direction = 1;

    //public override void PasiveEffect(CardInCombat card)
    //{
    //    if (card.slot == 0 && direction == -1)
    //    {
    //        direction = 1;
    //    }
    //    else if (card.slot == 2 && direction == 1)
    //    {
    //        direction = -1;
    //    }


    //    if (card.playerCard && card.deck.combatManager.playerCards[card.slot + direction] != null)
    //    {
    //        direction *= -1;
    //    }
    //    else if (!card.playerCard && card.deck.combatManager.enemyCards[card.slot + direction] != null)
    //    {
    //        direction *= -1;
    //    }
    //    Move(card);

    //}

    //void Move(CardInCombat card)
    //{
    //    if (card.slot + direction < 0 || card.slot + direction >= card.deck.combatManager.playerCombatSlots.Length) return;

    //    if (!card.benched)
    //    {
    //        if (card.playerCard && card.deck.combatManager.playerCards[card.slot + direction] == null)
    //        {
    //            card.deck.combatManager.playerCards[card.slot] = null;
    //            card.deck.combatManager.playerCards[card.slot + direction] = card;
    //            card.slot += direction;
    //            card.MoveAnimationStarter(0.5f, card.deck.combatManager.playerCombatSlots[card.slot].transform.position);
    //            card.deck.PlaySigilAnimation(card.transform, card.card, this);
    //        }
    //        else if (!card.playerCard && card.deck.combatManager.enemyCards[card.slot + direction] == null)
    //        {
    //            card.deck.combatManager.enemyCards[card.slot] = null;
    //            card.deck.combatManager.enemyCards[card.slot + direction] = card;
    //            card.slot += direction;
    //            card.MoveAnimationStarter(0.5f, card.deck.combatManager.enemyCombatSlots[card.slot].transform.position);
    //            card.deck.PlaySigilAnimation(card.transform, card.card, this);
    //        }
    //    }

    //}
}
