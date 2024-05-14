using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapSaver : MonoBehaviour
{
    [System.Serializable]
    public class EventRoom{
        public string name;

        public bool used;
    }

    [System.Serializable]
    public class Map{
        public int seed;
        public int layerIndex;
        public int nodeIndex;
        public bool hasTraveled;

        public int world;

        public EventRoom eventRoom;

        public Map(){
            seed = 0;
            layerIndex = 0;
            nodeIndex = 0;
            hasTraveled = false;
            world = 0;
            eventRoom = null;
        }
    }

    public static MapSaver mapSaver;

    public int playerHP;

    public Map map;

    private void Start() {
         if(mapSaver != null){
            Destroy(gameObject);
            return;
        }

        mapSaver = this;

        InitializeMapSaver();

        DontDestroyOnLoad(gameObject);
    }

    public void InitializeMapSaver(){
        map = new();
        playerHP = 20;
    }

    public void LoadMapData(){
        ScenePersistenceManager.scenePersistence.currentWorld = map.world;

        MapManager.mapManager.thisWorld = ScenePersistenceManager.scenePersistence.worlds[ScenePersistenceManager.scenePersistence.currentWorld];
        MapManager.mapManager.thisWorld = Instantiate(MapManager.mapManager.thisWorld);

        MapManager.mapManager.thisWorld.mapSeed = map.seed;
        MapManager.mapManager.thisWorld.generalSeed = Mathf.FloorToInt(UnityEngine.Random.value*214783646);
        if (!ScenePersistenceManager.scenePersistence.resetMap){
            MapManager.mapManager.hasTraveled = map.hasTraveled;
        }
        //Generate the world if the seed isn't new
        if(MapManager.mapManager.thisWorld.mapSeed != 0){
            MapManager.GenerateWorld(MapManager.mapManager.thisWorld.mapSeed);
            
            if (MapManager.mapManager.hasTraveled)
            {
                MapManager.SelectNode(MapManager.mapManager.thisWorld.GetGameObjectFromNode(MapManager.mapManager.thisWorld.floor[map.layerIndex].nodes[map.nodeIndex]).GetComponent<MapNode>());
            }
        }

        MapManager.mapManager.mapDeck.playerHealth = playerHP;
        MapManager.mapManager.mapDeck.UpdateHPText();
    }

    public void SaveMapData(){
        map.seed = MapManager.mapManager.thisWorld.mapSeed;
        map.hasTraveled = MapManager.mapManager.hasTraveled;

        if(MapManager.mapManager.onEvent){
            map.eventRoom.name = ScenePersistenceManager.scenePersistence.lastEvent;
            map.eventRoom.used = MapManager.mapManager.eventUsed;
        }else{
            map.eventRoom = null;
        }
        
        if(MapManager.mapManager.hasTraveled){
            map.layerIndex = MapManager.mapManager.currentNodeScript.thisNode.layerIndex;
            map.nodeIndex = MapManager.mapManager.currentNodeScript.thisNode.index;
        }

        map.world = ScenePersistenceManager.scenePersistence.currentWorld;

        playerHP = MapManager.mapManager.mapDeck.playerHealth;
    }

}
