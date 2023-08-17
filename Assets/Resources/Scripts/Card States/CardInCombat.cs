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
    public bool moved;

    Vector3 startPosition;
    Vector3 endPosition;
    float curentAnimationTime = -0.5f;
    float maxAnimationTime;

    bool returnMovement;

    GameObject bloodSplat;
    void Start()
    {
        deck.UpdateCardAppearance(transform, card);
        bloodSplat = Resources.Load<GameObject>("Prefabs/BloodSplatPart");
    }

    private void Update()
    {

        if (returnMovement && curentAnimationTime > -0.5f)
        {
            curentAnimationTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(endPosition, startPosition, Mathf.Abs(curentAnimationTime)*2);

            if (curentAnimationTime < 0f)
            {
                deck.UpdateCardAppearance(transform, card);
            }
        }
        else if (!returnMovement && curentAnimationTime > 0f)
        {
            curentAnimationTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(endPosition, startPosition, curentAnimationTime * (1/maxAnimationTime));
        }
        else if (card.health <= 0)
        {
            card.CreateCard(lastTypeOfDamage);

            if (playerCard)
            {
                deck.combatManager.playerCards[slot] = null;
                deck.discardPile.Add(card);
            }
            else
            {
                deck.combatManager.enemyCards[slot] = null;
                deck.combatManager.enemyDeck.discardPile.Add(card);
            }

            Instantiate(bloodSplat, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }

    public void PutOnOrOffTheBenchEnemyCards()
    {
        if (benched)
        {
            MoveAnimationStarter(0.5f, deck.combatManager.enemyBenchSlots[slot].transform.position);
            return;
        }
        MoveAnimationStarter(0.5f, deck.combatManager.enemyCombatSlots[slot].transform.position);
    }

    public void BenchOrUnbench() 
    {
        if (!playerCard || deck.combatManager.gamePhase == 1) return;
        benched = !benched;
        PutOnOrOffTheBench();
    }

    public void PutOnOrOffTheBench() 
    {
        if (benched) 
        {
            MoveAnimationStarter(0.5f, deck.combatManager.playerBenchSlots[slot].transform.position);
            return;
        }
        MoveAnimationStarter(0.5f, deck.combatManager.playerCombatSlots[slot].transform.position);
    }

    public void PerformShortAttackAnimation()
    {
        maxAnimationTime = 0.5f;
        curentAnimationTime = maxAnimationTime + 0.25f * slot;
        endPosition = new Vector3(transform.position.x, 1f, 0f);
        startPosition = transform.position;
        returnMovement = true;
    }

    public void MoveAnimationStarter(float time, Vector3 end)
    {
        maxAnimationTime = time;
        curentAnimationTime = time;
        endPosition = end;
        startPosition = transform.position;
        returnMovement = false;
    }
}
