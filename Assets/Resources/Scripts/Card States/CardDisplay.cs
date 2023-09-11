using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class CardDisplay : MonoBehaviour
{
    [Header("Displayed Card")]
    public Card card;
    
    [Header("Card Visuals")]
    public Image icon;
    public TextMeshProUGUI cardName;

    [Header("Card Stats")]
    public Image attackTypeIcon;
    public TextMeshProUGUI cardCost;
    public TextMeshProUGUI cardHealth;
    public TextMeshProUGUI cardAttack;

    [Header("Sigils")]
    public Image sigil1;
    public Image sigil2;
    public Image sigil3;

    [Header("Injuries")]
    public Image injury1;
    public Image injury2;
    public Image injury3;

    [Header("Injury Icons")]
    public Sprite biteDamageIcon;
    public Sprite scrachDamageIcon;
    public Sprite poisonDamageIcon;

    private void Start() {
        UpdateCardAppearance();
    }

    public void UpdateCardAppearance()
    {
        icon.sprite = card.image;

        Sprite damageIcon;
        if (card.typeOfDamage == Card.TypeOfDamage.Bite) damageIcon = biteDamageIcon;
        else if (card.typeOfDamage == Card.TypeOfDamage.Scratch) damageIcon = scrachDamageIcon;
        else damageIcon = poisonDamageIcon;
        attackTypeIcon.sprite = damageIcon;

        cardName.text = card.name;
        cardCost.text = card.cost.ToString();
        cardHealth.text = card.health.ToString();
        cardAttack.text = card.attack.ToString();


        sigil1.color = new Color(1, 1, 1, 0);
        sigil2.color = new Color(1, 1, 1, 0);
        sigil3.color = new Color(1, 1, 1, 0);
        // Set sigil sprites
        if (card.sigils.Count == 1)
        {
            sigil1.sprite = card.sigils[0].image;
            sigil1.color = new Color(1, 1, 1, 1);
        }
        else if (card.sigils.Count == 2)
        {
            sigil2.sprite = card.sigils[0].image;
            sigil3.sprite = card.sigils[1].image;
            sigil2.color = new Color(1, 1, 1, 1);
            sigil3.color = new Color(1, 1, 1, 1);
        }
        else if (card.sigils.Count == 3)
        {
            sigil1.sprite = card.sigils[0].image;
            sigil2.sprite = card.sigils[1].image;
            sigil3.sprite = card.sigils[2].image;
            sigil1.color = new Color(1, 1, 1, 1);
            sigil2.color = new Color(1, 1, 1, 1);
            sigil3.color = new Color(1, 1, 1, 1);
        }
        
        // Set injury marks
        injury1.color = new Color(1, 1, 1, 0);
        injury2.color = new Color(1, 1, 1, 0);
        injury3.color = new Color(1, 1, 1, 0);
        foreach (Card.TypeOfDamage injury in card.injuries)
        {
            if (injury == Card.TypeOfDamage.Bite) injury1.color = new Color(1, 1, 1, 1);
            else if (injury == Card.TypeOfDamage.Scratch) injury2.color = new Color(1, 1, 1, 1);
            else if (injury == Card.TypeOfDamage.Poison) injury3.color = new Color(1, 1, 1, 1);
        }
        transform.GetChild(7).GetComponent<SigilTooltip>().UpdateSigilTooltip();
        transform.GetChild(8).GetComponent<SigilTooltip>().UpdateSigilTooltip();
        transform.GetChild(9).GetComponent<SigilTooltip>().UpdateSigilTooltip();
    }
    
    public void HealCard(){
        if(SceneManager.GetActiveScene().name == "Map" && MapManager.mapManager.currentNode.roomType == MapNode.RoomType.Graveyard && !MapManager.mapManager.currentNode.used){
            card.AcceptLostSoul();
            MapManager.mapManager.currentNode.used = true;
            MapManager.mapManager.deckDisplay.canClose = true;
        }
        UpdateCardAppearance();
    }

}
