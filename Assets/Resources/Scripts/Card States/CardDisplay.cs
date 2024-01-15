using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.EventSystems;

public class CardDisplay : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
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

    [Header("Crown")]
    public Image crown;

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

        if (card.captain)
        {
            crown.gameObject.SetActive(true);
        }
        else
        {
            crown.gameObject.SetActive(false);
        }


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
            sigil1.sprite = card.sigils[0].image;
            sigil2.sprite = card.sigils[1].image;
            sigil1.color = new Color(1, 1, 1, 1);
            sigil2.color = new Color(1, 1, 1, 1);
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
        if(SceneManager.GetActiveScene().name == "Map"){
            if (MapManager.mapManager.currentNode != null){
                if (MapManager.mapManager.currentNode.roomType == MapNode.RoomType.Graveyard && !MapManager.mapManager.currentNode.used){
                    if (card.name != "LostSoul"){
                        if (card.injuries.Count > 0 || !MapManager.mapManager.mapDeck.HasInjuredCards())
                        {
                            card.AcceptLostSoul();
                            SoundManager.soundManager.Play("LostSoul");

                            GameObject soulHeartObject = Resources.Load<GameObject>("Prefabs/LostSoulHeart");

                            Instantiate(soulHeartObject, transform.position, Quaternion.identity);
                
                            LostSoulVisuals soulHeart;

                            soulHeart = Instantiate(soulHeartObject, transform.position, Quaternion.identity).GetComponent<LostSoulVisuals>();
                            soulHeart.angle = 120f;
                            soulHeart.primaryHeart = false;

                            soulHeart = Instantiate(soulHeartObject, transform.position, Quaternion.identity).GetComponent<LostSoulVisuals>();
                            soulHeart.GetComponent<LostSoulVisuals>().angle = 240f;
                            soulHeart.primaryHeart = false;

                            MapManager.mapManager.currentNode.used = true;
                            MapManager.mapManager.deckDisplay.canClose = true;
                            LeanTween.delayedCall(2, () => {

                                MapManager.mapManager.deckDisplay.ShowDeck(5, 250);
                            });
                        }
                    }
                }
            }
        }
        UpdateCardAppearance();
    }

    public void PickCard()
    {
        if (SceneManager.GetActiveScene().name == "Map")
        {
            if (MapManager.mapManager.currentNode != null)
            {
                if (MapManager.mapManager.currentNode.roomType == MapNode.RoomType.RestSite && !MapManager.mapManager.currentNode.used)
                {
                    MapManager.mapManager.mapDeck.cards.Add(card);
                    Destroy(GameObject.Find("ThreeCardChoice"));
                }
            }
        }
    }

    public void SelectCardForSacrifice(){
        if(SceneManager.GetActiveScene().name == "Map"){
            if(MapManager.mapManager.currentNode != null){
                if(MapManager.mapManager.currentNode.roomType == MapNode.RoomType.Event){
                    //Check if this card display is displaying an offered card    
                    if(gameObject.GetComponent<CardOffered>() != null){
                        MapManager.mapManager.currentEvent.GetComponent<ExchangeShop>().SelectOfferedCard(gameObject, card);
                    //Check if this card display is displaying a selected card
                    }else if(gameObject.GetComponent<CardSelected>() == null){
                        //Place the card on the first available sacrificial spot
                        MapManager.mapManager.currentEvent.GetComponent<EventCardSlotHandler>().AddCardOnAvailableSlot(card);
                    }
                }
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
        transform.SetAsLastSibling();
        SoundManager.soundManager.Play("CardPickUp");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = Vector3.one;
    }

    private void OnDisable() {
        transform.localScale = Vector3.one;
    }
}
