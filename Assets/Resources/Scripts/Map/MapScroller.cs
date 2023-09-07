using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapScroller : MonoBehaviour
{
    public Camera mapCamera;

    public int sensitivity;

    public MapManagerManager mapManagerManager;

    private void Start() {
        
    }

    private void Update() {
        if(Input.GetAxis("Mouse ScrollWheel") != 0f){
            //mapCamera.transform.position = new Vector3(mapCamera.transform.position.x, mapCamera.transform.position.y + Input.GetAxis("Mouse ScrollWheel")*3, mapCamera.transform.position.z);
            LeanTween.moveY(gameObject, mapCamera.transform.position.y + Input.GetAxis("Mouse ScrollWheel") * sensitivity, 0.5f);
        }
    }

    public void SetUpCameraPosition(){
        mapCamera.transform.position = new Vector3(0, mapManagerManager.mapManagers[0].currentNode.transform.position.y, -10);
    }
    
}
