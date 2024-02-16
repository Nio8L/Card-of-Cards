using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/Active Sigil/Death Clock")]
public class DeathClock : ActiveSigil
{
    public override void OnSummonEffects(CardInCombat card)
    {
        if(card.card.injuries.Count == 3) return;

        canBeUsed = true;
    }

    public override void ActiveEffect(CardInCombat card)
    {
        if(card.card.injuries.Count == 1){
            
            if(card.card.injuries[0] == Card.TypeOfDamage.Poison){
                
                card.card.injuries.Add(Card.TypeOfDamage.Scratch);
            
            }else if(card.card.injuries[0] == Card.TypeOfDamage.Scratch){
                
                card.card.injuries.Add(Card.TypeOfDamage.Bite);
            
            }else{
                
                card.card.injuries.Add(Card.TypeOfDamage.Poison);
            }
            card.card.injuries.RemoveAt(0);
        
        }else if(card.card.injuries.Count == 2){
            if(card.card.injuries.Contains(Card.TypeOfDamage.Poison) && card.card.injuries.Contains(Card.TypeOfDamage.Scratch)){
                card.card.injuries = new()
                {
                    Card.TypeOfDamage.Scratch,
                    Card.TypeOfDamage.Bite
                };
            }else if(card.card.injuries.Contains(Card.TypeOfDamage.Poison) && card.card.injuries.Contains(Card.TypeOfDamage.Bite)){
                card.card.injuries = new()
                {
                    Card.TypeOfDamage.Scratch,
                    Card.TypeOfDamage.Poison
                };
            }else{
                card.card.injuries = new()
                {
                    Card.TypeOfDamage.Bite,
                    Card.TypeOfDamage.Poison
                };
            }
        }

        SoundManager.soundManager.Play("DeathClock");

        card.deck.UpdateCardAppearance(card.transform, card.card);

        canBeUsed = false;
    }

    public override void PasiveEffect(CardInCombat card)
    {
        if(card.card.injuries.Count == 3) return;

        canBeUsed = true;
    }
}
