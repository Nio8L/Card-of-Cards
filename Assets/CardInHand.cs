using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardInHand : MonoBehaviour
{
    public Card card;
    public Deck deck;
    Transform slot;
    void Start()
    {
        UpdateCardAppearance();
    }
    private void Update()
    {
        if (deck.selectedCard == transform)
        {
            if (CheckForSlot())
            {
                PlayCard(slot);
                deck.selectedCard = null;
            }
        }
    }

    public void OnClick()
    {
        if (deck.selectedCard != transform) deck.selectedCard = transform; 
        else deck.selectedCard = null;
    }

    void UpdateCardAppearance()
    {
        transform.GetChild(0).GetComponent<Image>().sprite = card.image;
        //transform.GetChild(2).GetComponent<Image>().sprite = ;
        transform.GetChild(3).GetComponent<TextMeshProUGUI>().text = card.name;
        transform.GetChild(4).GetComponent<TextMeshProUGUI>().text = card.cost.ToString();
        transform.GetChild(5).GetComponent<TextMeshProUGUI>().text = card.hp.ToString();
        transform.GetChild(6).GetComponent<TextMeshProUGUI>().text = card.attack.ToString();

    }

    bool CheckForSlot()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.forward, 100);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "CardSlot")
            {
                slot = hit.collider.transform;
                return true;
            }
        }
        return false;
    }

    public void PlayCard(Transform _transform)
    {
        GameObject cardToCreate =  Instantiate(deck.cardInCombatPrefab, _transform.position, Quaternion.identity);
        cardToCreate.transform.SetParent(deck.canvasTransform);
        cardToCreate.transform.localScale = Vector3.one;
        CardInCombat cardInCombat = cardToCreate.GetComponent<CardInCombat>();
        cardInCombat.card = card;
        cardInCombat.deck = deck;
        Destroy(gameObject);
    }
}
