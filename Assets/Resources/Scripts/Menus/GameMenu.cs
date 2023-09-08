using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] public GameObject menuButtons;
    [SerializeField] private Button mapButton;

    [SerializeField] private GameObject settingsMenu;

    [Header("Stuff to Disable")]
    [SerializeField] private GameObject cardSlots;
    [SerializeField] private GameObject cardsInHand;
    [SerializeField] private GameObject cardsInCombat;
    [SerializeField] private GameObject bench;
    [SerializeField] private GameObject pilesAndHp;

    private void Update() {
        if(Input.GetKeyUp(KeyCode.Escape)){
           ChangeButtonsState();
           TooltipSystem.QuickHide();
        }
    }

    public void ResumeGame(){
        ChangeButtonsState();
    }

    public void SaveGame(){
        DataPersistenceManager.DataManager.SaveGame();
    }

    public void OnClickSettings(){
        SoundManager.soundManager.Play("ButtonClick");
        DataPersistenceManager.DataManager.LoadSettings();
        menuButtons.SetActive(!menuButtons.activeSelf);
        settingsMenu.SetActive(!settingsMenu.activeSelf);
    }

    public void OnClickMainMenu(){
        ChangeButtonsState();
        SceneManager.LoadSceneAsync("Main Menu");
    }

    private void ChangeButtonsState(){
        SoundManager.soundManager.Play("ButtonClick");
        menuButtons.SetActive(!menuButtons.activeSelf);
        mapButton.interactable = !CombatManager.inCombat;
        ChangeUIState();
    }

    private void ChangeUIState(){
        cardSlots.SetActive(!cardSlots.activeSelf);
        cardsInHand.SetActive(!cardsInHand.activeSelf);
        cardsInCombat.SetActive(!cardsInCombat.activeSelf);
        bench.SetActive(!bench.activeSelf);
        pilesAndHp.SetActive(!pilesAndHp.activeSelf);
    }

    public void OnClickMap(){
        ChangeButtonsState();
        SceneManager.LoadSceneAsync("Map");
    }
}
