using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CardInHand : MonoBehaviour
{
    public Card card;
    void Start()
    {
        UpdateCardAppearance();
    }

    public void OnClick()
    {

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
