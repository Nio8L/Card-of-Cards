using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public Camera mapCamera;

    public int sensitivity;

    private void Start() {
        mapCamera.transform.position = new Vector3(mapCamera.transform.position.x, MapManager.mapManager.currentNode.transform.position.y, mapCamera.transform.position.z);
    }

    private void Update() {
        if(Input.GetAxis("Mouse ScrollWheel") != 0f){
            //mapCamera.transform.position = new Vector3(mapCamera.transform.position.x, mapCamera.transform.position.y + Input.GetAxis("Mouse ScrollWheel")*3, mapCamera.transform.position.z);
            LeanTween.moveY(gameObject, mapCamera.transform.position.y + Input.GetAxis("Mouse ScrollWheel") * sensitivity, 0.5f);
        }
    }
    
}
