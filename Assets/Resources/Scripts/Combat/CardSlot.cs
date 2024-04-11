using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardSlot : MonoBehaviour
{
    public bool canBeUsed = true;
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
    [SerializeField]
    ParticleSystem smokeParticles;

    [HideInInspector]
    public int turnsIgnited;
    public int turnsDrenched;

    SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        EventManager.CombatEnd += OnCombatEnd;
    }
    private void OnEnable() {
        if(turnsIgnited > 0){
            ignitedParticles.Play();
        }
    }

    public void OnCombatEnd(){
        if (status == Status.Ignited){
            EventManager.NextTurn -= ApplyIgnitedEffects;
        }
        EventManager.CombatEnd -= OnCombatEnd;
    }

    public void ResetSlot(){
        status = Status.Normal;
        spriteRenderer.color = Color.white;
        ignitedParticles.Stop();
        smokeParticles.Stop();

        turnsIgnited = 0;
        turnsDrenched = 0;

        EventManager.NextTurn -= ApplyIgnitedEffects;
    }

    public void IgniteSlot(int duration){
        EventManager.NextTurn -= ApplyIgnitedEffects;

        status = Status.Ignited;
        turnsIgnited = duration;
        spriteRenderer.color = Color.red;

        ignitedParticles.Play();

        EventManager.NextTurn += ApplyIgnitedEffects;
    }

    public void ApplyIgnitedEffects(){
        turnsIgnited--;

        
        CardInCombat cardInCombat = CombatManager.combatManager.GetCardAtSlot(this);
        if (cardInCombat != null && !cardInCombat.card.fireImmune)
        {
             cardInCombat.card.health--;
             GameObject fireExplosion = Instantiate(CombatManager.combatManager.fireExplosionPrefab, transform);
             fireExplosion.transform.localScale = Vector3.one;
             AnimationUtilities.ChangeAlpha(fireExplosion.transform, 0.6f, 0.8f, 0f);
             SoundManager.soundManager.Play("FireExplosion");
        }

        if(turnsIgnited == 0){
            ResetSlot();
        }else if (turnsIgnited == 1){
            smokeParticles.Play();
        }
    }

    public void DrenchSlot(int duration){
        status = Status.Drenched;
        turnsDrenched = duration;
        spriteRenderer.color = Color.blue;
    }

}
