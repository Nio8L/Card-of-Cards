using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Layer : MonoBehaviour
{
    [System.Serializable]
    public struct Nodes
    {
        [SerializeField] public MapNode[] NodesOnConections;
    }

    public List<MapNode> mapNodes; 

    public ConectionType enterConectionType;
    public ConectionType exitConectionType;

    public Nodes[] enterNodes;
    public Nodes[] exitNodes;

    public int placeInTheArray;

    public enum ConectionType
    {
        Right,
        RightMiddle,
        RightLeft,
        All,
        Middle,
        MiddleLeft,
        Left,
        None
    };

    public List<MapNode> GetAllExitNodes() 
    {
        List<MapNode> returnList = new List<MapNode>();
        for (int i = 0; i < 3; i++)
        {
            foreach (MapNode node in exitNodes[i].NodesOnConections)
            {
                returnList.Add(node);
            }
        }
        return returnList;
    }
}
