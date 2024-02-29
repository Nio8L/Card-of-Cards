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

    public bool displaysHidden = false;
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

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneChange;
    }

    void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneChange;
    }

    void Update(){
        if(Input.GetKeyUp(KeyCode.D) && !displaysHidden){
            string sceneName = SceneManager.GetActiveScene().name;

            List<Card> cards = new List<Card>();

            if      (sceneName == "Map")    cards = MapManager.mapManager.mapDeck.cards;
            else if (sceneName == "Combat") cards = CombatManager.combatManager.deck.cards;

            if (cards.Count != 0) SingularDisplay("deck", cards);
        }
        
        if (Input.GetKeyUp(KeyCode.Escape)){
            SetActiveDisplays(displaysHidden);
            displaysHidden = !displaysHidden;
        }
    }

    public static void UpdateAllDisplays(){
        for (int i = 0; i < deckUtilities.activeDisplays.Count; i++){
            DeckDisplay deckDisplay = deckUtilities.activeDisplays[i];
            if (deckDisplay != null){
                deckDisplay.ShowCards();
            }
        }
    }

    public static DeckDisplay CreateDisplay(Vector2 position, float width, float height, string name){
        // Creates a display via code and returns it
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
        // Closes all displays
        while (deckUtilities.activeDisplays.Count > 0){
            if (deckUtilities.activeDisplays[0] != null){
                deckUtilities.activeDisplays[0].CloseDisplay();
            }else{
                deckUtilities.activeDisplays.RemoveAt(0);
            }
        }
    }
    public static void SetActiveDisplays(bool active){
        // makes all displays active or inactive
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

    void OnSceneChange(Scene scene, LoadSceneMode mode){
        displaysHidden = false;
    }

}
