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
        DataPersistenceManager.DataManager.inTutorial = false;
        if (!DataPersistenceManager.DataManager.HasGameData()){
            continueButton.interactable = false;
            loadButton.interactable = false;
        }
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

    public void StartTutorial(EnemyBase enemy)
    {
        DataPersistenceManager.DataManager.inTutorial = true;
        DataPersistenceManager.DataManager.currentCombatAI = enemy;
        DataPersistenceManager.DataManager.playerDeck.AddRange(tutorialCards);
        SceneManager.LoadSceneAsync("SampleScene");
    }
}
