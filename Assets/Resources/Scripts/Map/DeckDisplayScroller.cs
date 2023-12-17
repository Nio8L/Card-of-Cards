using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckDisplayScroller : MonoBehaviour
{
    public GameObject deckDisplayObject;

    public DeckDisplay deckDisplay;

    RectTransform rectTransform;

    public int sensitivity;

    private void Start() {
        rectTransform = deckDisplayObject.GetComponent<RectTransform>();
    }

    private void Update() {
        if(Input.GetAxis("Mouse ScrollWheel") != 0f){
           if (deckDisplay.cardDisplays.Count > 15){
                if(rectTransform.localPosition.y <= deckDisplay.cardDisplays.Count / 15 * 770 && rectTransform.localPosition.y >= 0){
                    LeanTween.moveY(gameObject, deckDisplayObject.transform.position.y - Input.GetAxis("Mouse ScrollWheel") * sensitivity, 0.5f);
                }
           }
        }
        if(rectTransform.localPosition.y > deckDisplay.cardDisplays.Count / 15 * 770){
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, deckDisplay.cardDisplays.Count / 15 * 760, 0);
        }
        if(rectTransform.localPosition.y < 0){
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, 0, 0);
        }
    }

    private void OnEnable() {
        if(rectTransform != null){
            rectTransform.localPosition = Vector3.zero;
        }
    }
}
