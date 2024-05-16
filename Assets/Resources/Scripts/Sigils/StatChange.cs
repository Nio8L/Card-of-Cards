using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Spell Sigil/Stat Change")]
public class StatChange : SpellSigil
{
    public bool permanentBuff;
    public int bonusHealth;
    public int bonusAttack;

    public GameObject particles;

    public override void OnPlay(CardSlot slot)
    {
        CardInCombat cardInCombat = CombatManager.combatManager.GetCardAtSlot(slot);
        
        if (permanentBuff){
            cardInCombat.card.maxHealth     += bonusHealth;
            cardInCombat.card.health        += bonusHealth;
            cardInCombat.card.defaultAttack += bonusAttack;
            cardInCombat.card.attack        += bonusAttack;
        }else{
            cardInCombat.card.health += bonusHealth;
            cardInCombat.card.attack += bonusAttack;
        }
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
