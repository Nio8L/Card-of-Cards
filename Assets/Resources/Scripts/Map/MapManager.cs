using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour, IDataPersistence
{
    public static MapManager mapManager;
    public MapWorld thisWorld;
    public MapDeck mapDeck;
    
    [Header("Developer Tools")]
    public bool REGENERATE;
    
    [Header("Tracking")]
    public GameObject currentNode;
    public MapNode currentNodeScript;
    public GameObject currentEvent;
    public bool canTravel;
    public bool canScroll;
    public bool eventUsed;
    public bool hasTraveled = false;

    [Header("Objects")]
    public Transform eventCanvas;
    public GameObject mapLegend;
    public GameObject playerHP;
    
    public SpriteRenderer background;
    // Particles and other
    public GameObject restSiteParticles;
    public GameObject mapTutorialPO;
    

    public List<GameObject> selectableNodes = new List<GameObject>();
    void Awake(){
        try{
            mapManager = this;
            
            thisWorld = ScenePersistenceManager.scenePersistence.stages[ScenePersistenceManager.scenePersistence.currentStage];
            thisWorld = Instantiate(thisWorld);
        }catch (Exception){
            SceneManager.LoadSceneAsync("Main Menu");
            return;
        }
    }
    void Start(){
        if (ScenePersistenceManager.scenePersistence.resetMap){
            ScenePersistenceManager.scenePersistence.resetMap = false;
            ClearMap();
            LoadWorld(ScenePersistenceManager.scenePersistence.stages[ScenePersistenceManager.scenePersistence.currentStage]);
        }else{
            GenerateWorld();
        }

        if (ScenePersistenceManager.scenePersistence.inTutorial){
            if (ScenePersistenceManager.scenePersistence.tutorialStage != 0){
                SelectNode(thisWorld.GetGameObjectFromNode(thisWorld.floor[ScenePersistenceManager.scenePersistence.tutorialStage-1].nodes[0]).GetComponent<MapNode>());
            }else{
                Instantiate(mapTutorialPO);
                canTravel = false;
                canScroll = false;
            }
        }else if (!hasTraveled){
            Camera.main.GetComponent<MapScroller>().FirstLoadAnimation();
        }


    }
    void Update(){
        if (REGENERATE){
            FullClear();
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
        GenerateWorld(Mathf.FloorToInt(UnityEngine.Random.value*214783646));
    }
    static void GenerateWorld(int seed){
        // Generate a map with a given seed
        mapManager.thisWorld.mapSeed = seed;
        mapManager.thisWorld.GenerateLayout  ();
        mapManager.thisWorld.GenerateNodes   ();
        mapManager.thisWorld.AssignRooms     ();
        mapManager.thisWorld.AssignEncounters();
        

        mapManager.MakeBottomLayerSelectable ();

        mapManager.background.color = mapManager.thisWorld.backgroundColor;
        // Start using the general seed
        UnityEngine.Random.InitState(mapManager.thisWorld.generalSeed);
    }
    static void ClearMap(){
        // Destroy all nodes and lines
        for (int i = mapManager.transform.childCount-1; i >= 0; i--){
            Destroy(mapManager.transform.GetChild(i).gameObject);
        }
        mapManager.selectableNodes.Clear();
        mapManager.hasTraveled = false;
        mapManager.currentNode = null;
        mapManager.currentNodeScript = null;
    }
    static void FullClear(){
        ClearMap();
        mapManager.thisWorld.generalSeed = Mathf.FloorToInt(UnityEngine.Random.value*214783646);
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
            ScenePersistenceManager.scenePersistence.currentCombatAI = ai;
            SceneManager.LoadSceneAsync("Combat");

        }
        else if (mapNode.thisNode.thisRoom == MapWorld.RoomType.Restsite)
        {
            // Click on rest site node
            if (mapManager.mapDeck.playerHealth < 10) mapManager.mapDeck.playerHealth += 10;
            else mapManager.mapDeck.playerHealth = 20;

            Instantiate(mapManager.restSiteParticles, mapManager.eventCanvas.transform);
            mapManager.mapDeck.UpdateHPText();
            ScenePersistenceManager.scenePersistence.currentCombatAI = null;

            SoundManager.soundManager.Play("LostSoul");

        }
        else if (mapNode.thisNode.thisRoom == MapWorld.RoomType.Event)
        {
             // Click on event node
            GameObject eventObject = mapNode.eventOnThisNode;

            GameObject eventUI = Instantiate(eventObject, mapManager.eventCanvas);
            eventUI.name = eventObject.name;
            
            mapManager.mapLegend.SetActive(false);
            mapManager.playerHP.SetActive(false);            

            mapManager.currentEvent = eventUI;

            ScenePersistenceManager.scenePersistence.lastEvent = eventObject.name;
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
        ScenePersistenceManager.scenePersistence.currentCombatAI = Resources.Load<EnemyBase>("Enemies/" + data.enemyAI);

        thisWorld.mapSeed = data.map.seed;
        thisWorld.generalSeed = Mathf.FloorToInt(UnityEngine.Random.value*214783646);
        if (!ScenePersistenceManager.scenePersistence.resetMap){
            hasTraveled = data.map.hasTraveled;
        }
        //Generate the world if the seed isn't new
        if(thisWorld.mapSeed != 0){
            GenerateWorld(thisWorld.mapSeed);
            
            if (hasTraveled)
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
         if(ScenePersistenceManager.scenePersistence.currentCombatAI != null){
            data.enemyAI = ScenePersistenceManager.scenePersistence.currentCombatAI.ReturnPath();
        }else{
            data.enemyAI = "";
        }

        data.map.seed = thisWorld.mapSeed;
        data.map.hasTraveled = hasTraveled;
        if(hasTraveled){
            data.map.layerIndex = currentNodeScript.thisNode.layerIndex;
            data.map.nodeIndex = currentNodeScript.thisNode.index;
        }
    }

    public static void LoadWorld(MapWorld newWorld){
        ClearMap();

        newWorld.mapSeed     = mapManager.thisWorld.mapSeed;
        newWorld.generalSeed = mapManager.thisWorld.generalSeed;

        mapManager.thisWorld = newWorld;

        GenerateWorld(newWorld.mapSeed);
    }
}