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
    [Header("Displaying")]

    public DisplayType displayType = DisplayType.Display;
    public enum DisplayType{
        Display,
        Combat,
        Hand
    }

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

    [Header("Sigil Stars")]
    public Sprite activeStar;
    public Sprite  selectedActiveStar;
    public Image sigilStar1;
    public Image sigilStar2;
    public Image sigilStar3;

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

    [Header("Death Marks")]
    public Sprite deathMarkScratch;
    public Sprite deathMarkBite;
    public Sprite deathMarkPoison;

    private void Start() {
        UpdateCardAppearance();
    }

    public void UpdateCardAppearance()
    {
        icon.sprite = card.image;

        Sprite damageIcon;
        if(card.typeOfDamage == Card.TypeOfDamage.Heart) damageIcon = heartDamageIcon;
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
        sigil1.transform.GetComponent<SigilTooltip>().UpdateSigilTooltip();
        sigil2.transform.GetComponent<SigilTooltip>().UpdateSigilTooltip();
        sigil3.transform.GetComponent<SigilTooltip>().UpdateSigilTooltip();

        if (displayType == DisplayType.Combat)
        {
            sigilStar1.color = new Color(1, 1, 1, 0);
            sigilStar2.color = new Color(1, 1, 1, 0);
            sigilStar3.color = new Color(1, 1, 1, 0);
            ShowSigilStars();
        }
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

    //Update the card appearence after a delay
    public IEnumerator DelayUpdateCardAppearence(float delay){
        yield return new WaitForSeconds(delay);

        UpdateCardAppearance();
    }

    //Update cost text
    public void UpdateCostText(){
        if(CombatManager.combatManager.deck.energy < card.cost){
            cardCost.color = new Color(0.75f, 0, 0, 1);
        }else{
            cardCost.color = new Color(0, 0, 0, 1);
        }
    }

    //Update tilt
    public void UpdateTilt(float tiltAngle){
        transform.rotation = Quaternion.Euler(0, 0, tiltAngle);
    }

    //Show and hide the active start depending on if the sigil is active
    public void ShowSigilStars(){
        for (int i = 0; i < card.sigils.Count; i++){
            Sigil sigil = card.sigils[i];
            int alpha = 0;
            ActiveSigil activeSigil = sigil.GetActiveSigil();
            if (activeSigil != null && activeSigil.canBeUsed) alpha = 1;

            if (card.sigils[0] == sigil)
            {
                sigilStar1.color = new Color(1, 1, 1, alpha);
                sigilStar1.sprite = activeStar;
            }
            else if (card.sigils[1] == sigil)
            {
                sigilStar2.color = new Color(1, 1, 1, alpha);
                sigilStar2.sprite = activeStar;
            }
            else
            {
                sigilStar3.color = new Color(1, 1, 1, alpha);
                sigilStar3.sprite = activeStar;
            }
        }
    }

    //Set the active sigil star if the sigil can be used
    public void SetActiveSigilStar(Sigil sigil)
    {
        Sprite spriteToUse;
        ActiveSigil activeSigil = sigil.GetActiveSigil();
        if (activeSigil != null && activeSigil.canBeUsed) spriteToUse = selectedActiveStar;
        else                                              spriteToUse = activeStar;

        if (card.sigils[0] == sigil)
        {
            sigilStar1.sprite = spriteToUse;
        }
        else if (card.sigils[1] == sigil)
        {
            sigilStar2.sprite = spriteToUse;
        }
        else
        {
            sigilStar3.sprite = spriteToUse;
        }
        Debug.Log("updated active");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (displayType == DisplayType.Display)
        {
            //transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
            AnimationUtilities.ChangeScale(transform, 0.1f, 0, Vector3.one * 1.5f);
            transform.SetAsLastSibling();
            SoundManager.soundManager.Play("CardPickUp");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (displayType == DisplayType.Display)
        {
            AnimationUtilities.ChangeScale(transform, 0.1f, 0, Vector3.one);
        }
    }

    private void OnDisable() {
        transform.localScale = Vector3.one;
    }

    private void OnEnable() {
        //Prevent scaling issues when opening and closing the menu
        if (displayType == DisplayType.Combat)
        {
            transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        }
    }
}
