using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour, IDataPersistence
{
    public GameObject nodeObject;
    public GameObject lineObject;
    public static MapManager mapManager;

    public static MapNode currentNode;

    public int numOfLayers = 0;
    public bool isGenerating = true;
    public int[] chaceForRooms;

    static List<Layer> layers = new List<Layer>();
    static List<GameObject>[] allLayers = new List<GameObject>[7];
    static List<LineRenderer> lines = new List<LineRenderer>();
    public List<MapNode> nodesAvaliable = new List<MapNode>();

    List<MapNode> nodesWithoutRoom = new List<MapNode>();

    public Layer lastLayer = null;

    // Enemy Ai list
    static EnemyAI[] tier1EnemyAIs;
    static EnemyAI[] huntEnemyAIs;
    static EnemyAI[] hunterEnemyAIs;

    static MapDeck mapDeck;

    public static DeckDisplay deckDisplay;

    private bool shouldGenerate = true;

    private void Awake()
    {
        mapManager = this;
        //DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        deckDisplay = GameObject.Find("DeckDisplayManager").GetComponent<DeckDisplay>();

        tier1EnemyAIs = Resources.LoadAll<EnemyAI>("Enemies/Tier1Combat");
        huntEnemyAIs = Resources.LoadAll<EnemyAI>("Enemies/Hunt");
        hunterEnemyAIs = Resources.LoadAll<EnemyAI>("Enemies/Tier1Hunter");
        mapDeck = GameObject.Find("Deck").GetComponent<MapDeck>();


        if(shouldGenerate){
            Generate(0, Layer.ConectionType.None);
        
            while (nodesWithoutRoom.Count != 0) 
            {
                MapNode curNode = nodesWithoutRoom[nodesWithoutRoom.Count - 1];
                nodesWithoutRoom.AddRange(GenerateRoom(curNode));
                nodesWithoutRoom.Remove(curNode);
            }

            MakeAvvNodesDifferent();
        }

        if (currentNode != null){
            currentNode.GetComponent<SpriteRenderer>().color = Color.red;
        }else{
            foreach(Layer.Nodes nodes in layers[0].enterNodes)
            {
                nodesAvaliable.AddRange(nodes.NodesOnConections);
            }
            MakeAvvNodesDifferent();
        }

        if(DataPersistenceManager.DataManager.currentCombatAI != null){
            SceneManager.LoadScene("SampleScene");
        }
    }

    public void Generate(int curentDepth, Layer.ConectionType typeWanted) 
    {
        GameObject newLayer = null;
        Layer newLayerScript;

        if (typeWanted == Layer.ConectionType.None)
        {
            int randomRoom = 3;
            newLayer = Instantiate(allLayers[randomRoom][Random.Range(0, allLayers[randomRoom].Count)]);
            newLayerScript = newLayerScript = newLayer.GetComponent<Layer>();

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
            newLayerScript = newLayerScript = newLayer.GetComponent<Layer>();
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
                    nodesAvaliable = currentNode.children;
                    MakeAvvNodesDifferent();
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
        if (mapManager.nodesAvaliable.Contains(node) && deckDisplay.canClose)
        {
            if (currentNode != null)
            {
                currentNode.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
                currentNode.isCurrentNode = false;
            }
            currentNode = node;
            node.isCurrentNode = true;
            ReverseMakeAvvNodesDifferent();
            mapManager.nodesAvaliable = node.children;
            MakeAvvNodesDifferent();
            currentNode.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

            if (currentNode.roomType == MapNode.RoomType.Combat)
            {
                EnemyAI ai = tier1EnemyAIs[Mathf.FloorToInt(Random.value * tier1EnemyAIs.Length)];
                DataPersistenceManager.DataManager.currentCombatAI = ai;
                SceneManager.LoadSceneAsync("SampleScene");
            }
            else if (currentNode.roomType == MapNode.RoomType.RestSite)
            {
                if (mapDeck.playerHealth < 15)
                {
                    mapDeck.playerHealth += 5;
                }
                else
                {
                    mapDeck.playerHealth = 20;
                }
                mapDeck.UpdateHPText();
                DataPersistenceManager.DataManager.currentCombatAI = null;
            }
            else if (currentNode.roomType == MapNode.RoomType.Graveyard)
            {
                if (!deckDisplay.deckDisplay.activeSelf)
                {
                    deckDisplay.ShowDeck();
                }
                deckDisplay.canClose = false;
                DataPersistenceManager.DataManager.currentCombatAI = null;
            }
            else if (currentNode.roomType == MapNode.RoomType.Hunt)
            {
                EnemyAI ai = huntEnemyAIs[Mathf.FloorToInt(Random.value * huntEnemyAIs.Length)];
                DataPersistenceManager.DataManager.currentCombatAI = ai;
                SceneManager.LoadSceneAsync("SampleScene");
            }
            else if (currentNode.roomType == MapNode.RoomType.Hunter)
            {
                EnemyAI ai = hunterEnemyAIs[Mathf.FloorToInt(Random.value * hunterEnemyAIs.Length)];
                DataPersistenceManager.DataManager.currentCombatAI = ai;
                SceneManager.LoadSceneAsync("SampleScene");
            }
            //new curent node
        }
    }

    List<MapNode> GenerateRoom(MapNode node) 
    {
        if(node.roomType != MapNode.RoomType.emptyRoom)return node.children;

        int randomValue = Random.Range(0, 101);
        MapNode.RoomType room = MapNode.RoomType.emptyRoom;
        bool retryed = false;

    retry:;

        for (int i = 0; i < 4; i++)
        {
            if (randomValue <= chaceForRooms[i]) 
            {
                room = (MapNode.RoomType)i;
                break;
            }
            randomValue -= chaceForRooms[i];
        }

        if (room == node.roomType && Random.Range(0, 2) == 0 && !retryed) 
        {
            retryed = true;
            goto retry;
        }

        node.roomType = room;

        PutSprite(node);
        return node.children;
    }

    void PutSprite(MapNode node) 
    {
        SpriteRenderer spriteRenderer = node.gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/RoomSprites/" + node.roomType.ToString());
    }

    public static void MakeAvvNodesDifferent() 
    {
        for (int i = 0; i < mapManager.nodesAvaliable.Count; i++)
        {
            mapManager.nodesAvaliable[i].indicator.SetActive(true);
        }
    }

    public static void ReverseMakeAvvNodesDifferent()
    {
        for (int i = 0; i < mapManager.nodesAvaliable.Count; i++)
        {
            mapManager.nodesAvaliable[i].indicator.SetActive(false);
        }
    }

    public bool LinesInterescting(LineRenderer lineRenderer1, LineRenderer lineRenderer2){
        bool isIntersecting = false;

        Vector2 L1_start = lineRenderer1.GetPosition(0);
        Vector2 L1_end = lineRenderer1.GetPosition(1);

        Vector2 L2_start = lineRenderer2.GetPosition(0);
        Vector2 L2_end = lineRenderer2.GetPosition(1);

        Vector2 p1 = new(L1_start.x, L1_start.y);
        Vector2 p2 = new(L1_end.x, L1_end.y);

        Vector2 p3 = new(L2_start.x, L2_start.y);
        Vector2 p4 = new(L2_end.x, L2_end.y);

        float denominator = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
	
        if (denominator != 0)
	    {
	    	float u_a = ((p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x)) / denominator;
	    	float u_b = ((p2.x - p1.x) * (p1.y - p3.y) - (p2.y - p1.y) * (p1.x - p3.x)) / denominator;

	    	//Is intersecting if u_a and u_b are between 0 and 1
	    	if (u_a >= 0 && u_a <= 1 && u_b >= 0 && u_b <= 1)
	    	{
		    	isIntersecting = true;
	    	}
        }

        return isIntersecting;
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
        Debug.Log("loading from map: " + data.enemyAI);
        DataPersistenceManager.DataManager.currentCombatAI = Resources.Load<EnemyBase>("Enemies/" + data.enemyAI);
    }

    public void SaveData(ref GameData data)
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
    }
}
