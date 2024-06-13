using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName="Map World")]
public class MapWorld : ScriptableObject
{
    public Color backgroundColor;
    public int stages;
    public int minWidth;
    public int maxWidth;
    public bool spawnBoss;
    [Header("Spawn chances for all rooms should add up to 1")]
    public List<Room>          roomChances = new List<Room>();
    public List<GameObject>    events  = new List<GameObject>();
    public List<EnemyAI>       combats = new List<EnemyAI>();
    public List<ScriptedEnemy> hunts   = new List<ScriptedEnemy>();
    public List<EnemyAI>       hunters = new List<EnemyAI>();

    public Layer[] floor;

    public List<Layer> layerBuilder = new List<Layer>();

    // Used only for map generation
    public int mapSeed;
    // Used for everything else (events, combats, etc.)
    public int generalSeed;

    public enum RoomType
    {
        None,
        Elite,
        Hunt,
        Event,
        Restsite,
        Unknown,
        Hunter
    };

    List<GameObject> nodesAsGameObjects = new List<GameObject>();
    [System.Serializable]
    public class Node{
        public RoomType thisRoom;
        public EnemyBase thisEnemy;
        public GameObject thisEvent;
        public int layerIndex;
        public int index;
        public Node(int _index, Layer _layer){
            index = _index;
            layerIndex = _layer.index;
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
    [System.Serializable]
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
    [System.Serializable]
    public class Room{
        public RoomType roomType;
        public float chanceToAppear;
    }
    public void GenerateLayout(){
        Random.InitState(mapSeed);

        if (spawnBoss) floor = new Layer[stages + 1 + layerBuilder.Count];
        else           floor = new Layer[stages + layerBuilder.Count];
        
        // Start creating layers
        for (int layer = 0; layer < floor.Length; layer++){
            // Generate this layers amount of nodes
            int layerSize = minWidth;
            if (layer < layerBuilder.Count){
                 layerSize = layerBuilder[layer].size;
            }
            else if (minWidth != maxWidth && layer != 0){
                do{
                    layerSize = Mathf.FloorToInt(Random.value * maxWidth) + minWidth;
                    if (layerSize > maxWidth) layerSize = maxWidth;
                }while(layer != 1 && layerSize == floor[layer-2].size);
            }
            
            if (spawnBoss && layer == stages + layerBuilder.Count) layerSize = 1;

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
                Vector3 position = new Vector3(node * 2f - thisLayer.nodes.Length + Random.value, layer * 3 + Random.value, 0);
                GameObject newNode = Instantiate(nodeBase, position, Quaternion.identity);
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
            // Generate rooms
            if (layer >= layerBuilder.Count){
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
                if (spawnBoss && layer == floor.Length-1){
                    for (int node = 0; node < thisLayer.nodes.Length; node++){
                        thisLayer.nodes[node].thisRoom = RoomType.Hunt;
                        GetGameObjectFromNode(thisLayer.nodes[node]).GetComponent<MapNode>().SetRoom(RoomType.Hunter);
                    }
                    continue;
                }

                // Make the seconds to last layer a rest site
                if (spawnBoss && layer == floor.Length-2){
                    for (int node = 0; node < thisLayer.nodes.Length; node++){
                        thisLayer.nodes[node].thisRoom = RoomType.Restsite;
                        GetGameObjectFromNode(thisLayer.nodes[node]).GetComponent<MapNode>().SetRoom(RoomType.Restsite);
                    }
                    continue;
                }

                // Random
                for (int node = 0; node < thisLayer.nodes.Length; node++){
                    RoomType roomTypeToUse = RoomType.Elite;

                    // Select a room 
                    bool end = false;
                    while (!end){
                        // Pick a random room
                        float roomToUse = Random.value;
                        for (int i = 0; i < roomChances.Count; i++){
                            if (roomToUse <= roomChances[i].chanceToAppear){
                                roomTypeToUse = roomChances[i].roomType;
                                break;
                            }else{
                                roomToUse -= roomChances[i].chanceToAppear;                         
                            }
                        }

                        // Check if its banned
                        end = true;
                        for (int i = 0; i < bannedRooms.Count; i++){
                            if (bannedRooms[i] == roomTypeToUse){
                                end = false;
                                break;
                            }
                        }
                    }

                    // Use the selected room
                    MapNode nodeGameObject = GetGameObjectFromNode(thisLayer.nodes[node]).GetComponent<MapNode>();
                    thisLayer.nodes[node].thisRoom = roomTypeToUse;
                    nodeGameObject.SetRoom(roomTypeToUse);
                }
            }else{
                // Use prebuild
                Layer builderLayer = layerBuilder[layer];
                for (int node = 0; node < builderLayer.nodes.Length; node++){
                    RoomType roomTypeToUse = builderLayer.nodes[node].thisRoom;

                    // Use the selected room
                    MapNode nodeGameObject = GetGameObjectFromNode(thisLayer.nodes[node]).GetComponent<MapNode>();
                    thisLayer.nodes[node].thisRoom = roomTypeToUse;
                    nodeGameObject.SetRoom(roomTypeToUse);
                }
            }
        }
    }
    public void AssignEncounters(){
        // Assigns enemy ai's and events to nodes
        EnemyBase  lastEnemy = null;
        GameObject lastEvent = null;
        // Go through all layers
        for (int layer = 0; layer < floor.Length; layer++){
            // Find this layer
            Layer thisLayer = floor[layer];

            // Go through all nodes
            for (int node = 0; node < thisLayer.nodes.Length; node++){
                // Find the room of this node
                RoomType roomTypeToUse = thisLayer.nodes[node].thisRoom;

                // Find its game object
                MapNode nodeGameObject = GetGameObjectFromNode(thisLayer.nodes[node]).GetComponent<MapNode>();

                // Skip this node if it's a restsite
                if (roomTypeToUse == RoomType.Restsite) continue;

                // Generate encounter
                if (layer >= layerBuilder.Count){
                    // Select unknown 
                    if (roomTypeToUse == RoomType.Unknown){
                        float rand = Random.value;
                        if      (rand <= 0.1f)  roomTypeToUse = RoomType.Elite; // 10%
                        else if (rand <= 0.55f) roomTypeToUse = RoomType.Hunt;  // 45%
                        else                    roomTypeToUse = RoomType.Event; // 45%
                    }
                    // Select a combat
                    if (roomTypeToUse != RoomType.Event){
                        EnemyBase enemy;
                        do{
                            if      (roomTypeToUse == RoomType.Elite) enemy = combats[Random.Range(0, combats.Count)];
                            else if (roomTypeToUse == RoomType.Hunt  ) enemy = hunts  [Random.Range(0, hunts  .Count)];
                            else                                       enemy = hunters[Random.Range(0, hunters.Count)];
                        }while(lastEnemy == enemy);
                        
                        nodeGameObject.AssignEnemy(enemy);
                        lastEnemy = enemy;
                        continue;
                    }

                    // Select an event
                    if (roomTypeToUse == RoomType.Event){
                        GameObject eventToUse;
                        do{
                            eventToUse = events[Random.Range(0, events.Count)];
                        }while(lastEvent == eventToUse);

                        nodeGameObject.AssignEvent(eventToUse);
                        lastEvent = eventToUse;
                        continue;
                    }
                }else{
                    // Use pre-build layers
                    Layer builderLayer = layerBuilder[layer];

                    if (roomTypeToUse != RoomType.Event){
                        nodeGameObject.AssignEnemy(builderLayer.nodes[node].thisEnemy);
                    }else{
                        nodeGameObject.AssignEvent(builderLayer.nodes[node].thisEvent);
                    }
                }
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
        Layer startLayer = floor[startNode.layerIndex];

        int connectionWindow = Mathf.CeilToInt((float)targetLayer.size/startLayer.size);
        int minConnectionIndex = startNode.index * connectionWindow;
        int connectionIndex = Random.Range(minConnectionIndex, minConnectionIndex + connectionWindow);

        if (connectionIndex >= targetLayer.size) connectionIndex = targetLayer.size-1;

        return targetLayer.nodes[connectionIndex];
    }
    bool ConnectionExists(Node bottomNode, Node topNode){
        // Returns true if there is a connection between the 2 given nodes
        Layer layer = floor[bottomNode.layerIndex];
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
        Layer thisLayer = floor[targetNode.layerIndex];
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
