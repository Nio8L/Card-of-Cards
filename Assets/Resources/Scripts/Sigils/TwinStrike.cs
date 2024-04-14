using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Twin Strike")]
public class TwinStrike : Sigil
{
    bool activatedThisTurn = false;
    public override void OnHitEffect(CardInCombat card)
    {
        if (activatedThisTurn) return;

        card.deck.PlaySigilAnimation(card.transform, card.card, this);
        activatedThisTurn = true;

        if (card.playerCard && CombatManager.combatManager.enemyCombatCards[card.slot] != null)
        {
            CardInCombat cardToHit = CombatManager.combatManager.enemyCombatCards[card.slot];
            CombatManager.combatManager.CardCombat1Attacker(card, cardToHit, card.card.attack);
        }
        else if (!card.playerCard && CombatManager.combatManager.playerCombatCards[card.slot] != null)
        {
            CardInCombat cardToHit = CombatManager.combatManager.playerCombatCards[card.slot];
            CombatManager.combatManager.CardCombat1Attacker(card, cardToHit, card.card.attack);
        }
        
    }

    public override void OnFightStartEffect(CardInCombat card)
    {
        activatedThisTurn = false;
    }
}
