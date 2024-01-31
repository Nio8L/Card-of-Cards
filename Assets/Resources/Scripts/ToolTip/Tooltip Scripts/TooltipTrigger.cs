using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{    
    public string header;
    
    [Multiline()]
    public string content;

    public void OnPointerEnter(PointerEventData eventData){
        StartCoroutine(ShowTooltip(0.5f));
    }

    private IEnumerator ShowTooltip(float delay){
        yield return new WaitForSeconds(delay);

        TooltipSystem.Show(content, header);
    }

    public void OnPointerExit(PointerEventData eventData){
        StopAllCoroutines();
        TooltipSystem.Hide();
    }

    private void OnDestroy() {
        TooltipSystem.Hide();
    }

}
