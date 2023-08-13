using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardInCombat : MonoBehaviour
{
    public Card card;
    public Deck deck;
    void Start()
    {
        UpdateCardAppearance();
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
}
