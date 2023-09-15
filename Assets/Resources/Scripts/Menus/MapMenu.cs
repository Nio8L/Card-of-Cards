using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapMenu : MonoBehaviour
{
    public GameObject menuButtons;
    public GameObject stuffToDisable;
    public SettingsMenu settingsMenu;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape)){
            SoundManager.soundManager.Play("ButtonClick");
            if(!settingsMenu.gameObject.activeSelf){
                menuButtons.SetActive(!menuButtons.activeSelf);
                stuffToDisable.SetActive(!stuffToDisable.activeSelf);
            }else{
                settingsMenu.DeactivateMenu();
                stuffToDisable.SetActive(!stuffToDisable.activeSelf);
            }
        }
    }

    public void OnClickMainMenu(){
        SoundManager.soundManager.Play("ButtonClick");
        menuButtons.SetActive(!menuButtons.activeSelf);
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void ResumeGame(){
        SoundManager.soundManager.Play("ButtonClick");
        menuButtons.SetActive(!menuButtons.activeSelf);
        stuffToDisable.SetActive(!stuffToDisable.activeSelf);
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
