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
    public virtual void OnTurnStartEffect(CardInCombat card) 
    {
        /*
            Activates at the start of the owners turn
        */
    }
    public virtual void OnHitEffect(CardInCombat card)
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
    public virtual void OnSummonEffect(CardInCombat card) 
    {
        /*
            Activates when the card with this sigils is played
        */
    }
    public virtual void OnDeadEffect(CardInCombat card) 
    {
        /*
            This activates when the card with this sigils dies (or would die)
        */
    }
    public virtual void OnConsumeEffect(CardInCombat card, Card consumedCard){
        /*
            Consume effects use this after consuming a card
        */
    }
    public virtual void OnFightStartEffect(CardInCombat card)
    {
        /*
            Activates right before the start of a fight
        */
    }
    public virtual void OnBattleEndEffect(CardInCombat card){
        /*
            Triggers at the end of the combat
        */
    }
    public virtual void OnDrawEffect(Card card){
        /*
            Triggers when drawn (This is used before the card is even played)
        */
    }
    public virtual void OnDiscardEffect(Card card){
        /*
            Triggers when discarded (This is used before the card is even played)
        */
    }
    public virtual void OnNotDrawn(Card card){
        /*
           Triggers whenever this card is in the draw pile and it is not drawn.
        */
    }
    public virtual ActiveSigil GetActiveSigil(){
        /*
            Ignore this unless you are working with active sigils 
        */
        return null;
    }

    public virtual SpellSigil GetSpellSigil(){
        /*
            Ignore this unless you are working with spell sigils 
        */
        return null;
    }

}
