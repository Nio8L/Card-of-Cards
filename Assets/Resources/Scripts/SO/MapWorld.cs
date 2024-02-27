using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName="Map World")]
public class MapWorld : ScriptableObject
{
    public int stages;
    public int minWidth;
    public int maxWidth;
    public List<GameObject>    events  = new List<GameObject>();
    public List<EnemyAI>       combats = new List<EnemyAI>();
    public List<ScriptedEnemy> hunts   = new List<ScriptedEnemy>();
    public List<EnemyAI>       hunters = new List<EnemyAI>();

    public Layer[] floor;

    public int randomSeed;

    public enum RoomType
    {
        None,
        Combat,
        Hunt,
        Event,
        Restsite,
        Hunter
    };

    List<GameObject> nodesAsGameObjects = new List<GameObject>();
    public class Node{
        public RoomType thisRoom;
        public Layer layer;
        public int index;
        public Node(int _index, Layer _layer){
            index = _index;
            layer = _layer;
        }
    }
    public class Connection{
        public Node bottomNode;
        public Node topNode;

        public Connection(Node _bottomNode, Node _topNode){
            bottomNode  = _bottomNode;
            topNode     = _topNode;
        }
    }
    public class Layer{
        public int size;
        public int index;
        public Node[] nodes;
        public List<Connection> connections = new List<Connection>();
        public Layer(int _size, int _index){
            size = _size;
            index = _index;
            nodes = new Node[_size];
            
        }

    }
    public void GenerateLayout(){
        Random.InitState(randomSeed);

        floor = new Layer[stages + 1];
        
        // Start creating layers
        for (int layer = 0; layer < stages + 1; layer++){
            // Generate this layers amount of nodes
            int layerSize = minWidth;
            if (minWidth != maxWidth && layer != 0){
                do{
                    layerSize = Mathf.FloorToInt(Random.value * maxWidth + minWidth);
                }while(layerSize == floor[layer-1].size);
            }
            
            if (layer == stages) layerSize = 1;

            // Generate this layer
            Layer thisLayer = new Layer(layerSize, layer);
            floor[layer] = thisLayer;

            // Generate this layer's nodes
            for (int nodeIndex = 0; nodeIndex < layerSize; nodeIndex++){
                thisLayer.nodes[nodeIndex] = new Node(nodeIndex, thisLayer);
            }

            // If this is the first layer stop here
            if (layer == 0) continue;

            Layer lastLayer = floor[layer-1];
            // Make list of unconnected nodes
            List<Node> unconnectedThisLayer = new List<Node>();
            for (int i = 0; i < thisLayer.size; i++) unconnectedThisLayer.Add(thisLayer.nodes[i]);

            List<Node> unconnectedLastLayer = new List<Node>();
            for (int i = 0; i < lastLayer.size; i++) unconnectedLastLayer.Add(lastLayer.nodes[i]);
            
            // Connect all nodes
            bool finished = false;
            while (!finished){
                finished = true;
                if (unconnectedLastLayer.Count > 0){
                    finished = false;
                    // Find an unconnected node 
                    Node nodeToConnect = unconnectedLastLayer[0];

                    // Find the maximum amount of possible connections
                    int maxConnections = Mathf.CeilToInt((float)thisLayer.size/lastLayer.size);
                    // Get a random amount of connections
                    int connections = Random.Range(1, maxConnections);

                    for (int i = 0; i < connections; i++){
                        // Find a suitable connection
                        Node nodeToConnectTo = GetSuitableConnectorNode(nodeToConnect, thisLayer);
                        // If the nodes are already connected continue
                        if (ConnectionExists(nodeToConnect, nodeToConnectTo)) continue;
                        
                        // Connect the nodes
                        Connection newConnection = new Connection(nodeToConnect, nodeToConnectTo);
                        lastLayer.connections.Add(newConnection);

                        // Remove them from the unconnected list
                        unconnectedThisLayer.Remove(nodeToConnectTo);
                        unconnectedLastLayer.Remove(nodeToConnect);
                    }
                }else if (unconnectedThisLayer.Count > 0){
                    finished = false;
                    // Find a suitable connection
                    Node nodeToConnectTo = GetSuitableConnectorNode(unconnectedThisLayer[0], lastLayer);
                    
                     // Connect the nodes
                    Connection newConnection = new Connection(nodeToConnectTo, unconnectedThisLayer[0] );
                    lastLayer.connections.Add(newConnection);

                    // Remove them from the unconnected list
                    unconnectedThisLayer.RemoveAt(0);
                    
                }
            }
        }
    }
    public void GenerateNodes(){
        nodesAsGameObjects.Clear();
        GameObject nodeBase = Resources.Load<GameObject>("Prefabs/Map/MapNode");
        GameObject lineBase = Resources.Load<GameObject>("Prefabs/Map/Line");
        for (int layer = 0; layer < floor.Length; layer++){
            Layer thisLayer = floor[layer];
            // Spawn nodes
            for (int node = 0; node < thisLayer.nodes.Length; node++){
                GameObject newNode = Instantiate(nodeBase, new Vector3(node * 2f - thisLayer.nodes.Length + Random.value, layer * 3 + Random.value, 0), Quaternion.identity);
                newNode.transform.SetParent(MapManager.mapManager.transform);
                newNode.GetComponent<MapNode>().thisNode = thisLayer.nodes[node];

                nodesAsGameObjects.Add(newNode);
            }
        }

        for (int layer = 0; layer < floor.Length; layer++){
            Layer thisLayer = floor[layer];
            // Spawn lines
            for (int line = 0; line < thisLayer.connections.Count; line++){
                GameObject newLine = Instantiate(lineBase, Vector3.zero, Quaternion.identity);
                newLine.transform.SetParent(MapManager.mapManager.transform);
                LineRenderer lineRenderer = newLine.GetComponent<LineRenderer>();
                lineRenderer.SetPosition(0, GetGameObjectFromNode(thisLayer.connections[line].bottomNode).transform.position);
                lineRenderer.SetPosition(1, GetGameObjectFromNode(thisLayer.connections[line].topNode   ).transform.position);
            }
        }
        
    }
    public void AssignRooms(){
        for (int layer = 0; layer < floor.Length; layer++){
            Layer thisLayer = floor[layer];

            List<RoomType> bannedRooms = new List<RoomType>();
            if (layer > 1) bannedRooms = GetRoomsAtLayer(floor[layer - 2]);

            // Make the first layer a hunt
            if (layer == 0){
                for (int node = 0; node < thisLayer.nodes.Length; node++){
                    thisLayer.nodes[node].thisRoom = RoomType.Hunt;
                    GetGameObjectFromNode(thisLayer.nodes[node]).GetComponent<MapNode>().SetRoom(RoomType.Hunt);
                }
                continue;
            }

            // Make the last layer a hunter
            if (layer == floor.Length-1){
                for (int node = 0; node < thisLayer.nodes.Length; node++){
                    thisLayer.nodes[node].thisRoom = RoomType.Hunt;
                    GetGameObjectFromNode(thisLayer.nodes[node]).GetComponent<MapNode>().SetRoom(RoomType.Hunter);
                }
                continue;
            }

            // Make the seconds to last layer a rest site
            if (layer == floor.Length-2){
                for (int node = 0; node < thisLayer.nodes.Length; node++){
                    thisLayer.nodes[node].thisRoom = RoomType.Restsite;
                    GetGameObjectFromNode(thisLayer.nodes[node]).GetComponent<MapNode>().SetRoom(RoomType.Restsite);
                }
                continue;
            }

            // Random
            for (int node = 0; node < thisLayer.nodes.Length; node++){
                RoomType roomTypeToUse = RoomType.Combat;
                bool end = false;
                while (!end){
                    // Pick a random room
                    int roomToUse = Mathf.FloorToInt(Random.value * 4);
                    if      (roomToUse == 0) roomTypeToUse = RoomType.Combat;
                    else if (roomToUse == 1) roomTypeToUse = RoomType.Hunt;
                    else if (roomToUse == 2) roomTypeToUse = RoomType.Restsite;
                    else                     roomTypeToUse = RoomType.Event;

                    // Check if its banned
                    end = true;
                    for (int i = 0; i < bannedRooms.Count; i++){
                        if (bannedRooms[i] == roomTypeToUse) end = false;
                    }
                }
                // Use the selected room
                thisLayer.nodes[node].thisRoom = RoomType.Restsite;
                GetGameObjectFromNode(thisLayer.nodes[node]).GetComponent<MapNode>().SetRoom(roomTypeToUse);
            }

        }
    }
    public GameObject GetGameObjectFromNode(Node node){
        for (int i = 0; i < nodesAsGameObjects.Count; i++){
            if (nodesAsGameObjects[i].GetComponent<MapNode>().thisNode == node){
                return nodesAsGameObjects[i];
            }
        }
        return null;
    }
    Node GetSuitableConnectorNode(Node startNode, Layer targetLayer){
        Layer startLayer = startNode.layer;

        int connectionWindow = Mathf.CeilToInt((float)targetLayer.size/startLayer.size);
        int minConnectionIndex = startNode.index * connectionWindow;
        int connectionIndex = Random.Range(minConnectionIndex, minConnectionIndex + connectionWindow);

        if (connectionIndex >= targetLayer.size) connectionIndex = targetLayer.size-1;

        return targetLayer.nodes[connectionIndex];
    }
    bool ConnectionExists(Node bottomNode, Node topNode){
        // Returns true if there is a connection between the 2 given nodes
        Layer layer = bottomNode.layer;
        for (int i = 0; i < layer.connections.Count; i++){
            Connection connection = layer.connections[i];
            if (connection.bottomNode == bottomNode && connection.topNode == topNode){
                return true;
            }
        }
        return false;
    }
    public List<Node> GetConnectedNodes(Node targetNode){
        List<Node> nodes = new List<Node>();
        Layer thisLayer = targetNode.layer;
        for (int i = 0; i < thisLayer.connections.Count; i++){
            if (thisLayer.connections[i].bottomNode == targetNode){
                nodes.Add(thisLayer.connections[i].topNode);
            }
        }
        return nodes;
    }
    public List<RoomType> GetRoomsAtLayer(Layer targetLayer){
        List<RoomType> rooms = new List<RoomType>();
        for (int i = 0; i < targetLayer.nodes.Length; i++){
            rooms.Add(targetLayer.nodes[i].thisRoom);
        }
        return rooms;
    }
}