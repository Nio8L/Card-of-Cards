using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapMenu : MonoBehaviour
{
    public GameObject menuButtons;
    public GameObject[] stuffToDisable;
    public SettingsMenu settingsMenu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape)){
            SoundManager.soundManager.Play("ButtonClick");
            if(!settingsMenu.gameObject.activeSelf){
                menuButtons.SetActive(!menuButtons.activeSelf);
            }else{
                settingsMenu.DeactivateMenu();
            }
            SetActiveObjects();
            DeckUtilities.SetActiveDisplays(!menuButtons.activeSelf);
            TooltipSystem.QuickHide();
        }
    }

    private void SetActiveObjects(){
        for(int i = 0; i < stuffToDisable.Length; i++){
            stuffToDisable[i].SetActive(!stuffToDisable[i].activeSelf);
        }
    }

    public void OpenMenu(){
        SoundManager.soundManager.Play("ButtonClick");
        if(!settingsMenu.gameObject.activeSelf){
            menuButtons.SetActive(!menuButtons.activeSelf);
            SetActiveObjects();
        }else{
            settingsMenu.DeactivateMenu();
            SetActiveObjects();
        }
        DeckUtilities.SetActiveDisplays(!menuButtons.activeSelf);
        TooltipSystem.QuickHide();
    }

    public void OnClickMainMenu(){
        SoundManager.soundManager.Play("ButtonClick");
        menuButtons.SetActive(!menuButtons.activeSelf);
        SceneManager.LoadSceneAsync("Main Menu");
        // Clear all existing displays
        DeckUtilities.CloseAllDisplays();
    }

    public void ResumeGame(){
        SoundManager.soundManager.Play("ButtonClick");
        menuButtons.SetActive(!menuButtons.activeSelf);
        SetActiveObjects();
        DeckUtilities.SetActiveDisplays(!menuButtons.activeSelf);
    }

    public void OnClickSettings(){
        SoundManager.soundManager.Play("ButtonClick");
        DataPersistenceManager.DataManager.LoadSettings();
        menuButtons.SetActive(!menuButtons.activeSelf);
        settingsMenu.ActivateMenu();
    }

    public void SaveGame(){
        DataPersistenceManager.DataManager.SaveGame();
    }
}
