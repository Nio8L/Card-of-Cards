using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    [Header("Menu Buttons")]
    [SerializeField] private GameObject backButton;
    [SerializeField] private GameObject saveGameButton;
    [SerializeField] private GameObject mainMenuButton;

    [Header("Stuff to Disable")]
    [SerializeField] private GameObject cardSlots;
    [SerializeField] private GameObject cardsInHand;
    [SerializeField] private GameObject cardsInCombat;
    [SerializeField] private GameObject buttons;
    [SerializeField] private GameObject bench;
    [SerializeField] private GameObject pilesAndHp;

    private void Update() {
        if(Input.GetKeyUp(KeyCode.Escape)){
           ChangeButtonsState();
           TooltipSystem.Hide();
        }
    }

    public void ResumeGame(){
        ChangeButtonsState();
    }

    public void SaveGame(){
        DataPersistenceManager.DataManager.SaveGame();
    }

    public void OnClickMainMenu(){
        ChangeButtonsState();
        SceneManager.LoadSceneAsync("Main Menu");
    }

    private void ChangeButtonsState(){
        backButton.SetActive(!backButton.activeSelf);
        saveGameButton.SetActive(!saveGameButton.activeSelf);
        mainMenuButton.SetActive(!mainMenuButton.activeSelf);
        ChangeUIState();
    }

    private void ChangeUIState(){
        cardSlots.SetActive(!cardSlots.activeSelf);
        cardsInHand.SetActive(!cardsInHand.activeSelf);
        cardsInCombat.SetActive(!cardsInCombat.activeSelf);
        buttons.SetActive(!backButton.activeSelf);
        bench.SetActive(!bench.activeSelf);
        pilesAndHp.SetActive(!pilesAndHp.activeSelf);
    }
}
