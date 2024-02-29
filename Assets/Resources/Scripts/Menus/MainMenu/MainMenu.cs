using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private SaveSlotsMenu saveSlotsMenu;
    [SerializeField] private SettingsMenu settingsMenu;

    [Header("Main Menu Buttons")]
    [SerializeField] private Button continueButton;
    [SerializeField] private Button loadButton;

    public List<Card> tutorialCards = new List<Card>();
    private void Start() {
        //Reset tutorial
        ScenePersistenceManager.scenePersistence.inTutorial = false;
        ScenePersistenceManager.scenePersistence.tutorialStage = 0;
        ScenePersistenceManager.scenePersistence.tutorialDeck.Clear();

        ScenePersistenceManager.scenePersistence.currentCombatAI = null;
        if (!DataPersistenceManager.DataManager.HasGameData()){
            continueButton.interactable = false;
            loadButton.interactable = false;
        }

        //Reset the map world
        ScenePersistenceManager.scenePersistence.mapWorld = Resources.Load<MapWorld>("Map Worlds/Base world");
    }

    public void OnNewGameClick(){
        SoundManager.soundManager.Play("ButtonClick");
        saveSlotsMenu.ActivateMenu(false);
        DeactivateMenu();
    }

    public void OnLoadGameClicked(){
        SoundManager.soundManager.Play("ButtonClick");
        saveSlotsMenu.ActivateMenu(true);
        DeactivateMenu();
    }

    public void OnContinueClick(){
        SoundManager.soundManager.Play("ButtonClick");
        SceneManager.LoadSceneAsync("Map");
    }  

    public void OnSettingsClick(){
        SoundManager.soundManager.Play("ButtonClick");
        DataPersistenceManager.DataManager.LoadSettings();
        settingsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void OnQuitClick(){
        Application.Quit();
    }

    public void ActivateMenu(){
        gameObject.SetActive(true);
    }

    public void DeactivateMenu(){
        gameObject.SetActive(false);
    } 

    public void StartTutorial()
    {
        ScenePersistenceManager.scenePersistence.inTutorial = true;
        ScenePersistenceManager.scenePersistence.mapWorld = Resources.Load<MapWorld>("Map Worlds/Tutorial");
        SceneManager.LoadSceneAsync("Map");
    }
}
