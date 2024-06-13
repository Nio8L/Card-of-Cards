using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class TotemDisplay : MonoBehaviour
{
    public Totem totem;
    public void UpdateDisplay(Totem newTotem){
        totem = newTotem;

        Image thisImage = GetComponent<Image>();
        TooltipTrigger thisTrigger = GetComponent<TooltipTrigger>();
        if (totem != null){
            thisImage.color = Color.white;
            thisImage.sprite = totem.totemSprite;
            thisTrigger.header  = totem.visibleName;
            thisTrigger.content = totem.description;
        }else{
            thisImage.color = new Color(0, 0, 0, 0);
            thisImage.sprite = null;
            thisTrigger.header = "";
            thisTrigger.content  = "";
        }
    }

    public void ActivateEffect(int index) {
        TotemManager.UseActive(index);
    }
}
