using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/SecondSkin")]
public class SecondSkin : Sigil
{
    //bool hasASkin = true;
    //public Card Skin;

    //public override void OnDeadEffects(CardInCombat card) 
    //{
    //    hasASkin = true;
    //}

    //public override void OnTakeDamageEffect(CardInCombat card)
    //{
    //    if (!hasASkin) return;

    //    int startingSlot = card.slot;
    //    CardInCombat[] cardColectionToLookAt;
    //    CardInCombat[] enemyCardColection;
    //    GameObject[] slotColection;
    //    int direction = card.direction;

    //    if (card.playerCard)
    //    {
    //        slotColection = card.deck.combatManager.playerCombatSlots;
    //        cardColectionToLookAt = card.deck.combatManager.playerCards;
    //        enemyCardColection = card.deck.combatManager.enemyCards;
    //    }
    //    else 
    //    {
    //        cardColectionToLookAt = card.deck.combatManager.enemyCards;
    //        slotColection = card.deck.combatManager.enemyCombatSlots;
    //        enemyCardColection = card.deck.combatManager.playerCards;
    //    }

    //    if ((startingSlot + direction < cardColectionToLookAt.Length && startingSlot + direction >= 0) && cardColectionToLookAt[startingSlot + 1] == null) 
    //    {
    //        cardColectionToLookAt[startingSlot + direction] = card;
    //        card.slot += direction;
    //        card.transform.position = slotColection[card.slot].transform.position;

    //        card.benched = true;

    //        if (card.playerCard) card.PutOnOrOffTheBench();
    //        else card.PutOnOrOffTheBenchEnemyCards();

    //        card.moved = true;

    //        SpawnSkin(slotColection, startingSlot, card, enemyCardColection[startingSlot], cardColectionToLookAt);
    //        card.card.health = card.card.lastBattle.thisCardOldHp;
    //        enemyCardColection[startingSlot].card.health += card.card.attack;

    //        card.deck.combatManager.Skirmish(cardColectionToLookAt[startingSlot], enemyCardColection[startingSlot]);

    //        card.PerformeAtackAnimation = false;
    //        hasASkin = false;
    //    }
    //    else if((startingSlot - direction < cardColectionToLookAt.Length && startingSlot - direction >= 0) && cardColectionToLookAt[startingSlot - 1] == null)
    //    {

    //        cardColectionToLookAt[card.slot - direction] = card;
    //        card.slot -= direction;
    //        card.transform.position = slotColection[card.slot].transform.position;

    //        card.benched = true;

    //        if (card.playerCard) card.PutOnOrOffTheBench();
    //        else card.PutOnOrOffTheBenchEnemyCards();

    //        card.moved = true;

    //        SpawnSkin(slotColection, startingSlot, card, enemyCardColection[startingSlot], cardColectionToLookAt);
    //        card.card.health = card.card.lastBattle.thisCardOldHp;
    //        enemyCardColection[startingSlot].card.health += card.card.attack;

    //        card.deck.combatManager.Skirmish(cardColectionToLookAt[startingSlot], enemyCardColection[startingSlot]);

    //        card.PerformeAtackAnimation = false;
    //        hasASkin = false;
    //    }
    //}

    //void SpawnSkin(GameObject[] slotColection, int slot, CardInCombat card, CardInCombat enemyCard, CardInCombat[] cardColection) 
    //{
    //    card.deck.PlaySigilAnimation(card.transform, card.card, this);
    //    GameObject cardToCreate = Instantiate(card.deck.cardInCombatPrefab, slotColection[slot].transform.position, Quaternion.identity);
    //    cardToCreate.transform.SetParent(card.deck.CardsInCombatParent);
    //    cardToCreate.transform.localScale = Vector3.one * 0.75f;

    //    CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
    //    cardInCombat.card = Instantiate(Skin);
    //    cardInCombat.card.ResetCard();
    //    cardInCombat.deck = card.deck;
    //    cardInCombat.slot = slot;
    //    cardInCombat.benched = false;

    //    cardColection[slot] = cardInCombat;

    //    cardInCombat.playerCard = card.playerCard;
    //}
}
