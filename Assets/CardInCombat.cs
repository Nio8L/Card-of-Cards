using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardInCombat : MonoBehaviour
{
    public Card card;
    public Deck deck;
    public Card.TypeOfDamage lastTypeOfDamage;

    public bool playerCard = true;
    public int slot = 0;

    Vector3 startPosition;
    Vector3 endPosition;
    float curentAnimationTime = -0.5f;
    float maxAnimationTime = 0.5f;
    void Start()
    {
        UpdateCardAppearance();
    }

    private void Update()
    {

        if (curentAnimationTime > -0.5f)
        {
            curentAnimationTime -= Time.deltaTime;
            transform.position = Vector3.Lerp(endPosition, startPosition, Mathf.Abs(curentAnimationTime)*2);
        }
        else if (card.health <= 0)
        {
            card.CreateCard(lastTypeOfDamage);

            if (playerCard) deck.combatManager.playerCards[slot] = null;
            else deck.combatManager.enemyCards[slot] = null;

            Destroy(gameObject);
        }
    }

    public void UpdateCardAppearance()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = card.image;
        //transform.GetChild(2).GetComponent<Image>().sprite = ;
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = card.name;
        transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = card.cost.ToString();
        transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = card.health.ToString();
        transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = card.attack.ToString();

    }

    public void PerformShortAttackAnimation()
    {
        curentAnimationTime = maxAnimationTime;
        endPosition = new Vector3(transform.position.x, 0f, 0f);
        startPosition = transform.position;
    }
}
