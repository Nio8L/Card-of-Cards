using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public Camera mapCamera;

    public int sensitivity;

    private void Start()
    {
        SetUpCameraPosition();
    }

    private void Update()
    {
        if (!MapManager.mapManager.deckDisplay.deckDisplay.activeSelf)
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                LeanTween.moveY(gameObject, mapCamera.transform.position.y + Input.GetAxis("Mouse ScrollWheel") * sensitivity, 0.5f);
            }
        }
    }

    public void SetUpCameraPosition()
    {
        if(MapManager.currentNode != null){
            mapCamera.transform.position = new Vector3(0, MapManager.currentNode.transform.position.y, -10);
        }
    }

}
