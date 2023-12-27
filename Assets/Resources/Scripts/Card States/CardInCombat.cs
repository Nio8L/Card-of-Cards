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
    public bool PerformAtackAnimation = true;

    public bool canBeBenched = true;
    public bool playerCard = true;
    public int slot = 0;
    public int direction = 1;

    Vector3 startPosition;
    Vector3 endPosition;
    float currentAnimationTime = -0.5f;
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


        if (returnMovement && currentAnimationTime > -0.5f)
        {
            currentAnimationTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(endPosition, startPosition, Mathf.Abs(currentAnimationTime) * 2);

            if (currentAnimationTime < 0f && !updatedAfterReturnAnimation)
            {
                deck.UpdateCardAppearance(transform, card);
                updatedAfterReturnAnimation = true;
            }
        }
        else if (returnMovement && currentAnimationTime <= -0.5 && updatedAfterReturnAnimation) 
        {
            updatedAfterReturnAnimation = false;
        }
        else if (!returnMovement && currentAnimationTime > 0f)
        {
            currentAnimationTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(endPosition, startPosition, currentAnimationTime * (1 / maxAnimationTime));
        }
        else if (card.health <= 0)
        {
            OnDeath();
        }

        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y, 0);
    }

    public void BenchOrUnbench() 
    {
        if (!canBeBenched || !playerCard || CombatManager.combatManager.gamePhase == 2||rightClicked|| Input.GetMouseButtonDown(1)) return;
        benched = !benched;

        if (benched)
        {
            CombatManager.combatManager.playerCombatCards[slot] = CombatManager.combatManager.playerBenchCards[slot];
            CombatManager.combatManager.playerBenchCards[slot] = this;
            if (CombatManager.combatManager.playerCombatCards[slot] != null)
            {
                CombatManager.combatManager.playerCombatCards[slot].benched = !benched;
                CombatManager.combatManager.playerCombatCards[slot].PutOnOrOffTheBench();
            }
            SoundManager.soundManager.Play("CardSlide");
        }
        else
        {
            CombatManager.combatManager.playerBenchCards[slot] = CombatManager.combatManager.playerCombatCards[slot];
            CombatManager.combatManager.playerCombatCards[slot] = this;
            if (CombatManager.combatManager.playerBenchCards[slot] != null)
            {
                CombatManager.combatManager.playerBenchCards[slot].benched = !benched;
                CombatManager.combatManager.playerBenchCards[slot].PutOnOrOffTheBench();
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
            CombatManager.combatManager.enemyCombatCards[slot] = CombatManager.combatManager.enemyBenchCards[slot];
            CombatManager.combatManager.enemyBenchCards[slot] = this;
            if (CombatManager.combatManager.enemyCombatCards[slot] != null)
            {
                CombatManager.combatManager.enemyCombatCards[slot].benched = !benched;
                CombatManager.combatManager.enemyCombatCards[slot].PutOnOrOffTheBenchEnemyCards();
            }
        }
        else
        {
            CombatManager.combatManager.enemyBenchCards[slot] = CombatManager.combatManager.enemyCombatCards[slot];
            CombatManager.combatManager.enemyCombatCards[slot] = this;
            if (CombatManager.combatManager.enemyBenchCards[slot] != null)
            {
                CombatManager.combatManager.enemyBenchCards[slot].benched = !benched;
                CombatManager.combatManager.enemyBenchCards[slot].PutOnOrOffTheBenchEnemyCards();
            }
        }

        PutOnOrOffTheBenchEnemyCards();
    }
    public void PutOnOrOffTheBench() 
    {
        if (benched) 
        {
            MoveAnimationStarter(0.5f, CombatManager.combatManager.playerBenchSlots[slot].transform.position, false);
            return;
        }
        MoveAnimationStarter(0.5f, CombatManager.combatManager.playerCombatSlots[slot].transform.position, false);
    }
    public void PutOnOrOffTheBenchEnemyCards()
    {
        if (benched)
        {
            MoveAnimationStarter(0.5f, CombatManager.combatManager.enemyBenchSlots[slot].transform.position, false);
            return;
        }
        MoveAnimationStarter(0.5f, CombatManager.combatManager.enemyCombatSlots[slot].transform.position, false);
    }
    public void MoveAnimationStarter(float time, Vector3 end, bool returnMove)
    {
        maxAnimationTime = time;
        currentAnimationTime = time;
        endPosition = end;
        startPosition = transform.position;
        returnMovement = returnMove;
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
                if (!CombatManager.combatManager.enemy.huntAI)
                {
                    if (card.canRevive) CombatManager.combatManager.enemyDeck.discardPile.Add(card);
                }
                else if (CombatManager.combatManager.battleReward.Count < 3)
                {
                    if (card.canRevive) CombatManager.combatManager.battleReward.Add(card);
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
            transform.GetChild(14 + index).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
        else if (card.sigils[1] == sigil)
        {
            transform.GetChild(15 + index).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
        }
        else
        {
            transform.GetChild(16).GetComponent<Image>().color = new Color(1, 1, 1, alpha);
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
            transform.GetChild(14 + index).GetComponent<Image>().sprite = spriteToUse;
        }
        else if (card.sigils[1] == sigil)
        {
            transform.GetChild(15 + index).GetComponent<Image>().sprite = spriteToUse;
        }
        else
        {
            transform.GetChild(16).GetComponent<Image>().sprite = spriteToUse;
        }
    }
    public void RemoveCardFromCardCollections() 
    {
        if (benched)
        {
            if(playerCard && CombatManager.combatManager.playerBenchCards[slot] == this) CombatManager.combatManager.playerBenchCards[slot] = null;
            else if (CombatManager.combatManager.enemyBenchCards[slot] == this) CombatManager.combatManager.enemyBenchCards[slot] = null;
        }
        else
        {
            if (playerCard && CombatManager.combatManager.playerCombatCards[slot] == this) CombatManager.combatManager.playerCombatCards[slot] = null;
            else if (CombatManager.combatManager.enemyCombatCards[slot] == this) CombatManager.combatManager.enemyCombatCards[slot] = null;
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
                if (!CombatManager.combatManager.enemy.huntAI)
                {
                    if (card.canRevive) CombatManager.combatManager.enemyDeck.discardPile.Add(card);
                }
                else if (CombatManager.combatManager.battleReward.Count < 3)
                {
                    if (card.canRevive) CombatManager.combatManager.battleReward.Add(card);
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
