using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenePersistenceManager : MonoBehaviour
{
    public static ScenePersistenceManager scenePersistence;

    public MapWorld mapWorld;

    [Header("Between scenes tracking")]
    public EnemyBase currentCombatAI;
    public List<Card> playerDeck;
    public string lastEvent;

    [Header("Tutorial tracking")]
    public bool inTutorial;
    public int tutorialStage = 0;
    public List<Card> tutorialDeck;
    public List<ListWrapper> tutorialCardsToAdd;
    public List<EnemyBase> tutorialCombats;

    private void Awake() {
        if(scenePersistence != null){
            Destroy(gameObject);
            return;
        }

        scenePersistence = this;
        DontDestroyOnLoad(gameObject);        
    }

}
