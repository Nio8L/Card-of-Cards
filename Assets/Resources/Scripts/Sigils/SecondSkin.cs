using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/SecondSkin")]
public class SecondSkin : Sigil
{
    bool hasASkin = true;
    public Card cardToSpawn;
    Card secondSkin;

    public override void OnDeadEffects(CardInCombat card) 
    {
        hasASkin = true;
    }

    public override void OnBattleStartEffects(CardInCombat card)
    {
        if (!hasASkin) return;
        if (    card.playerCard
            && (card.deck.combatManager.enemyCombatCards[card.slot] == null
            ||  card.deck.combatManager.enemyCombatCards[card.slot].card.attack <= 0)) return;
        else if (!card.playerCard
            && (card.deck.combatManager.playerCombatCards[card.slot] == null
            || card.deck.combatManager.playerCombatCards[card.slot].card.attack <= 0)) return;

        secondSkin = Instantiate(cardToSpawn).ResetCard();
        secondSkin.name = cardToSpawn.name;

        hasASkin = false;

        if (card.playerCard && card.deck.combatManager.playerBenchCards[card.slot] == null)
        {
            card.BenchOrUnbench();
            SpawnSkin(card);
        }
        else if (!card.playerCard && card.deck.combatManager.enemyBenchCards[card.slot] == null)
        {
            card.BenchOrUnbenchEnemy();
            card.deck.combatManager.enemy.PlayCard(secondSkin, card.slot, false);
        }

    }

    void SpawnSkin(CardInCombat card) 
    {
        card.deck.PlaySigilAnimation(card.transform, card.card, this);
        GameObject cardToCreate = Instantiate(card.deck.cardInCombatPrefab, card.deck.combatManager.playerCombatSlots[card.slot].transform.position, Quaternion.identity);
        cardToCreate.transform.SetParent(card.deck.CardsInCombatParent);
        cardToCreate.transform.localScale = Vector3.one * 0.75f;

        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = secondSkin;
        cardInCombat.deck = card.deck;
        cardInCombat.slot = card.slot;
        cardInCombat.benched = false;

        card.deck.combatManager.playerCombatCards[card.slot] = cardInCombat;

        cardInCombat.card.health -= card.card.maxHealth - card.card.health;
        card.card.ResetHP();
    }
}
