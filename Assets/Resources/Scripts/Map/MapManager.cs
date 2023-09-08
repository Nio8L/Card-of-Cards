using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapManager : MonoBehaviour, IDataPersistence
{
    public GameObject nodeObject;
    public GameObject lineObject;
    
    public static MapManager mapManager;

    public List<MapNode> mapNodes;
    public List<LineRenderer> lines;
    //public List<Transform> mapNodeCoords;

    public MapNode currentNode;

    public int depth = 0;

    public int nodeNumber = 0;

    public bool isGenerating = true;

    // Enemy Ai list
    EnemyAI[] tier1EnemyAIs;
    EnemyAI[] huntEnemyAIs;

    MapDeck mapDeck;

    private void Awake() {
        mapManager = this;
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        tier1EnemyAIs = Resources.LoadAll<EnemyAI>("Enemies/Tier1Combat");
        huntEnemyAIs = Resources.LoadAll<EnemyAI>("Enemies/Hunt");
        mapDeck = GameObject.Find("Deck").GetComponent<MapDeck>();

        if (currentNode != null){
            currentNode.GetComponent<SpriteRenderer>().color = Color.red;
        }

        Invoke("AssignId", 1);
    }

    // Update is called once per frame
    void Update()
    {
        if(mapNodes.Count > 0 && !mapNodes[^1].canGenerate){
            CutNodes();
        }
    }

    public void AssignId(){
        for(int i = 0; i < mapNodes.Count; i++){
            mapNodes[i].nodeId = i;
        }
    }

    public void UpdateCurrentNode(MapNode newNode){
        if(newNode.nodeDepth > currentNode.nodeDepth && currentNode.connections.Contains(newNode) && newNode != currentNode){
        
            currentNode.spriteRenderer.color = Color.white;
            currentNode = newNode;
            currentNode.spriteRenderer.color = Color.red;

            if (currentNode.roomType == MapNode.RoomType.Combat)
            {
                EnemyAI ai = tier1EnemyAIs[Mathf.FloorToInt(UnityEngine.Random.value * tier1EnemyAIs.Length)];
                DataPersistenceManager.DataManager.currentCombatAI = ai;
                SceneManager.LoadSceneAsync("SampleScene");
            }
            else if(currentNode.roomType == MapNode.RoomType.RestSite)
            {
                if(mapDeck.playerHp < 15){
                    mapDeck.playerHp += 5;
                }else{
                    mapDeck.playerHp = 20;
                }
                mapDeck.playerHpText.text = "HP: " + mapDeck.playerHp;
            }
            else if (currentNode.roomType == MapNode.RoomType.Hunt)
            {
                EnemyAI ai = huntEnemyAIs[Mathf.FloorToInt(UnityEngine.Random.value * huntEnemyAIs.Length)];
                DataPersistenceManager.DataManager.currentCombatAI = ai;
                SceneManager.LoadSceneAsync("SampleScene");
            }
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

    public void CutNodes(){
        //Debug.Log("cutting nodes with depth of " + depth);
        for(int i = mapNodes.Count - 1; i > 0; i--){
            if(mapNodes[i].lines.Count == 1 && mapNodes[i].nodeDepth < depth){  
                for(int j = mapNodes[i].connections.Count - 1; j > -1; j--){
                    mapNodes[i].connections[j].connections.Remove(mapNodes[i]);
                }
                mapNodes[i].parentNode.lines.Remove(mapNodes[i].lines[0]);
                Destroy(mapNodes[i].lines[0].gameObject);

                Destroy(mapNodes[i].gameObject);
                mapNodes.Remove(mapNodes[i]);
            }
        }
    }

    public void LoadData(GameData data)
    {
        
        mapNodes.Clear();
        if(data.map.mapNodes.Count > 0){
            currentNode.gameObject.SetActive(false);
            currentNode = null;
            isGenerating = false;
           
            for(int i = 0; i < data.map.mapNodes.Count; i++){
                GameObject newNode = Instantiate(nodeObject, new Vector3(data.map.mapNodes[i].coordinates[0], data.map.mapNodes[i].coordinates[1], data.map.mapNodes[i].coordinates[2]), Quaternion.identity);
                MapNode newMapNode = newNode.GetComponent<MapNode>();
                newMapNode.canGenerate = false;
                newMapNode.roomType = (MapNode.RoomType) Enum.Parse(typeof(MapNode.RoomType), data.map.mapNodes[i].roomType);
                newMapNode.nodeId = data.map.mapNodes[i].id;
                newMapNode.nodeDepth = data.map.mapNodes[i].nodeDepth;
                newMapNode.used = data.map.mapNodes[i].used;
    
                mapNodes.Add(newMapNode);
            }
    
            currentNode = mapNodes[data.map.currentNodeId];

            for(int i = 0; i < data.map.mapNodes.Count; i++){
                for(int j = 0; j < data.map.mapNodes[i].connectionIds.Count; j++){
                    for(int k = 0; k < mapNodes.Count; k++){
                        if(mapNodes[k].nodeId == data.map.mapNodes[i].connectionIds[j]){
                            mapNodes[i].connections.Add(mapNodes[k]);

                            GameObject newLine = Instantiate(lineObject, mapNodes[i].transform.position, Quaternion.identity);
                            LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();
                            newLine.transform.SetParent(mapNodes[i].gameObject.transform);
                            lineRenderer.SetPosition(0, mapNodes[i].transform.position);
                            lineRenderer.SetPosition(1, mapNodes[k].transform.position);
                            mapNodes[i].lines.Add(lineRenderer);
                            mapNodes[k].lines.Add(lineRenderer);
                            lines.Add(lineRenderer);
                        }
                    }
                }
            }
        }else{
            currentNode.gameObject.SetActive(true);
            isGenerating = true;
        }
    }

    public void SaveData(ref GameData data)
    {        
        data.map.mapNodes.Clear();
        
        data.map.currentNodeId = currentNode.nodeId;

        for(int i = 0; i < mapNodes.Count; i++){
            data.map.mapNodes.Add(new MapNodeClass());
            data.map.mapNodes[^1].coordinates.Add(mapNodes[i].transform.position.x);
            data.map.mapNodes[^1].coordinates.Add(mapNodes[i].transform.position.y);
            data.map.mapNodes[^1].coordinates.Add(mapNodes[i].transform.position.z);

            data.map.mapNodes[^1].roomType = mapNodes[i].roomType.ToString();

            data.map.mapNodes[^1].id = mapNodes[i].nodeId;

            data.map.mapNodes[^1].nodeDepth = mapNodes[i].nodeDepth;

            data.map.mapNodes[^1].used = mapNodes[i].used;

            for(int j = 0; j < mapNodes[i].connections.Count; j++){
                data.map.mapNodes[^1].connectionIds.Add(mapNodes[i].connections[j].nodeId);
            }
        }
    }
}
