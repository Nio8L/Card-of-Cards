using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public Camera mapCamera;

    public int sensitivity;

    // Load animation:
    Vector3 startPosition;
    float animationTime;
    float animationSpeed = 1;

    Vector3 dragStart = Vector3.one * 10000;
    Vector3 cameraStartPosition;
    float savedIncrease;
    private void Start()
    {
        SetUpCameraPosition();
    }

    private void Update()
    {
        // Quick and ducktape-e fix to the scrolling issue
        if (DeckUtilities.deckUtilities.activeDisplays.Count > 0) return;

        //Scroll with the mouse wheel
        if(!MapManager.mapManager.canScroll) return;
        
            if (Input.GetAxis("Mouse ScrollWheel") != 0f && savedIncrease == 0)
            {
                if (animationTime > 0f){
                    animationSpeed = 4f;
                }else{
                    float limitTop = 9f;
                    if (!DataPersistenceManager.DataManager.inTutorial) limitTop = MapManager.mapManager.transform.GetChild(MapManager.mapManager.transform.childCount-1).transform.position.y;

                    float newCameraY = mapCamera.transform.position.y + Input.GetAxis("Mouse ScrollWheel") * sensitivity;
                    AnimationUtilities.MoveToPoint(mapCamera.transform, 0.25f, 0, new Vector3(0, Math.Clamp(newCameraY, 0, limitTop), -10));
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

        // Drag to scroll
        if (!Input.GetMouseButtonUp(0) && !AnimationUtilities.CheckForAnimation(gameObject) && animationTime <= 0){ 
            if (Input.GetMouseButtonDown(0)){
                dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                cameraStartPosition = mapCamera.transform.position;
                savedIncrease = 0;
            }
            if (Input.GetMouseButton(0)){
                if (dragStart != Vector3.one * 10000){
                    float limitTop = 9f;
                    if (!DataPersistenceManager.DataManager.inTutorial) limitTop = MapManager.mapManager.transform.GetChild(MapManager.mapManager.transform.childCount-1).transform.position.y;

                    Vector3 currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    savedIncrease += dragStart.y - currentMousePosition.y;
                    float newY = cameraStartPosition.y + savedIncrease;
                    mapCamera.transform.position = new Vector3(0, Math.Clamp(newY, 0, limitTop), mapCamera.transform.position.z);
                    dragStart = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
        }else{
            dragStart = Vector3.one * 10000;
                savedIncrease = 0;
        }
    }

    public void SetUpCameraPosition()
    {
        if(MapManager.mapManager.currentNode != null){
            mapCamera.transform.position = new Vector3(0, MapManager.mapManager.currentNode.transform.position.y, -10);
        }
    }

    public void FirstLoadAnimation(){
        startPosition = MapManager.mapManager.transform.GetChild(MapManager.mapManager.transform.childCount-1).transform.position;
        startPosition.z = -10;
        animationTime = 4f;
    }

}
