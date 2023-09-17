using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


[ExecuteInEditMode()]
public class Tooltip : MonoBehaviour
{
    public TextMeshProUGUI headerField;
    public TextMeshProUGUI contentField;

    public LayoutElement layoutElement;

    public Canvas parentCanvas;

    public int characterWrapLimit;

    public RectTransform rectTransform;

    private Vector2 position;

    private void Awake() {
        rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content, string header = ""){
        if(string.IsNullOrEmpty(header)){
            headerField.gameObject.SetActive(false);
        }else{
            headerField.gameObject.SetActive(true);
            headerField.text = header;
        }

        contentField.text = content;

        int headerLength = headerField.text.Length;
        int contentLength = contentField.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }

    private void Start() {

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform, Input.mousePosition,
            parentCanvas.worldCamera,
            out Vector2 pos);
    }

    private void Update() {
        position = Input.mousePosition;
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentCanvas.transform as RectTransform,
            position, parentCanvas.worldCamera,
            out Vector2 movePos);

        float pivotX = Input.mousePosition.x / Screen.width;
        float pivotY = Input.mousePosition.y / Screen.height;

        rectTransform.pivot = new Vector2(pivotX, pivotY);
        transform.position = parentCanvas.transform.TransformPoint(movePos);
    }
}
