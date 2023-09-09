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
        Hunter
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

        gameObject.transform.SetParent(MapManager.mapManager.transform);

        AddPhysics2DRaycaster();

        GenerateNodes(canGenerate);
    }

    public void GenerateNodes(bool canGenerate){
        if(nodeDepth != MapManager.mapManager.depth){
            MapManager.mapManager.depth++;
        }
        if(canGenerate){
            int numberOfNewNodes = Random.Range(minBranches, maxBranches);

            for(int i = 0; i < numberOfNewNodes; i++){
                GameObject newNode = Instantiate(nodeObject, new Vector3(Mathf.Clamp(transform.position.x + Random.Range(-5, 5), -10, 10), transform.position.y + Random.Range(2, 6), transform.position.z), Quaternion.identity);
                MapNode newMapNode = newNode.GetComponent<MapNode>();
                
                if(MapManager.mapManager.depth < depth){
                    newMapNode.canGenerate = true;
                }else{
                    newMapNode.canGenerate = false;
                }
                
                newMapNode.parentNode = this;
                newMapNode.nodeDepth = nodeDepth + 1;
                newMapNode.roomType = (RoomType)Random.Range(0, 4);
                connections.Add(newMapNode);
                newMapNode.connections.Add(this);
                

                GameObject newLine = Instantiate(lineObject, transform.position, Quaternion.identity);
                LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();

                newLine.transform.SetParent(gameObject.transform);

                lineRenderer.SetPosition(0, transform.position);
                lineRenderer.SetPosition(1, newNode.transform.position);
                lines.Add(lineRenderer);
                newMapNode.lines.Add(lineRenderer);
                MapManager.mapManager.lines.Add(lineRenderer);

                for(int j = 0; j < MapManager.mapManager.lines.Count; j++){
                    if(MapManager.mapManager.LinesInterescting(lineRenderer, MapManager.mapManager.lines[j]) && !lines.Contains(MapManager.mapManager.lines[j]) && !newMapNode.lines.Contains(MapManager.mapManager.lines[j])){
                        MapManager.mapManager.mapNodes.Remove(newMapNode);
                        connections.Remove(newMapNode);
                        Destroy(newNode);

                        MapManager.mapManager.lines.Remove(lineRenderer);
                        lines.Remove(lineRenderer);
                        Destroy(newLine);
                        //Debug.Log("intersecting");
                    }
                }
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
}
