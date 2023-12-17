using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardSelected : MonoBehaviour, IPointerClickHandler
{
    public EventCardSlot eventCardSlot;


    public void Unselect(){
        eventCardSlot.RemoveCard();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        Unselect();
    }
}
