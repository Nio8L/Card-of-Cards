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

    public bool canBeBenched = true;
    public bool playerCard = true;
    public int slot = 0;
    public bool moved;
    public int direction = 1;

    Vector3 startPosition;
    Vector3 endPosition;
    float curentAnimationTime = -0.5f;
    float maxAnimationTime;

    bool updatedAfterReturnAnimation;

    bool returnMovement;

    GameObject bloodSplat;
    GameObject deathMark;
    void Start()
    {
        card.ActivateOnSummonEffects(this);
        deck.UpdateCardAppearance(transform, card);
        bloodSplat = Resources.Load<GameObject>("Prefabs/BloodSplatPart");
        deathMark = Resources.Load<GameObject>("Prefabs/DeathMark");
    }

    private void Update()
    {
        if (returnMovement && curentAnimationTime > -0.5f)
        {
            curentAnimationTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(endPosition, startPosition, Mathf.Abs(curentAnimationTime) * 2);

            if (curentAnimationTime < 0f && !updatedAfterReturnAnimation)
            {
                deck.UpdateCardAppearance(transform, card);
                updatedAfterReturnAnimation = true;
            }
        }
        else if (returnMovement && curentAnimationTime <= -0.5 && updatedAfterReturnAnimation) 
        {
            updatedAfterReturnAnimation = false;
        }
        else if (!returnMovement && curentAnimationTime > 0f)
        {
            curentAnimationTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(endPosition, startPosition, curentAnimationTime * (1 / maxAnimationTime));
        }
        else if (card.health <= 0)
        {
            OnDeath();
        }

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }

    public void PutOnOrOffTheBenchEnemyCards()
    {
        if(canBeBenched) transform.position = deck.combatManager.enemyCombatSlots[slot].transform.position;
        if (benched)
        {
            MoveAnimationStarter(0.5f, deck.combatManager.enemyBenchSlots[slot].transform.position);
            return;
        }
        MoveAnimationStarter(0.5f, deck.combatManager.enemyCombatSlots[slot].transform.position);
    }

    public void BenchOrUnbench() 
    {
        SoundManager.soundManager.Play("CardSlide");

        if (!canBeBenched || !playerCard || deck.combatManager.gamePhase == 1) return;
        benched = !benched;
        PutOnOrOffTheBench();
    }

    public void PutOnOrOffTheBench() 
    {
        Debug.Log(card.name + " " + deck + " " + deck.combatManager.playerBenchSlots[slot]);
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

    void OnDeath()
    {
        if (GetComponent<DestroyTimer>().enabled == false)
        {
            card.ActivateOnDeadEffects(this);

            // Return if the cards has gained health via dead effects (ex: Fearless sigil)
            if (card.health > 0) return;

            // Effects v 
            Sprite markSprite;
            if (lastTypeOfDamage == Card.TypeOfDamage.Scratch) markSprite = deck.deathMarkScratch;
            else if (lastTypeOfDamage == Card.TypeOfDamage.Bite) markSprite = deck.deathMarkBite;
            else markSprite = deck.deathMarkPoison;

            Instantiate(bloodSplat, transform.position, Quaternion.identity);
            GameObject deathMarkObject = Instantiate(deathMark, transform.position, Quaternion.identity);
            deathMarkObject.GetComponent<SpriteRenderer>().sprite = markSprite;
            GetComponent<DestroyTimer>().enabled = true;

            // Return if the card can't be turned into lost soul (ex: Soulless sigil)
            if (!card.canRevive) return;

            card.CreateCard(lastTypeOfDamage);

            // Add changed card to deck
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

           
        }
    }

    private void OnDestroy() {
        SoundManager.soundManager.Play("CardDeath");
    }
}
