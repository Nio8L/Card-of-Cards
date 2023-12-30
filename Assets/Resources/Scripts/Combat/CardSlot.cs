using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public int slot = 0;
    public bool playerSlot = true;
    public bool bench;
    public Status status = Status.Normal;
    public enum Status{
        Normal,
        Ignited,
        Drenched
    };

    [SerializeField]
    ParticleSystem ignitedParticles;

    [HideInInspector]
    public int turnsIgnited;
    public int turnsDrenched;

    SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();

        if(playerSlot && bench && slot == 0){
            IgniteSlot(3);
        }
    }

    private void OnDisable() {
        EventManager.NextTurn -= ApplyIgnitedEffects;
    }

    public void ResetSlot(){
        status = Status.Normal;
        spriteRenderer.color = Color.white;

        turnsIgnited = 0;
        turnsDrenched = 0;

        EventManager.NextTurn -= ApplyIgnitedEffects;
    }

    public void IgniteSlot(int duration){
        status = Status.Ignited;
        turnsIgnited = duration;
        spriteRenderer.color = Color.red;

        ignitedParticles.Play();

        EventManager.NextTurn += ApplyIgnitedEffects;
    }

    public void ApplyIgnitedEffects(){
        if(turnsIgnited == 0){
            ResetSlot();
            ignitedParticles.Stop();
            return;
        }
        
        CardInCombat cardInCombat = CombatManager.combatManager.GetCardAtSlot(this);
        if (cardInCombat != null) cardInCombat.card.health--;

        turnsIgnited--;
    }

    public void DrenchSlot(int duration){
        status = Status.Drenched;
        turnsDrenched = duration;
        spriteRenderer.color = Color.blue;
    }

}
