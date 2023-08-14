using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Card")]
public class Card : ScriptableObject
{
    public int health;
    public int attack;
    public int cost;
    public Sprite image;
    public enum TypeOfDamage
    {
        Poison,
        Bite,
        Scratch
    };

    public TypeOfDamage typeOfDamage;

    public List<Sigil> sigils = new();

    public List<TypeOfDamage> injuries = new();

    public BattleData lastBattle;

    public void CreateCard(TypeOfDamage causeOfDeath)
    {
        foreach (TypeOfDamage type in injuries)
        {
            if (type == causeOfDeath)
            {
                // Lost soul
                return;
            }
        }
        injuries.Add(causeOfDeath);
        //new card
    }
    public void ActivateOnHitEffects(CardInCombat card) 
    {
        foreach (Sigil sigil in sigils) sigil.ApplyOnHitEffect(card);
    }

    public void ActivatePasiveEffects(CardInCombat card) 
    {
        foreach (Sigil sigil in sigils) sigil.PasiveEffect(card);
    }

    public void ActivateOnTakeDamageEffects(CardInCombat card)
    {
        foreach (Sigil sigil in sigils) sigil.OnTakeDamageEffect(card);
    }
}
