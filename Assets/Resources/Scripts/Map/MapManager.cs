using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager mapManager;

    public List<MapNode> mapNodes;
    public List<LineRenderer> lines;
    //public List<Transform> mapNodeCoords;

    public MapNode currentNode;

    public int depth = 0;

    private void Awake() {
        mapManager = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currentNode.GetComponent<SpriteRenderer>().color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if(!mapNodes[^1].canGenerate){
            CutNodes();
        }
    }

    public void UpdateCurrentNode(MapNode newNode){
        if(newNode.nodeDepth > currentNode.nodeDepth && currentNode.connections.Contains(newNode)){
        
            currentNode.spriteRenderer.color = Color.white;
            currentNode = newNode;
            currentNode.spriteRenderer.color = Color.red;
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
            if(mapNodes[i].lines.Count == 1 && mapNodes[i].nodeDepth < depth && mapNodes[i].lines.Count == 1){  
                for(int j = mapNodes[i].connections.Count - 1; j > 0; j--){
                    mapNodes[i].connections[j].connections.Remove(mapNodes[i]);
                }
                mapNodes[i].parentNode.lines.Remove(mapNodes[i].lines[0]);
                Destroy(mapNodes[i].lines[0].gameObject);

                Destroy(mapNodes[i].gameObject);
                mapNodes.Remove(mapNodes[i]);
            }
        }
    }

}
