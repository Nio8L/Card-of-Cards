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
        Vector2 position = Input.mousePosition;
            float x = position.x / Screen.width;
            float y = position.y / Screen.height;
            if (x <= y && x <= 1 - y) //left
                rectTransform.pivot = new Vector2(-0.15f, y);
            else if (x >= y && x <= 1 - y) //bottom
                rectTransform.pivot = new Vector2(x, -0.1f);
            else if (x >= y && x >= 1 - y) //right
                rectTransform.pivot = new Vector2(1.1f, y);
            else if (x <= y && x >= 1 - y) //top
                rectTransform.pivot = new Vector2(x, 1.3f);
            transform.position = position;
    }
}
