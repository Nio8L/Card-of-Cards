using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Card")]
public class Card : ScriptableObject
{
    public int maxHealth;
    [HideInInspector]
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

    public void AcceptLostSoul(){
        RemoveNegativeSigils();
        RemoveInjuries();
    }

    public void RemoveNegativeSigils(){
        for(int i = 0; i < sigils.Count; i++){
            if(sigils[i].negative){
                sigils.Remove(sigils[i]);
            }
        }
    }

    public void RemoveInjuries(){
        injuries.Clear();
    }

    public void CopyFrom(Card cardToCopyFrom)
    {
        name = cardToCopyFrom.name;

        maxHealth = cardToCopyFrom.maxHealth;
        attack = cardToCopyFrom.attack;
        cost = cardToCopyFrom.cost;
        image = cardToCopyFrom.image;
        typeOfDamage = cardToCopyFrom.typeOfDamage;
        sigils = cardToCopyFrom.sigils;
        injuries = cardToCopyFrom.injuries;
    }

    public void CreateCard(TypeOfDamage causeOfDeath)
    {
        Sigil bleed = Resources.Load<Sigil>("Sigils/WeakBleed");
        negativeSigil = Instantiate(bleed);
        negativeSigil.name = bleed.name;
        foreach (TypeOfDamage type in injuries)
        {
            if (type == causeOfDeath)
            {
                Card lostSoul = Resources.Load<Card>("Cards/LostSoul");
                CopyFrom(lostSoul);
                ResetCard();
                return;
            }
        }
        injuries.Add(causeOfDeath);
        health = maxHealth;
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
        foreach (Sigil sigil in sigils) 
        {
            sigil.PasiveEffect(card);
        } 
    }

    public void ActivateOnTakeDamageEffects(CardInCombat card)
    {
        foreach (Sigil sigil in sigils) sigil.OnTakeDamageEffect(card);
    }

    public Card ResetCard()
    {
        health = maxHealth;
        for (int i = 0; i < sigils.Count; i++)
        {
            string oldSigilName = sigils[i].name;
            sigils[i] = Instantiate(sigils[i]);
            sigils[i].name = oldSigilName;
        }
        return this;
    }
}
