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

    [HideInInspector]
    public int turnsIgnited;
    public int turnsDrenched;

    SpriteRenderer spriteRenderer;

    private void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnDisable() {
        EventManager.NextTurn -= ApplyIgnitedEffects;
    }

    public void ResetSlot(){
        status = Status.Normal;
        spriteRenderer.color = Color.white;

        EventManager.NextTurn -= ApplyIgnitedEffects;
    }

    public void IgniteSlot(int duration){
        status = Status.Ignited;
        turnsIgnited = duration;
        spriteRenderer.color = Color.red;

        EventManager.NextTurn += ApplyIgnitedEffects;
    }

    public void ApplyIgnitedEffects(){
        if(playerSlot){
            if(bench){
                if(CombatManager.combatManager.playerBenchCards[slot] != null){
                    CombatManager.combatManager.playerBenchCards[slot].card.health -= 1;
                }
            }else{
                if(CombatManager.combatManager.playerCombatCards[slot] != null){
                    CombatManager.combatManager.playerCombatCards[slot].card.health -= 1;
                }
            }
        }else{
            if(bench){
                if(CombatManager.combatManager.enemyBenchCards[slot] != null){
                    CombatManager.combatManager.enemyBenchCards[slot].card.health -= 1;
                }
            }else{
                if(CombatManager.combatManager.enemyCombatCards[slot] != null){
                    CombatManager.combatManager.enemyCombatCards[slot].card.health -= 1;
                }
            }
        }

        turnsIgnited--;
        if(turnsIgnited == 0){
            ResetSlot();
        }
    }

    public void DrenchSlot(int duration){
        status = Status.Drenched;
        turnsDrenched = duration;
        spriteRenderer.color = Color.blue;
    }

}
