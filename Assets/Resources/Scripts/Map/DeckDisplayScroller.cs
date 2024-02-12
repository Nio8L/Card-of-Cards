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
           if (deckDisplay.cardDisplays.Count > 12){
                if(rectTransform.localPosition.y <= deckDisplay.cardDisplays.Count / 12 * 770 && rectTransform.localPosition.y >= 0){
                    AnimationUtilities.MoveToPoint(transform, 0.25f, 0, new Vector3(deckDisplayObject.transform.position.x, deckDisplayObject.transform.position.y - Input.GetAxis("Mouse ScrollWheel") * sensitivity, deckDisplayObject.transform.position.z));
                }
           }
        }
        if(rectTransform.localPosition.y > deckDisplay.cardDisplays.Count / 12 * 770){
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, deckDisplay.cardDisplays.Count / 12 * 760, 0);
        }
        if(rectTransform.localPosition.y < 0){
            rectTransform.localPosition = new Vector3(rectTransform.localPosition.x, 0, 0);
        }
    }

    private void OnEnable() {
        if(rectTransform != null){
            //rectTransform.localPosition = Vector3.zero;
        }
    }
}
