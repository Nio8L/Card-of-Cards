using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MapNode : MonoBehaviour
{
    public enum RoomType{
        Combat,
        Hunt,
        RestSite,
        Graveyard,
        Hunter,
        emptyRoom
    };

    public List<MapNode> parents;
    public List<MapNode> children;
    public RoomType roomType;
    public List<LineRenderer> lines;
    public bool used = false;

    public void Clicked() 
    {
        MapManager.NodeClicked(this);
    }
}
