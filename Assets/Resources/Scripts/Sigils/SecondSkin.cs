using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Sigil/SecondSkin")]
public class SecondSkin : Sigil
{
    public int direction = 1;
    public Card Skin;
    int slot;
    CombatManager cm;
    bool hasASkin = true;

    public override void OnDeadEffects(CardInCombat card) 
    {
        hasASkin = true;
    }

    public override void OnTakeDamageEffect(CardInCombat card)
    {
        if (!hasASkin) return;

        cm = card.deck.combatManager;
        card.card.health = card.card.lastBattle.thisCardOldHp;
        slot = card.slot;

        if (slot == 0 && direction == -1)
        {
            direction = 1;
        }
        else if (slot == 2 && direction == 1)
        {
            direction = -1;
        }


        if (card.playerCard && cm.playerCards[slot + direction] != null)
        {
            direction *= -1;
        }
        else if (!card.playerCard && cm.enemyCards[slot + direction] != null)
        {
            direction *= -1;
        }
        Move(card);
    }


    void Move(CardInCombat card)
    {
        if (card.slot + direction < 0 || card.slot + direction >= cm.playerCombatSlots.Length) return;

        if (!card.benched)
        {
            if (card.playerCard && cm.playerCards[card.slot + direction] == null)
            {
                SpawnSkin(card);
                cm.playerCards[card.slot + direction] = card;
                card.slot += direction;
                card.transform.position = cm.playerCombatSlots[card.slot].transform.position;

                card.benched = true;
                card.PutOnOrOffTheBench();
                card.moved = true;

                hasASkin = false;
            }
            else if (!card.playerCard && cm.enemyCards[card.slot + direction] == null)
            {
                cm.EnemyPlayCard(card.card, slot);
                cm.enemyCards[card.slot + direction] = card;
                card.slot += direction;
                card.transform.position = cm.enemyCombatSlots[card.slot].transform.position;

                card.benched = true;
                card.PutOnOrOffTheBenchEnemyCards();
                card.moved = true;

                hasASkin = false;
            }
        }

    }

    void SpawnSkin(CardInCombat card)
    {
        GameObject cardToCreate = Instantiate(card.deck.cardInCombatPrefab, cm.playerCombatSlots[slot].transform.position, Quaternion.identity);
        cardToCreate.transform.SetParent(card.deck.CardsInCombatParent);
        cardToCreate.transform.localScale = Vector3.one * 0.75f;

        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = Skin;
        cardInCombat.deck = card.deck;
        cardInCombat.slot = slot;
        cardInCombat.benched = false;

        card.deck.combatManager.playerCards[slot] = cardInCombat;
    }
}
