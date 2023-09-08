using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil")]
public class Sigil : ScriptableObject
{
    public string sigilName;
    public bool negative;
    public Sprite image;
    public string description;

    [HideInInspector]
    public bool canUseAbility = false;
    CardSlot selectedSlot;

    public virtual void PasiveEffect(CardInCombat card) 
    {
        //effect should be here
    }

    public virtual void ApplyOnHitEffect(CardInCombat card)
    {
        //effect should be here
    }

    public virtual void OnTakeDamageEffect(CardInCombat card)
    {
        //effect should be here
    }

    public virtual void OnSummonEffects(CardInCombat card) 
    {
        //maj efecto trebe da e tuka
    }

    public virtual void OnDeadEffects(CardInCombat card) 
    {
        //pisna mi da addvam random effect types
    }

    public virtual bool ActiveSigilStart(CardInCombat card) 
    {
        return false;//returns true if it hs a second stage
    }

    public virtual bool TryToEndActiveSigil(CardSlot slot, CombatManager combatManager)
    {
        return true;//returns true if the second stage ends
	}

    public virtual void OnBattleStartEffects(CardInCombat card)
    {
        //Tyjno marti trqbvat ni
    }
}
