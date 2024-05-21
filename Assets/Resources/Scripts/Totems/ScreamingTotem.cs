using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName="Totem/ScreamingTotem")]
public class ScreamingTotem : Totem
{
    public int healBy;

    public GameObject particles;
    public override void Setup()
    {
        EventManager.NextTurn += Passive;
    }

     public override void Unsubscribe()
    {
        EventManager.NextTurn -= Passive;
    }

    public override void Passive()
    {
        int combatCards = 0;
        int benchedCards = 0;

        for (int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
            if (CombatManager.combatManager.playerCombatCards[i] != null) combatCards++;
            if (CombatManager.combatManager.playerBenchCards[i] != null) benchedCards++;
        }

        if (combatCards > benchedCards) CombatManager.combatManager.playerHealth += healBy;
        if (CombatManager.combatManager.playerHealth > 20) CombatManager.combatManager.playerHealth = 20;
        CombatManager.combatManager.combatUI.UpdateHPText();
    }

    public override void Active()
    {
        int cards = 0;

        for (int i = 0; i < CombatManager.combatManager.playerCombatSlots.Length; i++){
            if (CombatManager.combatManager.playerCombatCards[i] != null) cards++;
            if (CombatManager.combatManager.playerBenchCards[i] != null) cards++;
        }

        CombatManager.combatManager.playerHealth += healBy * cards;
        if (CombatManager.combatManager.playerHealth > 20) CombatManager.combatManager.playerHealth = 20;
        CombatManager.combatManager.combatUI.UpdateHPText();

        Instantiate(particles, GameObject.Find("Canvas").transform);
    }

}
