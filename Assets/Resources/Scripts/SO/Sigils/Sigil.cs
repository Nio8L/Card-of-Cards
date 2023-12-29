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
    public virtual void PasiveEffect(CardInCombat card) 
    {
        /*
            Activates at the start of the owners turn
        */
    }

    public virtual void ApplyOnHitEffect(CardInCombat card)
    {
        /*
            Activates when the card with this sigils attacks 
        */
    }

    public virtual void OnTakeDamageEffect(CardInCombat card)
    {
        /*
            Activates when the card with this sigils is attacked during combat (used before on hit effects)
        */
    }

    public virtual void OnSummonEffects(CardInCombat card) 
    {
        /*
            Activates when the card with this sigils is played
        */
    }

    public virtual void OnDeadEffects(CardInCombat card) 
    {
        /*
            This activates when the card with this sigils dies (or would die)
        */
    }

    public virtual void OnConsumeEffects(CardInCombat card, Card consumedCard){
        /*
            Consume effects use this after consuming a card
        */
    }

    public virtual void OnBattleStartEffects(CardInCombat card)
    {
        /*
            Activates right before the start of combat
        */
    }

    public virtual void OnEndOfTurnEffects(CardInCombat card){
        //effect should be here
    }

    public virtual void OnBattleEndEffects(CardInCombat card){
        /*
            Triggers after the end of all combats
        */
    }

    public virtual ActiveSigil GetActiveSigil(){
        /*
            Ignore this unless you are working with active sigils 
        */
        return null;
    }
}
