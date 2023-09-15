using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour
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
    static List<MapNode> nodesAvaliable = new List<MapNode>();

    List<MapNode> nodesWithoutRoom = new List<MapNode>();

    Layer lastLayer = null;

    // Enemy Ai list
    static EnemyAI[] tier1EnemyAIs;
    static EnemyAI[] huntEnemyAIs;
    static EnemyAI[] hunterEnemyAIs;

    static MapDeck mapDeck;

    public static DeckDisplay deckDisplay;

    private void Awake()
    {
        mapManager = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < allLayers.Length; i++) allLayers[i] = new List<GameObject>();

        GameObject[] loadedLayers = Resources.LoadAll<GameObject>("Prefabs/Map/Layers");
        for (int i = 0; i < loadedLayers.Length; i++)
        {
            Layer newLayer = loadedLayers[i].GetComponent<Layer>();
            allLayers[(int)newLayer.enterConectionType].Add(loadedLayers[i]);
            newLayer.placeInTheArray = allLayers[(int)newLayer.enterConectionType].Count - 1;
        }

        tier1EnemyAIs = Resources.LoadAll<EnemyAI>("Enemies/Tier1Combat");
        huntEnemyAIs = Resources.LoadAll<EnemyAI>("Enemies/Hunt");
        hunterEnemyAIs = Resources.LoadAll<EnemyAI>("Enemies/Tier1Hunter");
        mapDeck = GameObject.Find("Deck").GetComponent<MapDeck>();

        if (currentNode != null){
            currentNode.GetComponent<SpriteRenderer>().color = Color.red;
        }

        Invoke("AssignId", 1);

        Generate(0, Layer.ConectionType.None);

        while (nodesWithoutRoom.Count != 0) 
        {
            MapNode curNode = nodesWithoutRoom[nodesWithoutRoom.Count - 1];
            nodesWithoutRoom.AddRange(GenerateRoom(curNode));
            nodesWithoutRoom.Remove(curNode);
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

    public void Generate(List<Layer> layersToRecreate) 
    {
        lastLayer = null;
        for (int i = 0; i < layersToRecreate.Count; i++)
        {
            Layer newLayer = Instantiate(allLayers[(int)layersToRecreate[i].enterConectionType][layersToRecreate[i].placeInTheArray]).GetComponent<Layer>();

            if (lastLayer != null)
            {
                ConectLayers(newLayer, lastLayer);
            }

            lastLayer = newLayer;
            layers.Add(newLayer);
            newLayer.transform.SetParent(transform);
        }
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
        bossNode.transform.position = new Vector3(0, lastLayer.transform.position.y + 6, 0);

        bossNode.parents.AddRange(lastLayer.GetAllExitNodes());

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
        if (nodesAvaliable.Contains(node))
        {
            if(currentNode != null)currentNode.gameObject.GetComponent<SpriteRenderer>().color = Color.white;
            currentNode = node;
            nodesAvaliable = node.children;
            currentNode.gameObject.GetComponent<SpriteRenderer>().color = Color.red;

            if (currentNode.roomType == MapNode.RoomType.Combat)
            {
                EnemyAI ai = tier1EnemyAIs[Mathf.FloorToInt(UnityEngine.Random.value * tier1EnemyAIs.Length)];
                DataPersistenceManager.DataManager.currentCombatAI = ai;
                SceneManager.LoadSceneAsync("SampleScene");
            }
            else if (currentNode.roomType == MapNode.RoomType.RestSite)
            {
                if (mapDeck.playerHp < 15)
                {
                    mapDeck.playerHp += 5;
                }
                else
                {
                    mapDeck.playerHp = 20;
                }
                mapDeck.playerHpText.text = "HP: " + mapDeck.playerHp;
            }
            else if (currentNode.roomType == MapNode.RoomType.Graveyard)
            {
                if (!deckDisplay.deckDisplay.activeSelf)
                {
                    deckDisplay.ShowDeck();
                }
                deckDisplay.canClose = false;
            }
            else if (currentNode.roomType == MapNode.RoomType.Hunt)
            {
                EnemyAI ai = huntEnemyAIs[Mathf.FloorToInt(UnityEngine.Random.value * huntEnemyAIs.Length)];
                DataPersistenceManager.DataManager.currentCombatAI = ai;
                SceneManager.LoadSceneAsync("SampleScene");
            }
            else if (currentNode.roomType == MapNode.RoomType.Hunter)
            {
                EnemyAI ai = hunterEnemyAIs[Mathf.FloorToInt(UnityEngine.Random.value * hunterEnemyAIs.Length)];
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
}
