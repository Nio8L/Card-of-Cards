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

    [Header("Decks")]
    public List<Card> startCards = new List<Card>();
    private bool isLoadingGame = false;

    EnemyBase defaultFight;

    private void Awake() {
        saveSlots = GetComponentsInChildren<SaveSlot>();
        defaultFight = Resources.Load<EnemyBase>("Enemies/Sheep");
    }

    public void OnSaveSlotClick(SaveSlot saveSlot){
        SoundManager.soundManager.Play("ButtonClick");

        DisableMenuButtons();

        DataPersistenceManager.DataManager.ChangeSelectedProfileId(saveSlot.GetProfileId());

        if(!isLoadingGame){
            DataPersistenceManager.DataManager.playerDeck.AddRange(startCards);
            DataPersistenceManager.DataManager.currentCombatAI = defaultFight;
            DataPersistenceManager.DataManager.NewGame();
        }


        SceneManager.LoadSceneAsync("Map");
    }

    public void OnBackClick(){
        SoundManager.soundManager.Play("ButtonClick");
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
