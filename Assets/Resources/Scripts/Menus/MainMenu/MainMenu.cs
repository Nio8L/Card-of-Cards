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
    private void Start() {
        if(!DataPersistenceManager.DataManager.HasGameData()){
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
        SceneManager.LoadSceneAsync("SampleScene");
    }  

    public void OnSettingsClick(){
        SoundManager.soundManager.Play("ButtonClick");
        settingsMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void ActivateMenu(){
        gameObject.SetActive(true);
    }

    public void DeactivateMenu(){
        gameObject.SetActive(false);
    } 
}
