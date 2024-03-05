using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Spell Sigil/Second Chance")]
public class SecondChance : SpellSigil
{
    public Notification notInjuredNotification;
    public override void OnPlay(CardSlot slot)
    {
        CardInCombat cardInCombat = CombatManager.combatManager.GetCardAtSlot(slot);        

        Card healedCard = cardInCombat.card;

        healedCard.AcceptLostSoul();
        CombatManager.combatManager.deck.UpdateCardAppearance(cardInCombat.transform, healedCard);

        AnimationUtilities.LostSoulAnimation(cardInCombat.transform); 
    }

    public override bool CanBePlayed(CardSlot slot, bool player)
    {
        CardInCombat target = CombatManager.combatManager.GetCardAtSlot(slot);

        if (slot.playerSlot != player) return false;

        if (target != null && target.card.injuries.Count > 0){
            return true;
        }
        NotificationManager.notificationManager.NotifyAutoEnd(notInjuredNotification, new Vector3(-700, 0, 0), 2);
        return false;
    }
}
