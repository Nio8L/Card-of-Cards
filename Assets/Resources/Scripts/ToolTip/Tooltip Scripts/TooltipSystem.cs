using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TooltipSystem : MonoBehaviour
{
    public static TooltipSystem tooltipSystem;

    public Tooltip tooltip;

    public bool hoveredIsSigil = false;
    public SigilTooltip hoveredSigil;

    public CanvasGroup canvasGroup;

    private void Awake() {
        if(tooltipSystem != null){
            Destroy(gameObject);
            return;
        }
        
        tooltipSystem = this;

        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += OnSceneChange;
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

    private void OnSceneChange(Scene current, Scene next)
    {
        QuickHide();
    }
}
