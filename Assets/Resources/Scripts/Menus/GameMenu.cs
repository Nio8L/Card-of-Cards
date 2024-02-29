using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] public GameObject menuButtons;
    [SerializeField] private GameObject settingsMenu;

    [Header("Stuff to Disable")]
    public GameObject[] stuffToDisable;

    public bool aboutToOpenMenu = false;

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Escape)){
            aboutToOpenMenu = true;           
        }

        if(Input.GetKeyUp(KeyCode.Escape)){
            aboutToOpenMenu = false;
            if(!settingsMenu.activeSelf){
                ChangeButtonsState();
                TooltipSystem.QuickHide();
            }else{
                SoundManager.soundManager.Play("ButtonClick");
                settingsMenu.SetActive(!settingsMenu.activeSelf);
                ChangeUIState();
            }
        }
    }

    public void OpenMenu(){
        if(!settingsMenu.activeSelf){
            ChangeButtonsState();
            TooltipSystem.QuickHide();
        }else{
            SoundManager.soundManager.Play("ButtonClick");
            settingsMenu.SetActive(!settingsMenu.activeSelf);
            ChangeUIState();
        }
    }

    public void ResumeGame(){
        ChangeButtonsState();
    }

    public void OnClickSettings(){
        SoundManager.soundManager.Play("ButtonClick");
        DataPersistenceManager.DataManager.LoadSettings();
        menuButtons.SetActive(!menuButtons.activeSelf);
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }

    public void OnClickMainMenu(){
        EventManager.CombatEnd?.Invoke();
        ChangeButtonsState();
        SceneManager.LoadSceneAsync("Main Menu");
        // Clear all existing displays
        DeckUtilities.CloseAllDisplays();
    }

    private void ChangeButtonsState(){
        SoundManager.soundManager.Play("ButtonClick");
        menuButtons.SetActive(!menuButtons.activeSelf);
        ChangeUIState();
    }

    private void ChangeUIState(){
        for(int i = 0; i < stuffToDisable.Length; i++){
            stuffToDisable[i].SetActive(!stuffToDisable[i].activeSelf);
        }
        GameObject.Find("NotificationManager").GetComponent<NotificationManager>().SetActiveNotificaitons(stuffToDisable[0].activeSelf);
    }
}
