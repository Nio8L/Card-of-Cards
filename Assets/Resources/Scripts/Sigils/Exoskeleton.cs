using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Spell Sigil/Exoskeleton")]
public class Exoskeleton : SpellSigil
{
    public int bonusHealth;

    public GameObject particles;

    public override void OnPlay(CardSlot slot)
    {
        CardInCombat cardInCombat = CombatManager.combatManager.GetCardAtSlot(slot);
        
        cardInCombat.card.health += bonusHealth;
        cardInCombat.UpdateCardAppearance();

        Instantiate(particles, slot.transform.position, Quaternion.identity);
    }

    public override bool CanBePlayed(CardSlot slot, bool player)
    {
        CardInCombat target = CombatManager.combatManager.GetCardAtSlot(slot);

        if(slot.playerSlot != player) return false;

        if(target != null){
            return true;
        }

        return false;
    }
}
