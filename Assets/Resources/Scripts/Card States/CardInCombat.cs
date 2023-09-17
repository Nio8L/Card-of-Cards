using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardInCombat : MonoBehaviour
{
    public bool benched = true;

    public Card card;
    public Deck deck;
    public Card.TypeOfDamage lastTypeOfDamage;

    [HideInInspector]
    public bool passivesTurnedOnThisTurn = false;
    [HideInInspector]
    public bool PerformeAtackAnimation = true;

    public bool canBeBenched = true;
    public bool playerCard = true;
    public int slot = 0;
    public bool moved;
    public int direction = 1;

    Vector3 startPosition;
    Vector3 endPosition;
    float curentAnimationTime = -0.5f;
    float maxAnimationTime;

    float delayAfterRightClick = 0f;
    float maxDelay = 0.2f;

    bool updatedAfterReturnAnimation;

    bool rightClicked;
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
        if (Input.GetMouseButton(1))
        {
            delayAfterRightClick = maxDelay;
            rightClicked = true;
        }
        else if(delayAfterRightClick > 0)
        {
            delayAfterRightClick -= Time.deltaTime;
        }
        else if (rightClicked)
        {
            rightClicked = false;
        }


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

    public void BenchOrUnbench() 
    {
        if (!canBeBenched || !playerCard || deck.combatManager.gamePhase == 2||rightClicked|| Input.GetMouseButtonDown(1)) return;
        benched = !benched;

        if (benched)
        {
            deck.combatManager.playerCombatCards[slot] = deck.combatManager.playerBenchCards[slot];
            deck.combatManager.playerBenchCards[slot] = this;
            if (deck.combatManager.playerCombatCards[slot] != null)
            {
                deck.combatManager.playerCombatCards[slot].benched = !benched;
                deck.combatManager.playerCombatCards[slot].PutOnOrOffTheBench();
            }
            SoundManager.soundManager.Play("CardSlide");
        }
        else
        {
            deck.combatManager.playerBenchCards[slot] = deck.combatManager.playerCombatCards[slot];
            deck.combatManager.playerCombatCards[slot] = this;
            if (deck.combatManager.playerBenchCards[slot] != null)
            {
                deck.combatManager.playerBenchCards[slot].benched = !benched;
                deck.combatManager.playerBenchCards[slot].PutOnOrOffTheBench();
            }
            SoundManager.soundManager.Play("CardSlide");
        }

        PutOnOrOffTheBench();

        
    }
    public void BenchOrUnbenchEnemy()
    {
        SoundManager.soundManager.Play("CardSlide");

        if (!canBeBenched || playerCard) return;
        benched = !benched;

        if (benched)
        {
            deck.combatManager.enemyCombatCards[slot] = deck.combatManager.enemyBenchCards[slot];
            deck.combatManager.enemyBenchCards[slot] = this;
            if (deck.combatManager.enemyCombatCards[slot] != null)
            {
                deck.combatManager.enemyCombatCards[slot].benched = !benched;
                deck.combatManager.enemyCombatCards[slot].PutOnOrOffTheBenchEnemyCards();
            }
        }
        else
        {
            deck.combatManager.enemyBenchCards[slot] = deck.combatManager.enemyCombatCards[slot];
            deck.combatManager.enemyCombatCards[slot] = this;
            if (deck.combatManager.enemyBenchCards[slot] != null)
            {
                deck.combatManager.enemyBenchCards[slot].benched = !benched;
                deck.combatManager.enemyBenchCards[slot].PutOnOrOffTheBenchEnemyCards();
            }
        }

        PutOnOrOffTheBenchEnemyCards();
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
    public void PutOnOrOffTheBenchEnemyCards()
    {
        if (benched)
        {
            MoveAnimationStarter(0.5f, deck.combatManager.enemyBenchSlots[slot].transform.position);
            return;
        }
        MoveAnimationStarter(0.5f, deck.combatManager.enemyCombatSlots[slot].transform.position);
    }
    public void PerformShortAttackAnimation()
    {
        if (card.attack == 0) return;
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
    public void OnDeath()
    {
        if (GetComponent<DestroyTimer>().enabled == false)
        {
            card.ActivateOnDeadEffects(this);

            if (card.health > 0) return;
            
            card.CreateCard(lastTypeOfDamage);

            if (playerCard)
            {
                RemoveCardFromCardCollections();
                if (card.canRevive) deck.discardPile.Add(card);
            }
            else
            {
                RemoveCardFromCardCollections();
                if (!deck.combatManager.enemy.huntAI)
                {
                    if (card.canRevive) deck.combatManager.enemyDeck.discardPile.Add(card);
                }
                else if (deck.combatManager.battleReward.Count < 3)
                {
                    if (card.canRevive) deck.combatManager.battleReward.Add(card);
                }

            }

            Sprite markSprite;
            if (lastTypeOfDamage == Card.TypeOfDamage.Scratch) markSprite = deck.deathMarkScratch;
            else if (lastTypeOfDamage == Card.TypeOfDamage.Bite) markSprite = deck.deathMarkBite;
            else markSprite = deck.deathMarkPoison;

            Instantiate(bloodSplat, transform.position, Quaternion.identity);
            GameObject deathMarkObject = Instantiate(deathMark, transform.position, Quaternion.identity);
            deathMarkObject.GetComponent<SpriteRenderer>().sprite = markSprite;
            GetComponent<DestroyTimer>().enabled = true;
        }
    }
    private void OnDestroy() {
        SoundManager.soundManager.Play("CardDeath");
    }
    public void ShowSigilStar(Sigil sigil)
    {
        int alpha = 0;
        if (sigil.canUseAbility) alpha = 1;
        int index = 0;
        if (card.sigils.Count == 2) index++;

        if (card.sigils[0] == sigil)
        {
            transform.GetChild(13 + index).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
        else if (card.sigils[1] == sigil)
        {
            transform.GetChild(14 + index).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
        else
        {
            transform.GetChild(15).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
    }
    public void SetActiveSigilStar(Sigil sigil)
    {
        Sprite spriteToUse;
        if (sigil.canUseAbility) spriteToUse = deck.selectedActiveStar;
        else                     spriteToUse = deck.activeStar;
        int index = 0;
        if (card.sigils.Count == 2) index++;

        if (card.sigils[0] == sigil)
        {
            transform.GetChild(13 + index).GetComponent<Image>().sprite = spriteToUse;
        }
        else if (card.sigils[1] == sigil)
        {
            transform.GetChild(14 + index).GetComponent<Image>().sprite = spriteToUse;
        }
        else
        {
            transform.GetChild(15).GetComponent<Image>().sprite = spriteToUse;
        }
    }
    public void RemoveCardFromCardCollections() 
    {
        if (benched)
        {
            if(playerCard && deck.combatManager.playerBenchCards[slot] == this) deck.combatManager.playerBenchCards[slot] = null;
            else if (deck.combatManager.enemyBenchCards[slot] == this) deck.combatManager.enemyBenchCards[slot] = null;
        }
        else
        {
            if (playerCard && deck.combatManager.playerCombatCards[slot] == this) deck.combatManager.playerCombatCards[slot] = null;
            else if (deck.combatManager.enemyCombatCards[slot] == this) deck.combatManager.enemyCombatCards[slot] = null;
        }
    }

    public void ForceKill()
    {
        
        if (GetComponent<DestroyTimer>().enabled == false)
        {
            RemoveCardFromCardCollections();
            card.CreateCard(lastTypeOfDamage);

            if (playerCard)
            {
                if (card.canRevive) deck.discardPile.Add(card);
            }
            else
            {
                if (!deck.combatManager.enemy.huntAI)
                {
                    if (card.canRevive) deck.combatManager.enemyDeck.discardPile.Add(card);
                }
                else if (deck.combatManager.battleReward.Count < 3)
                {
                    if (card.canRevive) deck.combatManager.battleReward.Add(card);
                }

            }

            Sprite markSprite;
            if (lastTypeOfDamage == Card.TypeOfDamage.Scratch) markSprite = deck.deathMarkScratch;
            else if (lastTypeOfDamage == Card.TypeOfDamage.Bite) markSprite = deck.deathMarkBite;
            else markSprite = deck.deathMarkPoison;

            Instantiate(bloodSplat, transform.position, Quaternion.identity);
            GameObject deathMarkObject = Instantiate(deathMark, transform.position, Quaternion.identity);
            deathMarkObject.GetComponent<SpriteRenderer>().sprite = markSprite;
            GetComponent<DestroyTimer>().enabled = true;
        }
    }
}
