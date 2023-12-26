using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public Camera mapCamera;

    public int sensitivity;

    // Load animation:
    Vector3 startPosition;
    float animationTime;
    float animationSpeed = 1;

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
                if (animationTime > 0f){
                    animationSpeed = 4f;
                }else{
                    LeanTween.moveY(gameObject, mapCamera.transform.position.y + Input.GetAxis("Mouse ScrollWheel") * sensitivity, 0.25f);
                }
            }
        }
        if (animationTime > 0f){
            animationTime -= Time.deltaTime * animationSpeed;
            if (animationTime <= 3f){
                transform.position = Vector3.Lerp(startPosition, new Vector3(0, 1, -10), 1-(float)Math.Pow(animationTime/3f, 2));
            }else{
                transform.position = startPosition;
            }
        }
    }

    public void SetUpCameraPosition()
    {
        if(MapManager.currentNode != null){
            mapCamera.transform.position = new Vector3(0, MapManager.currentNode.transform.position.y, -10);
        }
    }

    public void FirstLoadAnimation(){
        startPosition = MapManager.mapManager.transform.GetChild(MapManager.mapManager.transform.childCount-1).transform.position;
        startPosition.z = -10;
        animationTime = 4f;
    }

}
