using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapNode : MonoBehaviour, IPointerDownHandler
{
    public enum RoomType{
        Combat,
        Hunt,
        RestSite,
        Graveyard,
        Hunter,
        emptyRoom
    };

    public MapNode parentNode;

    public RoomType roomType;

    public List<MapNode> connections;
    public List<LineRenderer> lines;

    public GameObject nodeObject;
    public GameObject lineObject;

    public bool canGenerate = true;

    public SpriteRenderer spriteRenderer;

    public int nodeDepth = 0;

    public int nodeId;

    public bool used = false;

    bool[] directionsLeft = new bool[3];//left center right
    public int RoomsInARow = 0;

    [Header("Generation settings")]
    [SerializeField] public int depth;
    [SerializeField] public int minBranches;
    [SerializeField] public int maxBranches;


    private void Start() {
        nodeObject = Resources.Load<GameObject>("Prefabs/Map/MapNode");
        
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = Resources.Load<Sprite>("Sprites/RoomSprites/" + roomType.ToString());
        
        if(MapManager.mapManager.isGenerating){
            MapManager.mapManager.mapNodes.Add(this);
        }

        for (int i = 0; i < directionsLeft.Length; i++) directionsLeft[i] = false;

        gameObject.transform.SetParent(MapManager.mapManager.transform);

        AddPhysics2DRaycaster();

        GenerateNodes();
    }

    public void GenerateNodes(){
        if(nodeDepth != MapManager.mapManager.depth){
            MapManager.mapManager.depth++;
        }
        if(canGenerate){
            int numberOfNewNodes = Random.Range(minBranches, maxBranches);
            if (MapManager.foundTheFinalPoint) 
            {
                MapManager.finalNode.connections.Add(this);
                connections.Add(MapManager.finalNode);
                AddLine(MapManager.finalNode);
                return;
            }
            for(int i = 0; i < numberOfNewNodes; i++){
                if (MapManager.yLayers[nodeDepth] == -1) MapManager.yLayers[nodeDepth] = transform.position.y + Random.Range(3, 5);

                float x = GetRandomX();
                if (x == -100f) continue;

                for (int j = 0; j < MapManager.mapManager.xByLayers[nodeDepth].Count; j++)
                {
                    if (Mathf.Abs(MapManager.mapManager.xByLayers[nodeDepth][j] - x) < 3f) 
                    {
                        continue;
                    }
                }

                Debug.Log("survived " + nodeDepth + " " + x);
                MapManager.mapManager.xByLayers[nodeDepth+1].Add(x);
                GameObject newNode = Instantiate(nodeObject, new Vector3(Mathf.Clamp(x, -10, 10), MapManager.yLayers[nodeDepth] + Random.Range(-1, 1), transform.position.z), Quaternion.identity);
                MapNode newMapNode = newNode.GetComponent<MapNode>();
                
                if(MapManager.mapManager.depth < depth){
                    newMapNode.canGenerate = true;
                    newMapNode.roomType = GetARoom(roomType,RoomsInARow);
                    if (newMapNode.roomType == roomType) newMapNode.RoomsInARow = RoomsInARow + 1;
                }
                else
                {
                    newMapNode.canGenerate = false;
                    newMapNode.transform.position = new Vector2(0, transform.position.y + 5);
                    MapManager.foundTheFinalPoint = true;
                    newMapNode.roomType = RoomType.Hunter;
                    newMapNode.transform.localScale = Vector3.one;
                    MapManager.finalNode = newMapNode;
                }
                
                newMapNode.parentNode = this;
                newMapNode.nodeDepth = nodeDepth + 1;
                connections.Add(newMapNode);
                newMapNode.connections.Add(this);

                AddLine(newMapNode);
                
                if (MapManager.foundTheFinalPoint) break;
            }
        }
    }

    void AddLine(MapNode newMapNode) 
    {
        GameObject newLine = Instantiate(lineObject, transform.position, Quaternion.identity);
        LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();

        newLine.transform.SetParent(transform);

        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, newMapNode.transform.position);
        lines.Add(lineRenderer);
        newMapNode.lines.Add(lineRenderer);
        MapManager.mapManager.lines.Add(lineRenderer);

        for (int j = 0; j < MapManager.mapManager.lines.Count; j++)
        {
            if (MapManager.mapManager.LinesInterescting(lineRenderer, MapManager.mapManager.lines[j]) && !lines.Contains(MapManager.mapManager.lines[j]) && !newMapNode.lines.Contains(MapManager.mapManager.lines[j]) && MapManager.finalNode != newMapNode)
            {
                MapManager.mapManager.mapNodes.Remove(newMapNode);
                connections.Remove(newMapNode);
                Destroy(newMapNode.gameObject);

                MapManager.mapManager.lines.Remove(lineRenderer);
                lines.Remove(lineRenderer);
                Destroy(newLine);
                //Debug.Log("intersecting");
            }
        }
    }

   public void OnPointerDown(PointerEventData eventData)
    {
        MapManager.mapManager.UpdateCurrentNode(this);
        //Debug.Log("Clicked " + eventData.pointerCurrentRaycast.gameObject.GetComponent<MapNode>().roomType + " room");
    } 

    private void AddPhysics2DRaycaster()
    {
        Physics2DRaycaster physicsRaycaster = FindObjectOfType<Physics2DRaycaster>();
        if (physicsRaycaster == null)
        {
            Camera.main.gameObject.AddComponent<Physics2DRaycaster>();
        }
    }


    float GetRandomX()
    {
        int direction;
        List<int> directions = new List<int>();

        for (int i = 0; i < directionsLeft.Length; i++)
        {
            if (!directionsLeft[i])
            {
                directions.Add(i);
            }
        }
        direction = Random.Range(0, directions.Count);
        direction = directions[direction];
        directionsLeft[direction] = true;
        direction--;
        float final = transform.position.x + Random.Range(1, 3) * direction;

        if (final > 10f || final < -10f) return -100f;

        return final;
    }

    RoomType GetARoom(RoomType lastRoom, int timesInARoll) 
    {
        bool retryed = false;
        RoomType room = RoomType.emptyRoom;

    retry:;

        int randomValue = Random.Range(1, 101);

        for (int i = 0; i < 4; i++)
        {
            if (randomValue < MapManager.mapManager.chaceForRooms[i]) 
            {
                if (retryed && lastRoom == (RoomType)i) 
                {
                    goto retry;
                }

                room = (RoomType)i;
                break;
            }
            randomValue -= MapManager.mapManager.chaceForRooms[i];
        }

        if (room == lastRoom && Random.Range(0, 2 + RoomsInARow) > 1 && !retryed)
        {
            retryed = true;
            goto retry;
        }

        if (room == lastRoom && room == RoomType.Graveyard) goto retry;

        if (room == RoomType.emptyRoom) goto retry;

        return room;
    }
}
