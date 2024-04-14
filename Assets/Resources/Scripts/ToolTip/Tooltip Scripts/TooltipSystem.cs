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
        AnimationUtilities.ChangeCanvasAlpha(tooltipSystem.transform, 0.3f, 0, 1);
    }

    public static void QuickHide(){
        tooltipSystem.tooltip.gameObject.SetActive(false);
    }

    public static void Hide(){
        if(tooltipSystem != null && tooltipSystem.canvasGroup != null){
            AnimationUtilities.ChangeCanvasAlpha(tooltipSystem.transform, 0.1f, 0, 0);
            tooltipSystem.StartCoroutine(CloseTooltip(0.1f));
        }
    }

    private static IEnumerator CloseTooltip(float delay){
        yield return new WaitForSeconds(delay);

        tooltipSystem.tooltip.gameObject.SetActive(false);
    }

    private void OnSceneChange(Scene current, Scene next)
    {
        QuickHide();
    }
}
