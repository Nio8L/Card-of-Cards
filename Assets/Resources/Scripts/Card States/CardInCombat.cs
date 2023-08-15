using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardInCombat : MonoBehaviour
{
    public bool benched = true;

    public Card card;
    public Deck deck;
    public Card.TypeOfDamage lastTypeOfDamage;

    [HideInInspector]
    public bool passivesTurnedOnThisTurn = false;

    public bool playerCard = true;
    public int slot = 0;

    Vector3 startPosition;
    Vector3 endPosition;
    float curentAnimationTime = -0.5f;
    float maxAnimationTime = 0.5f;
    void Start()
    {
        deck.UpdateCardAppearance(transform, card);
    }

    private void Update()
    {

        if (curentAnimationTime > -0.5f)
        {
            curentAnimationTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(endPosition, startPosition, Mathf.Abs(curentAnimationTime)*2);

            if (curentAnimationTime < 0f)
            {
                deck.UpdateCardAppearance(transform, card);
            }
        }
        else if (card.health <= 0)
        {
            card.CreateCard(lastTypeOfDamage);

            if (playerCard)
            {
                deck.combatManager.playerCards[slot] = null;
                deck.discardPile.Add(card);
            }
            else deck.combatManager.enemyCards[slot] = null;

            Destroy(gameObject);
        }

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }

    public void PutOnOrOffTheBenchEnemyCards()
    {
        if (benched)
        {
            transform.position = deck.combatManager.enemyBenchSlots[slot].transform.position;
            return;
        }
        transform.position = deck.combatManager.enemyCombatSlots[slot].transform.position;
    }

    public void BenchOrUnbench() 
    {
        if (!playerCard || deck.combatManager.gamePhase == 1) return;
        benched = !benched;
        PutOnOrOffTheBench();
    }

    void PutOnOrOffTheBench() 
    {
        if (benched) 
        {
            transform.position = deck.combatManager.playerBenchSlots[slot].transform.position;
            return;
        }
        transform.position = deck.combatManager.playerCombatSlots[slot].transform.position;
    }

    public void PerformShortAttackAnimation()
    {
        curentAnimationTime = maxAnimationTime + 0.25f * slot;
        endPosition = new Vector3(transform.position.x, 1f, 0f);
        startPosition = transform.position;
    }
}
