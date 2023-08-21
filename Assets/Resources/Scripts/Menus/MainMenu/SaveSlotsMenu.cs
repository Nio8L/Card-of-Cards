using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SaveSlotsMenu : MonoBehaviour
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;

    private SaveSlot[] saveSlots;

    [Header("Menu Buttons")]
    [SerializeField] private Button backButton;

    private bool isLoadingGame = false;

    private void Awake() {
        saveSlots = GetComponentsInChildren<SaveSlot>();
    }

    public void OnSaveSlotClick(SaveSlot saveSlot){
        DisableMenuButtons();

        DataPersistenceManager.DataManager.ChangeSelectedProfileId(saveSlot.GetProfileId());

        if(!isLoadingGame){
            DataPersistenceManager.DataManager.NewGame();
        }
    
        SceneManager.LoadSceneAsync("Map");
    }

    public void OnBackClick(){
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }


    public void ActivateMenu(bool isLoadingGame){
        gameObject.SetActive(true);
        
        this.isLoadingGame = isLoadingGame;

        Dictionary<string, GameData> profilesGameData = DataPersistenceManager.DataManager.GetAllProfilesGameData();

        foreach(SaveSlot saveSlot in saveSlots){
            GameData profileData = null;
            profilesGameData.TryGetValue(saveSlot.GetProfileId(), out profileData);
            saveSlot.SetData(profileData);

            if(profileData == null && isLoadingGame){
                saveSlot.SetInteractable(false);
            }else{
                saveSlot.SetInteractable(true);
            }
        }
    }

    public void DeactivateMenu(){
        gameObject.SetActive(false);
    }

    private void DisableMenuButtons(){
        foreach(SaveSlot saveSlot in saveSlots){
            saveSlot.SetInteractable(false);
        }
        backButton.interactable = false;
    }
}
