using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
{
    public static MapManager mapManager;
    public MapWorld thisWorld;
    public MapDeck mapDeck;
    public GameObject currentNode;
    public GameObject currentEvent;
    public bool REGENERATE;
    public bool canTravel;
    public bool canScroll;
    public bool eventUsed;
    

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
        // Generate a new map
        mapManager.thisWorld.randomSeed = Mathf.FloorToInt(Random.value*10000000);
        mapManager.thisWorld.GenerateLayout();
        mapManager.thisWorld.GenerateNodes ();
        mapManager.thisWorld.AssignRooms   ();

        mapManager.MakeBottomLayerSelectable();
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

    public static void ActivateNode(MapNode mapNode){
        if (mapNode.thisNode.thisRoom == MapWorld.RoomType.Combat || mapNode.thisNode.thisRoom == MapWorld.RoomType.Hunt || mapNode.thisNode.thisRoom == MapWorld.RoomType.Hunter){
            EnemyBase ai = mapNode.enemyOnThisNode;
            DataPersistenceManager.DataManager.currentCombatAI = ai;
            SceneManager.LoadSceneAsync("Combat");
        }
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

    public void LoadData(GameData data){
        DataPersistenceManager.DataManager.currentCombatAI = Resources.Load<EnemyBase>("Enemies/" + data.enemyAI);
    }

    public void SaveData(GameData data){
        if(DataPersistenceManager.DataManager.currentCombatAI != null){
            data.enemyAI = DataPersistenceManager.DataManager.currentCombatAI.ReturnPath();
        }else{
            data.enemyAI = "";
        }

        data.mapEvent.name = DataPersistenceManager.DataManager.lastEvent;
        data.mapEvent.used = eventUsed;
    }
}