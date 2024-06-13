using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Card")]
public class Card : ScriptableObject
{
    public int defaultHealth;
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
        Scratch,
        Heart
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

    public bool CanHeal(){
        for(int i = 0; i < sigils.Count; i++){
            if(sigils[i].negative){
                return true;
            }
        }

        if(injuries.Count > 0){
            return true;
        }

        return false;
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

        playerCard = cardToCopyFrom.playerCard;

        defaultHealth = cardToCopyFrom.defaultHealth;
        defaultAttack = cardToCopyFrom.defaultAttack;
        attack = cardToCopyFrom.attack;
        cost = cardToCopyFrom.cost;
        image = cardToCopyFrom.image;
        typeOfDamage = cardToCopyFrom.typeOfDamage;
        sigils = cardToCopyFrom.sigils;
        injuries = cardToCopyFrom.injuries;

        if(cardToCopyFrom.name == "Lost Soul"){
            defaultHealth = 0;
            health = 0;
        }
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
        health = defaultHealth;
        
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

        if(sigils.Count != 3) AddSigil(negativeSigil);
    }

    public void AddSigil(Sigil sigil){
        sigils.Add(sigil);
        sigil.OnAcquireEffect(this);
    }

    // Effects
    public void ActivateOnHitEffects(CardInCombat card) 
    {
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnHitEffect(card);
        }

    }

    public void ActivateOnTurnStartEffects(CardInCombat card) 
    {
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnTurnStartEffect(card);
        }

    }
    public void ActivateOnTakeDamageEffects(CardInCombat card)
    {
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnTakeDamageEffect(card);
        }

    }
    public void ActivateOnSummonEffects(CardInCombat card) 
    {
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnSummonEffect(card);
        }

    }
    public void ActivateOnDeadEffects(CardInCombat card) 
    {
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnDeadEffect(card);
        }

    }  
    public void ActivateOnConsumeEffects(CardInCombat card, Card consumedCard){
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnConsumeEffect(card, consumedCard);
        }

    }
    public void ActivateOnFightStartEffects(CardInCombat card) 
    {
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnFightStartEffect(card);
        }

    }
    public void ActivateOnBattleEndEffects(){
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnBattleEndEffect(this);
        }

    }
    public void ActivateOnDrawEffects(){
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnDrawEffect(this);
        }

    }
    public void ActivateOnDiscardEffects(){
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnDiscardEffect(this);
        }

    }
    public void ActivateOnNotDrawnEffects(){
        for (int i = 0; i < sigils.Count; i++)
        {
            Sigil sigil = sigils[i];
            sigil.OnNotDrawn(this);
        }

    }
    public void ResetHP() 
    {
        health = defaultHealth;
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
            sigils[i].OnAcquireEffect(this);
        }

        return this;
    }

    //Used to check if the card can target a slot directly
    //This depends on whether the card has spell sigils or no
    //Some spell sigils deny slot targeting and some allow it
    public bool CanTargetSlot(){
        for(int i = 0; i < sigils.Count; i++){
            if(sigils[i].GetSpellSigil() != null){
                if (sigils[i].GetSpellSigil().disableSlotPlay)
                {
                    return false;
                }
            }
        }

        return true;
    }

    public bool HasSpellSigils(){
        for(int i = 0; i < sigils.Count; i++){
            if(sigils[i].GetSpellSigil() != null){
                return true;
            }
        }

        return false;
    }
}
