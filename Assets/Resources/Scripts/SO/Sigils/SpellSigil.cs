using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Spell Sigil")]
public class SpellSigil : Sigil
{
    public enum TargetType
    {
        Player,
        Enemy
    }
    public TargetType targetType;

    public bool disableSlotPlay = true;

    public virtual void OnPlay(CardSlot slot){
        /*
            Spell sigil effect goes here
        */
    }

    public virtual bool CanBePlayed(CardSlot slot, bool player){
        /*
            Return if this SpellSigil can be played
        */
        return false;
    }

    public override SpellSigil GetSpellSigil(){
        /*
            Return this SpellSigil so there is a way to get a reference to this
        */
        return this;
    }

}
