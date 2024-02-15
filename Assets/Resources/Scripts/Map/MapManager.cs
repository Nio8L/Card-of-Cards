using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour, IDataPersistence
{
    public bool canTravel = true;
    public bool canScroll = true;
    public GameObject nodeObject;
    public GameObject lineObject;
    public GameObject mapLegend;
    public static MapManager mapManager;

    public MapNode currentNode;

    public int numOfLayers = 0;
    public bool isGenerating = true;
    public int[] chanceForRooms;

    static List<Layer> layers = new List<Layer>();
    public List<GameObject>[] allLayers = new List<GameObject>[7];
    static List<LineRenderer> lines = new List<LineRenderer>();
    public List<MapNode> nodesAvaliable = new List<MapNode>();

    List<MapNode> nodesWithoutRoom = new List<MapNode>();

    public Layer lastLayer = null;

    // Enemy Ai list
    static EnemyAI[] tier1EnemyAIs;
    static EnemyBase[] huntEnemyAIs;
    static EnemyAI[] hunterEnemyAIs;

    public MapDeck mapDeck;
    private bool shouldGenerate = true;

    [Header("Tracking")]

    public bool eventUsed = false;

    public Transform eventCanvas;

    public GameObject[] events;
    public GameObject currentEvent;

    [Header("Tutorial")]
    public GameObject tutorialMap;
    public GameObject tutorialPO;

    public GameObject restSiteParticles;

    private void Awake()
    {
        mapManager = this;
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        tier1EnemyAIs = Resources.LoadAll<EnemyAI>("Enemies/Tier1Combat");
        huntEnemyAIs = Resources.LoadAll<EnemyBase>("Enemies/Hunt");
        hunterEnemyAIs = Resources.LoadAll<EnemyAI>("Enemies/Tier1Hunter");
        mapDeck = GameObject.Find("Deck").GetComponent<MapDeck>();


        if(DataPersistenceManager.DataManager.inTutorial){
            GenerateTutorialMap();

            if (DataPersistenceManager.DataManager.tutorialStage == 0)
            {
                canTravel = false;
                canScroll = false;
                Instantiate(tutorialPO, transform.position, Quaternion.identity);
            }
        }else if(shouldGenerate){
            Generate(0, Layer.ConectionType.None);
            Camera.main.GetComponent<MapScroller>().FirstLoadAnimation();

            while (nodesWithoutRoom.Count != 0) 
            {
                MapNode curNode = nodesWithoutRoom[nodesWithoutRoom.Count - 1];
                nodesWithoutRoom.AddRange(GenerateRoom(curNode));
                nodesWithoutRoom.Remove(curNode);
            }
        }

        if (currentNode != null){
            currentNode.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else{
            foreach(Layer.Nodes nodes in layers[0].enterNodes)
            {
                nodesAvaliable.AddRange(nodes.NodesOnConections);
            }
            MakeAvvNodesDifferent();
        }
        
        if(DataPersistenceManager.DataManager.currentCombatAI != null){
            SceneManager.LoadScene("Combat");
        }

        if (currentNode != null)nodesAvaliable = currentNode.children;
        MakeAvvNodesDifferent();
    }

    public void GenerateTutorialMap(){
        GameObject tutorialLayer = Instantiate(tutorialMap);
        layers.Add(tutorialMap.GetComponent<Layer>());
        
        tutorialLayer.transform.SetParent(transform);  
        nodesAvaliable.Add(tutorialLayer.GetComponent<Layer>().mapNodes[DataPersistenceManager.DataManager.tutorialStage]); 
    }

    public void Generate(int curentDepth, Layer.ConectionType typeWanted) 
    {
        GameObject newLayer = null;
        Layer newLayerScript;

        if (typeWanted == Layer.ConectionType.None)
        {
            int randomRoom = 3;
            newLayer = Instantiate(allLayers[randomRoom][Random.Range(0, allLayers[randomRoom].Count)]);
            newLayerScript = newLayer.GetComponent<Layer>();

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < newLayerScript.enterNodes[i].NodesOnConections.Length; j++)
                {
                    nodesWithoutRoom.Add(newLayerScript.enterNodes[i].NodesOnConections[j]);
                    nodesAvaliable.Add(newLayerScript.enterNodes[i].NodesOnConections[j]);
                }
            }
        }
        else
        {
            newLayer = Instantiate(allLayers[(int)typeWanted][Random.Range(0, allLayers[(int)typeWanted].Count)]);
            newLayerScript = newLayer.GetComponent<Layer>();
        }


        if (typeWanted != Layer.ConectionType.None)
        {
            ConectLayers(newLayerScript, lastLayer);
        }

        lastLayer = newLayerScript;
        layers.Add(newLayerScript);
        newLayer.transform.SetParent(transform);
        if (curentDepth != numOfLayers) Generate(curentDepth + 1, newLayerScript.exitConectionType);
        else AddBossRoom();
    }

    public void Generate(List<LayerClass> layersToRecreate) 
    {
        lastLayer = null;
        for (int i = 0; i < layersToRecreate.Count; i++)
        {
            Layer newLayer = Instantiate(allLayers[layersToRecreate[i].enterConectionType][layersToRecreate[i].placeInTheArray]).GetComponent<Layer>();

            for(int j = 0; j < newLayer.mapNodes.Count; j++){
                newLayer.mapNodes[j].roomType = layersToRecreate[i].mapNodeClasses[j].roomType;
                newLayer.mapNodes[j].used = layersToRecreate[i].mapNodeClasses[j].used;
                newLayer.mapNodes[j].isCurrentNode = layersToRecreate[i].mapNodeClasses[j].isCurrentNode;

                if(newLayer.mapNodes[j].isCurrentNode){
                    currentNode = newLayer.mapNodes[j];
                    ReverseMakeAvvNodesDifferent();
                    currentNode.gameObject.GetComponent<SpriteRenderer>().color = Color.red;
                }
                PutSprite(newLayer.mapNodes[j]);
            }

            if (lastLayer != null)
            {
                ConectLayers(newLayer, lastLayer);
            }

            lastLayer = newLayer;
            layers.Add(newLayer);
            newLayer.transform.SetParent(transform);
        }
        AddBossRoom();
    }

    void ConectLayers(Layer newLayerScript, Layer oldLayer) 
    {
        newLayerScript.transform.position = oldLayer.transform.position + Vector3.up * 6;

        for (int i = 0; i < 3; i++)
        {
            if (newLayerScript.enterNodes[i].NodesOnConections == null) continue;

            //getting a conection point left/middle/right
            for (int j = 0; j < newLayerScript.enterNodes[i].NodesOnConections.Length; j++)
            {
                newLayerScript.enterNodes[i].NodesOnConections[j].parents.AddRange(oldLayer.exitNodes[i].NodesOnConections);//adding old nodes to the parent

                for (int l = 0; l < oldLayer.exitNodes[i].NodesOnConections.Length; l++) AddLine(newLayerScript.enterNodes[i].NodesOnConections[j], newLayerScript.enterNodes[i].NodesOnConections[j].transform, oldLayer.exitNodes[i].NodesOnConections[l].transform);
            }

            for (int j = 0; j < oldLayer.exitNodes[i].NodesOnConections.Length; j++)
            {
                oldLayer.exitNodes[i].NodesOnConections[j].children.AddRange(newLayerScript.enterNodes[i].NodesOnConections);//adding old points to the children
            }
        }
    }

    void AddBossRoom() 
    {
        MapNode bossNode = Instantiate(nodeObject).GetComponent<MapNode>();
        if (lastLayer == null) return;
        bossNode.transform.position = new Vector3(0, lastLayer.transform.position.y + 6, 0);

        bossNode.parents.AddRange(lastLayer.GetAllExitNodes());
        bossNode.transform.SetParent(transform);

        bossNode.roomType = MapNode.RoomType.Hunter;
        PutSprite(bossNode);

        bossNode.transform.localScale *= 2;

        for (int i = 0; i < bossNode.parents.Count; i++)
        {
            bossNode.parents[i].children.Add(bossNode);
            AddLine(bossNode, bossNode.transform, bossNode.parents[i].transform);
            
            //Make all rooms before the Hunter be Rest sites
            bossNode.parents[i].roomType = MapNode.RoomType.RestSite;
            PutSprite(bossNode.parents[i]);
        }
    }

    void AddLine(MapNode parenNode, Transform pos1, Transform pos2)
    {
        GameObject newLine = Instantiate(lineObject, transform.position, Quaternion.identity);
        LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();

        newLine.transform.SetParent(parenNode.transform);

        lineRenderer.SetPosition(0, pos1.position);
        lineRenderer.SetPosition(1, pos2.position);
        parenNode.lines.Add(lineRenderer);
        lines.Add(lineRenderer);
    }

    public static void NodeClicked(MapNode node) 
    {
        if(!mapManager.canTravel) return;

        if (mapManager.nodesAvaliable.Contains(node))
        {
            if (mapManager.currentNode != null)
            {
                mapManager.currentNode.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                mapManager.currentNode.isCurrentNode = false;
            }

            mapManager.currentNode = node;
            node.isCurrentNode = true;
            ReverseMakeAvvNodesDifferent();
            mapManager.nodesAvaliable = node.children;
            MakeAvvNodesDifferent();
            mapManager.currentNode.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

            if (mapManager.currentNode.roomType == MapNode.RoomType.Combat)
            {
                // Click on combat node
                EnemyAI ai = tier1EnemyAIs[Mathf.FloorToInt(Random.value * tier1EnemyAIs.Length)];
                DataPersistenceManager.DataManager.currentCombatAI = ai;
                SceneManager.LoadSceneAsync("Combat");
            }
            else if (mapManager.currentNode.roomType == MapNode.RoomType.RestSite)
            {
                // Click on rest site node
                if (mapManager.mapDeck.playerHealth < 10) mapManager.mapDeck.playerHealth += 10;
                else mapManager.mapDeck.playerHealth = 20;

                Instantiate(mapManager.restSiteParticles, mapManager.eventCanvas.transform);
                mapManager.mapDeck.UpdateHPText();
                DataPersistenceManager.DataManager.currentCombatAI = null;
            }
            else if (mapManager.currentNode.roomType == MapNode.RoomType.Hunt)
            {
                // Click on hunt node
                retry:;
                EnemyBase ai = huntEnemyAIs[Mathf.FloorToInt(Random.value * huntEnemyAIs.Length)];
                if(ai == DataPersistenceManager.DataManager.lastEnemyAI){
                    goto retry;
                }
                DataPersistenceManager.DataManager.currentCombatAI = ai;
                DataPersistenceManager.DataManager.lastEnemyAI = ai;
                SceneManager.LoadSceneAsync("Combat");
            }
            else if (mapManager.currentNode.roomType == MapNode.RoomType.Hunter)
            {
                // Click on hunter node
                EnemyAI ai = hunterEnemyAIs[Mathf.FloorToInt(Random.value * hunterEnemyAIs.Length)];
                DataPersistenceManager.DataManager.currentCombatAI = ai;
                SceneManager.LoadSceneAsync("Combat");
            }
            else if (mapManager.currentNode.roomType == MapNode.RoomType.Event)
            {
                // Click on event node
                GameObject eventObject;
                // Reroll the event until it picks different one from last time
                do{
                    eventObject = mapManager.events[Random.Range(0, mapManager.events.Length)];
                }while (eventObject.name == DataPersistenceManager.DataManager.lastEvent);

                GameObject eventUI = Instantiate(eventObject, mapManager.eventCanvas);
                eventUI.name = eventObject.name;
                
                mapManager.mapLegend.SetActive(false);

                mapManager.currentEvent = eventUI;

                DataPersistenceManager.DataManager.lastEvent = eventObject.name;
                mapManager.eventUsed = false;
                
            }
            //new curent node
        }
    }

    List<MapNode> GenerateRoom(MapNode node) 
    {
        //Debug.Log(node.roomType.ToString());
        if(node.roomType != MapNode.RoomType.emptyRoom)return node.children;

        MapNode.RoomType room = MapNode.RoomType.emptyRoom;
        bool retryed = false;

    retry:;
        int randomValue = Random.Range(0, 101);
        for (int i = 0; i < 4; i++)
        {
            if (randomValue <= chanceForRooms[i]) 
            {
                room = (MapNode.RoomType)i;
                break;
            }
            randomValue -= chanceForRooms[i];
        }

        if (room == node.roomType && Random.Range(0, 2) == 0 && !retryed) 
        {
            retryed = true;
            goto retry;
        }

        node.roomType = room;


        //Make all starting nodes a hunt
        if(layers[0].mapNodes.Contains(node)){
            node.roomType = MapNode.RoomType.Hunt;
        }

        foreach (MapNode parentNode in node.parents)
        {
            if(node.roomType == MapNode.RoomType.Combat && parentNode.roomType == MapNode.RoomType.Combat){
                goto retry;
            }

            if(node.roomType == MapNode.RoomType.Event && parentNode.roomType == MapNode.RoomType.Event)
            {
                goto retry;
            }
        }

        PutSprite(node);
        return node.children;
    }

    public void PutSprite(MapNode node) 
    {
        SpriteRenderer spriteRenderer = node.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/RoomSprites/" + node.roomType.ToString());
    }

    public static void MakeAvvNodesDifferent() 
    {
        for (int i = 0; i < mapManager.nodesAvaliable.Count; i++)
        {
            if (mapManager.nodesAvaliable[i] != null)
            {
                mapManager.nodesAvaliable[i].indicator.SetActive(true);
            }
        }
    }

    public static void ReverseMakeAvvNodesDifferent()
    {
        for (int i = 0; i < mapManager.nodesAvaliable.Count; i++)
        {
            if (mapManager.nodesAvaliable[i] != null)
            {
                mapManager.nodesAvaliable[i].indicator.SetActive(false);
            }
        }
    }

    public void LoadData(GameData data)
    {
        for (int i = 0; i < allLayers.Length; i++) allLayers[i] = new List<GameObject>();

        GameObject[] loadedLayers = Resources.LoadAll<GameObject>("Prefabs/Map/Layers");
        for (int i = 0; i < loadedLayers.Length; i++)
        {
            Layer newLayer = loadedLayers[i].GetComponent<Layer>();
            allLayers[(int)newLayer.enterConectionType].Add(loadedLayers[i]);
            newLayer.placeInTheArray = allLayers[(int)newLayer.enterConectionType].Count - 1;
        }

        layers.Clear();

        if(data.mapLayers.Count > 0){
            shouldGenerate = false;
            Generate(data.mapLayers);  
        }
        DataPersistenceManager.DataManager.currentCombatAI = Resources.Load<EnemyBase>("Enemies/" + data.enemyAI);

        if (currentNode != null)
        {
            if(currentNode.roomType == MapNode.RoomType.Event){
                for(int i = 0; i < events.Length; i++){
                    if(events[i].name == data.mapEvent.name && !data.mapEvent.used){
                        GameObject eventUI = Instantiate(events[i], eventCanvas);
                        eventUI.name = events[i].name;
                        
                        mapManager.mapLegend.SetActive(false);
    
                        mapManager.currentEvent = eventUI;
    
                        DataPersistenceManager.DataManager.lastEvent = eventUI.name;
                    }
                }
            }
        }
    }

    public void SaveData(GameData data)
    {
        data.mapLayers = new();

        foreach (Layer layer in layers)
        {
            LayerClass layerToSave = new();
            
            foreach (MapNode mapNode in layer.mapNodes)
            {
                MapNodeClass mapNodeToSave = new()
                {
                    roomType = mapNode.roomType,
                    used = mapNode.used,
                    isCurrentNode = mapNode.isCurrentNode 
                };
                layerToSave.mapNodeClasses.Add(mapNodeToSave);
            }

            layerToSave.placeInTheArray = layer.placeInTheArray;

            layerToSave.enterConectionType = (int)layer.enterConectionType;

            data.mapLayers.Add(layerToSave);
        }

        if(DataPersistenceManager.DataManager.currentCombatAI != null){
            data.enemyAI = DataPersistenceManager.DataManager.currentCombatAI.ReturnPath();
        }else{
            data.enemyAI = "";
        }

        data.mapEvent.name = DataPersistenceManager.DataManager.lastEvent;
        data.mapEvent.used = eventUsed;
    }
}
