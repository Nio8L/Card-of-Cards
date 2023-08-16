using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem tooltipSystem;

    public Tooltip tooltip;

    private void Awake() {
        tooltipSystem = this;
    }

    public static void Show(string content, string header = ""){
        if(!string.IsNullOrEmpty(content)){
            tooltipSystem.tooltip.SetText(content, header);
            tooltipSystem.tooltip.gameObject.SetActive(true);
        }
    }

    public static void Hide(){
        tooltipSystem.tooltip.gameObject.SetActive(false);
    }
}
