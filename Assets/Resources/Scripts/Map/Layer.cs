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

    public ConectionType enterConectionType;
    public ConectionType exitConectionType;

    public Nodes[] enterNodes;
    public Nodes[] exitNodes;

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
}
