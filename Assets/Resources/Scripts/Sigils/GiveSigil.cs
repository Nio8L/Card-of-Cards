using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Sigil/Spell Sigil/Give Sigil")]
public class GiveSigil : SpellSigil
{
    public Sigil sigilToGive;
    public SigilType sigilPermanence;
    public GameObject particles;

    public override void OnPlay(CardSlot slot)
    {
        string sigilName = sigilToGive.name;
        sigilToGive = Instantiate(sigilToGive);
        sigilToGive.name = sigilName;
        sigilToGive.sigilType = sigilPermanence;


        CardInCombat cardInCombat = CombatManager.combatManager.GetCardAtSlot(slot);
        
        cardInCombat.card.sigils.Add(sigilToGive);
        cardInCombat.UpdateCardAppearance();

        if (particles != null) Instantiate(particles, slot.transform.position, Quaternion.identity);
    }

    public override bool CanBePlayed(CardSlot slot, bool player)
    {
        CardInCombat target = CombatManager.combatManager.GetCardAtSlot(slot);
        
        if(target == null){
            return false;
        }

        if (target.card.sigils.Count >= 3) return false;

        if(slot.playerSlot != player) return false;

        return true;
        
    }
}
