using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using System.Linq;

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

    [Header("Injury Icons")]
    public Sprite[] biteInjuryIcon;
    public Sprite[] scratchInjuryIcon;
    public Sprite[] poisonInjuryIcon;

    [Header("Damage Icons")]
    public Sprite biteDamageIcon;
    public Sprite scrachDamageIcon;
    public Sprite poisonDamageIcon;
    public Sprite heartDamageIcon;


    private void Start() {
        UpdateCardAppearance();
    }

    public void UpdateCardAppearance()
    {
        icon.sprite = card.image;

        Sprite damageIcon;
        if(card.name == "Lost Soul") damageIcon = heartDamageIcon;
        else if (card.typeOfDamage == Card.TypeOfDamage.Bite) damageIcon = biteDamageIcon;
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
            if(DataPersistenceManager.DataManager.redMarkers){
                if(injury == Card.TypeOfDamage.Bite){
                    injury1.sprite = biteInjuryIcon[1];
                    injury1.color = new Color(1, 1, 1, 1);
                }else if(injury == Card.TypeOfDamage.Scratch){
                    injury2.sprite = scratchInjuryIcon[1];
                    injury2.color = new Color(1, 1, 1, 1);
                }else if(injury == Card.TypeOfDamage.Poison){
                    injury3.sprite = poisonInjuryIcon[1];
                    injury3.color = new Color(1, 1, 1, 1);
                }
            }else{
                if(injury == Card.TypeOfDamage.Bite){
                    injury1.sprite = biteInjuryIcon[0];
                    injury1.color = new Color(1, 1, 1, 1);
                }else if(injury == Card.TypeOfDamage.Scratch){
                    injury2.sprite = scratchInjuryIcon[0];
                    injury2.color = new Color(1, 1, 1, 1);
                }else if(injury == Card.TypeOfDamage.Poison){
                    injury3.sprite = poisonInjuryIcon[0];
                    injury3.color = new Color(1, 1, 1, 1);
                }
            }
        }
        transform.GetChild(7).GetComponent<SigilTooltip>().UpdateSigilTooltip();
        transform.GetChild(8).GetComponent<SigilTooltip>().UpdateSigilTooltip();
        transform.GetChild(9).GetComponent<SigilTooltip>().UpdateSigilTooltip();
    }
    public void SelectCard(){
        if(SceneManager.GetActiveScene().name == "Map"){
            if(MapManager.mapManager.currentNode != null){
                if(MapManager.mapManager.currentNode.GetComponent<MapNode>().thisNode.thisRoom == MapWorld.RoomType.Event){
                    //Check if this card display is displaying an offered card    
                    if(gameObject.GetComponent<CardOffered>() != null){
                        //Find the event and select the card
                        IEnumerable<IEvent> events = FindObjectsOfType<MonoBehaviour>().OfType<IEvent>();
                        foreach(IEvent ievent in events){
                            ievent.SelectCard(this);
                        }

                    //Check if this card display is displaying a selected card
                    }else if(gameObject.GetComponent<CardSelected>() == null){
                        //Place the card on the first available sacrificial spot
                        if(MapManager.mapManager.currentEvent == null) return;
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
