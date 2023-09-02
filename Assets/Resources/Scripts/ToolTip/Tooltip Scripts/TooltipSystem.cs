using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem tooltipSystem;

    public Tooltip tooltip;

    public bool hoveredIsSigil = false;
    public SigilTooltip hoveredSigil;

    public CanvasGroup canvasGroup;

    private void Awake() {
        tooltipSystem = this;

        DontDestroyOnLoad(gameObject);
    }

    public static void Show(string content, string header = ""){
        if(!string.IsNullOrEmpty(content)){
            tooltipSystem.tooltip.SetText(content, header);
            tooltipSystem.tooltip.gameObject.SetActive(true);
        }
        LeanTween.alphaCanvas(tooltipSystem.canvasGroup, 1f, 0.3f);
    }

    public static void QuickHide(){
        tooltipSystem.tooltip.gameObject.SetActive(false);
    }

    public static void Hide(){
        LeanTween.alphaCanvas(tooltipSystem.canvasGroup, 0f, 0.1f);
        LeanTween.delayedCall(0.1f, () => {
            tooltipSystem.tooltip.gameObject.SetActive(false);
        });
        
        
    }
}
