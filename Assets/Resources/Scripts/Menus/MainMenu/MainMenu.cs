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

    [SerializeField] private AudioSource buttonClick;
    
    [SerializeField] private GameObject mainMenu;

    private void Start() {
        if(!DataPersistenceManager.DataManager.HasGameData()){
            continueButton.interactable = false;
            loadButton.interactable = false;
        }
    }

    public void OnNewGameClick(){
        PlaySound();
        saveSlotsMenu.ActivateMenu(false);
        DeactivateMenu();
    }

    public void OnLoadGameClicked(){
        PlaySound();
        saveSlotsMenu.ActivateMenu(true);
        DeactivateMenu();
    }

    public void OnContinueClick(){
        PlaySound();
        SceneManager.LoadSceneAsync("SampleScene");
    }  

    public void OnSettingsClick(){
        PlaySound();
        settingsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void ActivateMenu(){
        mainMenu.SetActive(true);
    }

    public void DeactivateMenu(){
        mainMenu.SetActive(false);
    } 

    public void PlaySound(){
        buttonClick.Play();
    }
}
