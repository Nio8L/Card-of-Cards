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

    public static Sigil negativeSigil;

    public TypeOfDamage typeOfDamage;

    public List<Sigil> sigils = new();

    public List<TypeOfDamage> injuries = new();

    public BattleData lastBattle;

    public Card Copy() 
    {
        return new Card()
        {
            health = health,
            attack = attack,
            cost = cost,
            image = image,
            typeOfDamage = typeOfDamage,
            sigils = sigils,
            injuries = injuries
        };
    }

    public void CreateCard(TypeOfDamage causeOfDeath)
    {
        foreach (TypeOfDamage type in injuries)
        {
            if (type == causeOfDeath)
            {
                //lost soul
                return;
            }
        }
        injuries.Add(causeOfDeath);

        for (int i = 0; i < sigils.Count; i++)
        {
            if (!sigils[i].negative)
            {
                sigils.RemoveAt(i);
            }
        }

        if(sigils.Count != 3)sigils.Add(negativeSigil);
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
