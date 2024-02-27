using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour, IDataPersistence
{
    public static MapManager mapManager;
    public MapWorld thisWorld;
    public MapDeck mapDeck;
    public GameObject currentNode;
    public MapNode currentNodeScript;
    public GameObject currentEvent;
    public bool REGENERATE;
    public bool canTravel;
    public bool canScroll;
    public bool eventUsed;
    public bool hasTraveled = false;
    public Transform eventCanvas;
    public GameObject mapLegend;
    
    // Particles and other
    public GameObject restSiteParticles;
    

    public List<GameObject> selectableNodes = new List<GameObject>();
    void Awake(){
        mapManager = this;
        thisWorld = Instantiate(thisWorld);
    }
    void Start(){
        GenerateWorld();
        Random.InitState(mapManager.thisWorld.generalSeed);
    }
    void Update(){
        if (REGENERATE){
            ClearMap();
            GenerateWorld();
            REGENERATE = false;
        }
    }
    static void GenerateWorld(){
        //Prevent the map generating a world if the seed isn't new
        if(mapManager.thisWorld.mapSeed != 0){
            return;
        }
        // Reset the timer
        Timer.timer.time = 0;
        // Generate a new map
        GenerateWorld(Mathf.FloorToInt(Random.value*214783646));
    }
    static void GenerateWorld(int seed){
        // Generate a map with a given seed
        mapManager.thisWorld.mapSeed = seed;
        mapManager.thisWorld.GenerateLayout  ();
        mapManager.thisWorld.GenerateNodes   ();
        mapManager.thisWorld.AssignRooms     ();
        mapManager.thisWorld.AssignEncounters();

        mapManager.MakeBottomLayerSelectable ();
    }
    static void ClearMap(){
        // Destroy all nodes and lines
        for (int i = mapManager.transform.childCount-1; i >= 0; i--){
            Destroy(mapManager.transform.GetChild(i).gameObject);
        }
        mapManager.selectableNodes.Clear();
        mapManager.thisWorld.mapSeed = 0;
    }
    public static void SelectNode(MapNode mapNode){
        // Change the color of the old selected node
        if (mapManager.currentNode != null) mapManager.currentNode.GetComponent<SpriteRenderer>().color = Color.white;
        
        // Select the new node
        mapManager.currentNode = mapNode.gameObject;
        mapManager.currentNode.GetComponent<SpriteRenderer>().color = Color.red;
        mapManager.currentNodeScript = mapNode;

        // Clear the glow effect
        for (int i = 0; i < mapManager.selectableNodes.Count; i++){
            mapManager.selectableNodes[i].transform.GetChild(0).gameObject.SetActive(false);
        }
        mapManager.selectableNodes.Clear();

        // Make the new available nodes glow
        List<MapWorld.Node> connections = mapManager.thisWorld.GetConnectedNodes(mapNode.thisNode);
        for (int i = 0; i < connections.Count; i++){
            GameObject targetNode = mapManager.thisWorld.GetGameObjectFromNode(connections[i]); 
            MakeNodeSelectable(targetNode);
        }
    }
    public static void ActivateNode(MapNode mapNode){
        if (mapNode.thisNode.thisRoom == MapWorld.RoomType.Combat || mapNode.thisNode.thisRoom == MapWorld.RoomType.Hunt || mapNode.thisNode.thisRoom == MapWorld.RoomType.Hunter){
            // Click on combat like rooms
            EnemyBase ai = mapNode.enemyOnThisNode;
            DataPersistenceManager.DataManager.currentCombatAI = ai;
            SceneManager.LoadSceneAsync("Combat");

        }
        else if (mapNode.thisNode.thisRoom == MapWorld.RoomType.Restsite)
        {
            // Click on rest site node
            if (mapManager.mapDeck.playerHealth < 10) mapManager.mapDeck.playerHealth += 10;
            else mapManager.mapDeck.playerHealth = 20;

            Instantiate(mapManager.restSiteParticles, mapManager.eventCanvas.transform);
            mapManager.mapDeck.UpdateHPText();
            DataPersistenceManager.DataManager.currentCombatAI = null;

        }
        else if (mapNode.thisNode.thisRoom == MapWorld.RoomType.Event)
        {
             // Click on event node
            GameObject eventObject;
            // Reroll the event until it picks different one from last time
            do{
                eventObject = mapManager.thisWorld.events[Random.Range(0, mapManager.thisWorld.events.Count)];
            }while (eventObject.name == DataPersistenceManager.DataManager.lastEvent);

            GameObject eventUI = Instantiate(eventObject, mapManager.eventCanvas);
            eventUI.name = eventObject.name;
            
            mapManager.mapLegend.SetActive(false);

            mapManager.currentEvent = eventUI;

            DataPersistenceManager.DataManager.lastEvent = eventObject.name;
            mapManager.eventUsed = false;

        }

        mapManager.hasTraveled = true;
    }
    public static GameObject GetTopNode(){
        return mapManager.thisWorld.GetGameObjectFromNode(mapManager.thisWorld.floor[mapManager.thisWorld.floor.Length-1].nodes[0]);
    }
    public void MakeBottomLayerSelectable(){
        for (int i = 0; i < thisWorld.floor[0].nodes.Length; i++){
            MakeNodeSelectable(thisWorld.GetGameObjectFromNode(thisWorld.floor[0].nodes[i]));
        }
    }
    static void MakeNodeSelectable(GameObject node){
        node.transform.GetChild(0).gameObject.SetActive(true);
        mapManager.selectableNodes.Add(node);
    }
    public void LoadData(GameData data)
    {
        DataPersistenceManager.DataManager.currentCombatAI = Resources.Load<EnemyBase>("Enemies/" + data.enemyAI);

        thisWorld.mapSeed = data.map.seed;
        thisWorld.generalSeed = Mathf.FloorToInt(Random.value*214783646);
        hasTraveled = data.map.hasTraveled;
        //Generate the world if the seed isn't new
        if(thisWorld.mapSeed != 0){
            GenerateWorld(thisWorld.mapSeed);
            
            if (data.map.hasTraveled)
            {
                SelectNode(thisWorld.GetGameObjectFromNode(thisWorld.floor[data.map.layerIndex].nodes[data.map.nodeIndex]).GetComponent<MapNode>());
            }
                
        }

        if(data.enemyAI != ""){
            SceneManager.LoadSceneAsync("Combat");
        }

    }
    public void SaveData(GameData data)
    {
         if(DataPersistenceManager.DataManager.currentCombatAI != null){
            data.enemyAI = DataPersistenceManager.DataManager.currentCombatAI.ReturnPath();
        }else{
            data.enemyAI = "";
        }

        data.map.seed = thisWorld.mapSeed;
        if(data.map.hasTraveled){
            data.map.layerIndex = currentNodeScript.thisNode.layer.index;
            data.map.nodeIndex = currentNodeScript.thisNode.index;
        }
        data.map.hasTraveled = hasTraveled;
    }
}