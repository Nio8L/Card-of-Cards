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
    public RoomType roomType = RoomType.emptyRoom;
    public List<LineRenderer> lines;
    public bool used = false;
    public GameObject indicator;

    public void OnPointerDown()
    {
        MapManager.NodeClicked(this);
    }
}
