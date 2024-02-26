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
    

    public List<GameObject> selectableNodes = new List<GameObject>();
    void Awake(){
        mapManager = this;
        thisWorld = Instantiate(thisWorld);
    }
    
    void Start(){
        GenerateWorld();
    }

    void Update(){
        if (REGENERATE){
            ClearMap();
            REGENERATE = false;
        }
    }

    static void GenerateWorld(){
        //Prevent the map generating a world if the seed isn't new
        if(mapManager.thisWorld.randomSeed != 0){
            return;
        }

        Timer.timer.time = 0;

        // Generate a new map
        mapManager.thisWorld.randomSeed = Mathf.FloorToInt(Random.value*10000000);
        mapManager.thisWorld.GenerateLayout();
        mapManager.thisWorld.GenerateNodes ();
        mapManager.thisWorld.AssignRooms   ();

        mapManager.MakeBottomLayerSelectable();
        Debug.Log("generate " + mapManager.thisWorld.randomSeed);
    }

    static void GenerateWorld(int seed){
        // Generate a map with a given seed
        mapManager.thisWorld.randomSeed = seed;
        mapManager.thisWorld.GenerateLayout();
        mapManager.thisWorld.GenerateNodes ();
        mapManager.thisWorld.AssignRooms   ();

        mapManager.MakeBottomLayerSelectable();
        Debug.Log("generate with seed " + mapManager.thisWorld.randomSeed);
    }

    static void ClearMap(){
        // Destroy all nodes and lines
        for (int i = mapManager.transform.childCount-1; i >= 0; i--){
            Destroy(mapManager.transform.GetChild(i).gameObject);
        }
        mapManager.selectableNodes.Clear();
        GenerateWorld();
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
        
        // Active the node's room
        ActivateNode(mapNode);
    }
    
    public static void SelectNodeNoActivation(MapNode mapNode){
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
            EnemyBase ai = mapNode.enemyOnThisNode;
            DataPersistenceManager.DataManager.currentCombatAI = ai;
            SceneManager.LoadSceneAsync("Combat");
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
        thisWorld.randomSeed = data.map.seed;
        hasTraveled = data.map.hasTraveled;
        //Generate the world if the seed isn't new
        if(thisWorld.randomSeed != 0){
            GenerateWorld(thisWorld.randomSeed);
            
            if (data.map.hasTraveled)
            {
                SelectNodeNoActivation(thisWorld.GetGameObjectFromNode(thisWorld.floor[data.map.layerIndex].nodes[data.map.nodeIndex]).GetComponent<MapNode>());
            }
                
        }

    }

    public void SaveData(GameData data)
    {
        data.map.seed = thisWorld.randomSeed;
        if(data.map.hasTraveled){
            data.map.layerIndex = currentNodeScript.thisNode.layer.index;
            data.map.nodeIndex = currentNodeScript.thisNode.index;
        }
        data.map.hasTraveled = hasTraveled;
    }
}