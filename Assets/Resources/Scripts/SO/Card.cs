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
    public bool fireImmune;
    [HideInInspector]
    public bool playerCard = true;
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


    /*public Card Copy() 
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
    }*/

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

        playerCard = cardToCopyFrom.playerCard;

        maxHealth = cardToCopyFrom.maxHealth;
        defaultAttack = cardToCopyFrom.defaultAttack;
        attack = cardToCopyFrom.attack;
        cost = cardToCopyFrom.cost;
        image = cardToCopyFrom.image;
        typeOfDamage = cardToCopyFrom.typeOfDamage;
        sigils = cardToCopyFrom.sigils;
        injuries = cardToCopyFrom.injuries;
    }

    public void CreateCard(TypeOfDamage causeOfDeath)
    {

        // Returns if death cards aren't used in the current fight
        if (CombatManager.combatManager != null){
            ScriptedEnemy scriptedEnemy = CombatManager.combatManager.enemy.GetScriptedEnemy();
            if (scriptedEnemy != null && scriptedEnemy.ignoreCardDeathRules){
                ResetCard();
                return;
            }
        }


        // Find bleed sigil
        string bleedName = "Bleed " + causeOfDeath.ToString();
        Sigil bleed = Resources.Load<Sigil>("Sigils/" + bleedName);
        negativeSigil = Instantiate(bleed);
        negativeSigil.name = bleed.name;

        // Check injuries
        foreach (TypeOfDamage type in injuries)
        {
            if (type == causeOfDeath)
            {
                Card lostSoulBase = Resources.Load<Card>("Cards/Lost Soul");
                Card lostSoulToCopy = Instantiate(lostSoulBase).ResetCard();
                lostSoulToCopy.name = lostSoulBase.name;

                EventManager.CardDeath?.Invoke(this);
                
                CopyFrom(lostSoulToCopy);

                return;
            }
        }

        injuries.Add(causeOfDeath);
        health = maxHealth;
        
        if (sigils.Count == 3){
            for (int i = sigils.Count-1; i >= 0; i--)
            {
                if (!sigils[i].negative)
                {
                    sigils.RemoveAt(i);
                    break;
                }
            }
        }

        if(sigils.Count != 3)sigils.Add(negativeSigil);
    }

    // Effects v
    public void ActivateOnHitEffects(CardInCombat card) 
    {
        foreach (Sigil sigil in sigils) sigil.OnHitEffect(card);
    }

    public void ActivateOnTurnStartEffects(CardInCombat card) 
    {
        foreach (Sigil sigil in sigils) sigil.OnTurnStartEffect(card);
    }
    public void ActivateOnTakeDamageEffects(CardInCombat card)
    {
        foreach (Sigil sigil in sigils) sigil.OnTakeDamageEffect(card);
    }
    public void ActivateOnSummonEffects(CardInCombat card) 
    {
        foreach(Sigil sigil in sigils)sigil.OnSummonEffect(card);
    }
    public void ActivateOnDeadEffects(CardInCombat card) 
    {
        foreach (Sigil sigil in sigils) sigil.OnDeadEffect(card);
    }  
    public void ActivateOnConsumeEffects(CardInCombat card, Card consumedCard){
        foreach (Sigil sigil in sigils) sigil.OnConsumeEffect(card, consumedCard);
    }
    public void ActivateOnFightStartEffects(CardInCombat card) 
    {
        foreach (Sigil sigil in sigils) sigil.OnFightStartEffect(card);
    }
    public void ActivateOnBattleEndEffects(CardInCombat card){
        foreach (Sigil sigil in sigils) sigil.OnBattleEndEffect(card);
    }
    public void ActivateOnDrawEffects(){
        foreach (Sigil sigil in sigils) sigil.OnDrawEffect(this);
    }
    public void ActivateOnDiscardEffects(){
        foreach (Sigil sigil in sigils) sigil.OnDiscardEffect(this);
    }
    public void ActivateOnNotDrawnEffects(){
        foreach (Sigil sigil in sigils) sigil.OnNotDrawn(this);
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
        // Reset the card's stats to their base values
        ResetAttack();
        ResetHP();

        // Remove its fire immunity
        fireImmune = false;

        // Reset its sigils so they don't use the base ones
        for (int i = 0; i < sigils.Count; i++)
        {
            string oldSigilName = sigils[i].name;
            sigils[i] = Instantiate(sigils[i]);
            sigils[i].name = oldSigilName;
        }

        return this;
    }
}
