using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Card")]
public class Card : ScriptableObject
{
    public int maxHealth;
    [HideInInspector]
    public int health;
    [HideInInspector]
    public int attack;
    public int defaultAttack;
    public int cost;
    public Sprite image;
    [HideInInspector]
    public bool canRevive = true;
    [HideInInspector]
    public bool captain;
    public enum TypeOfDamage
    {
        Poison,
        Bite,
        Scratch
    };

    public static Sigil negativeSigil;

    public TypeOfDamage typeOfDamage;

    public List<Sigil> sigils = new List<Sigil>();

    public List<TypeOfDamage> injuries = new List<TypeOfDamage>();

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
        for(int i = sigils.Count - 1; i >= 0; i--){
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
        Sigil bleed = Resources.Load<Sigil>("Sigils/Weak Bleed");
        negativeSigil = Instantiate(bleed);
        negativeSigil.name = bleed.name;

        foreach (TypeOfDamage type in injuries)
        {
            if (type == causeOfDeath)
            {
                Card lostSoul = Resources.Load<Card>("Cards/LostSoul");
                CopyFrom(lostSoul);
                ResetCard();
                
                EventManager.CardDeath?.Invoke();

                return;
            }
        }

        injuries.Add(causeOfDeath);
        health = maxHealth;
        for (int i = 0; i < sigils.Count && sigils.Count == 3; i++)
        {
            if (!sigils[i].negative)
            {
                sigils.RemoveAt(i);
                break;
            }
        }

        if(sigils.Count != 3)sigils.Add(negativeSigil);
    }

    // Effects v
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

    public void ActivateOnSummonEffects(CardInCombat card) 
    {
        foreach(Sigil sigil in sigils)sigil.OnSummonEffects(card);
    }

    public void ActivateOnDeadEffects(CardInCombat card) 
    {
        foreach (Sigil sigil in sigils) sigil.OnDeadEffects(card);
    }  

    public void ActivateOnConsumeEffects(CardInCombat card, Card consumedCard){
        foreach (Sigil sigil in sigils) sigil.OnConsumeEffects(card, consumedCard);
    }
    public void ActivateOnBattleStartEffects(CardInCombat card) 
    {
        foreach (Sigil sigil in sigils) sigil.OnBattleStartEffects(card);
    }

    public void ActivaeOnEndOfTurnEffects(CardInCombat card){
        foreach (Sigil sigil in sigils) sigil.OnEndOfTurnEffects(card);
    }

    public void ActivateOnBattleEndEffects(CardInCombat card){
        foreach (Sigil sigil in sigils) sigil.OnBattleEndEffects(card);
    }

    public void ResetHP() 
    {
        health = maxHealth;
    }

    public void ResetAttack(){
        attack = defaultAttack;
    }

    public Card ResetCard()
    {
        ResetAttack();
        ResetHP();
        for (int i = 0; i < sigils.Count; i++)
        {
            string oldSigilName = sigils[i].name;
            sigils[i] = Instantiate(sigils[i]);
            sigils[i].name = oldSigilName;
            captain = false;
        }
        return this;
    }
}
