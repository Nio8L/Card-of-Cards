using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapNode : MonoBehaviour
{
    public MapWorld.Node thisNode;
    public EnemyBase enemyOnThisNode;
    public GameObject eventOnThisNode;
    public void SetRoom(MapWorld.RoomType roomType){
        thisNode.thisRoom = roomType;
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/RoomSprites/" + roomType.ToString());
        if (roomType == MapWorld.RoomType.Hunter){
            transform.localScale = Vector3.one * 2f;
            transform.GetChild(0).localScale = Vector3.one * 2f;
        }
    }

    public void AssignEnemy(EnemyBase _enemy){
        enemyOnThisNode = _enemy;
    }
    public void AssignEvent(GameObject _event){
        eventOnThisNode = _event;
    }
    public void Clicked(){
        if (!MapManager.mapManager.canTravel) return;

        // Find if this is a selectable node
        for (int i = 0; i < MapManager.mapManager.selectableNodes.Count; i++){
            if (MapManager.mapManager.selectableNodes[i] == gameObject){
                // If it is select it
                MapManager.SelectNode  (this);
                MapManager.ActivateNode(this);
                break;
            }
        }
    }
}
