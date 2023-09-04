using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManagerManager : MonoBehaviour
{
    public List<MapManager> mapManagers;
    
    public MapScroller mapScroller;

    private void Start() {
        mapManagers = FindMapManagers();

    try{
        foreach(MapManager mapManager in mapManagers){
            if(!mapManager.gameObject.activeSelf){
                    mapManagers.Remove(mapManager);
                    Destroy(mapManager.gameObject);
                }
            }
            mapScroller.SetUpCameraPosition();
        }catch{
            //Debug.Log("error with map manager shenanigans, ignore this, all good :D");
            mapScroller.SetUpCameraPosition();
        }
    }

    private List<MapManager> FindMapManagers(){
        IEnumerable<MapManager> MapManagers = FindObjectsOfType<MapManager>(true);
        return new List<MapManager>(MapManagers);
    }
}
