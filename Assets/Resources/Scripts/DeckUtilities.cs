using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeckUtilities : MonoBehaviour
{
    public static DeckUtilities deckUtilities;
    public GameObject deckDisplay;

    public List<DeckDisplay> activeDisplays = new List<DeckDisplay>();
    void Awake(){
        // Destroy this object if a DeckUtilities manager already exists
        if(deckUtilities != null){
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        deckUtilities = this;
        activeDisplays = new();
    }

    void Update(){
        if(Input.GetKeyUp(KeyCode.D)){
            List<Card> cards = new List<Card>();
            if      (SceneManager.GetActiveScene().name == "Map")         cards = MapManager.mapManager.mapDeck.cards;
            else if (SceneManager.GetActiveScene().name == "SampleScene") cards = CombatManager.combatManager.deck.cards;
            SingularDisplay("deck", cards);
        }
    }

    public static DeckDisplay CreateDisplay(Vector2 position, float width, float height, string name){
        DeckDisplay display = Instantiate(deckUtilities.deckDisplay, Vector3.zero, Quaternion.identity).GetComponent<DeckDisplay>();
        display.point = position;
        display.width = width;
        display.height = height;
        display.name = name;
        return display;
    }
    public static void AddDisplay(DeckDisplay newDisplay){
        // Add a display to the list
        deckUtilities.activeDisplays.Add(newDisplay);
    }

    public static void CloseAllDisplays(){
        while (deckUtilities.activeDisplays.Count > 0){
            deckUtilities.activeDisplays[0].CloseDisplay();
        }
    }
    public static void SetActiveDisplays(bool active){
        for(int i = 0; i < deckUtilities.activeDisplays.Count; i++){
            if (deckUtilities.activeDisplays[i] != null)
            {
                deckUtilities.activeDisplays[i].gameObject.SetActive(active);
            }
        }
    }

    public static DeckDisplay GetDisplayWithName(string name){
        DeckDisplay deck = null;

        for (int i = 0; i < deckUtilities.activeDisplays.Count; i++){
            if (deckUtilities.activeDisplays[i] != null)
            {
                if (deckUtilities.activeDisplays[i].name == name){
                    deck = deckUtilities.activeDisplays[i];
                }
            }
        }
        return deck;
    }

    public static void SingularDisplay(string name, List<Card> cards){
        DeckDisplay deck = GetDisplayWithName(name);
        if (deck == null){
            deck = CreateDisplay(Vector2.zero, Camera.main.pixelWidth*(2f/3f), Camera.main.pixelHeight*(2f/3f), name);
            deck.cards = cards;
        }else{
            deck.CloseDisplay();
        }
    }

}
