using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]
    [SerializeField] private string fileName;

    [Header("Developer Tools")]
    [SerializeField] public bool AutoSaveData = false;

    private GameData gameData;
    private SettingsData settingsData;

    public static DataPersistenceManager DataManager { get; private set; }

    private List<IDataPersistence> dataPersistenceObjects;
    private List<ISettingsPersistence> settingsPersistenceObjects;

    private FileDataHandler dataHandler;
    private FileDataHandler settingsHandler;


    private string selectedProfileId = "";

    private void Awake() {
        if(DataManager != null){
            //Debug.LogError("Found more than one Data Persistence Manager in this scene. Destroying the newest one.");
            Destroy(this.gameObject);
            return;
        }
        DataManager = this;
        DontDestroyOnLoad(this.gameObject);

        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        settingsHandler = new FileDataHandler(Application.persistentDataPath, "settings.json");

        selectedProfileId = dataHandler.GetMostRecentProfileId();
    }

    private void Start() {
        settingsPersistenceObjects = FindSettingsPersistenceObject();
    }

    private void OnEnable() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.sceneUnloaded += OnSceneUnloaded;
    }

    private void OnDisable() {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    public void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void OnSceneUnloaded(Scene scene){
        if(AutoSaveData){
            SaveGame();
            SaveSettings();
        }
    }

    public void ChangeSelectedProfileId(string newProfileId){
        selectedProfileId = newProfileId;

        LoadGame();
    }

    public void NewSettings(){
        settingsData = new SettingsData();
    }

    public void NewGame(){
        gameData = new GameData();
    }

    public void SaveSettings(){
        foreach(ISettingsPersistence settingsPersistenceObject in settingsPersistenceObjects){
            settingsPersistenceObject.SaveData(ref settingsData);
        }

        settingsHandler.SaveSettings(settingsData);
    }

    public void SaveGame(){
        if(gameData == null){
            Debug.LogWarning("No data was found. There needs to be a game started to save.");
            return;
        }
        
        foreach(IDataPersistence dataPersistenceObject in dataPersistenceObjects){
            dataPersistenceObject.SaveData(ref gameData);
        }
        //Debug.Log("saved, first and second card are " + gameData.Deck[0].name + ", " + gameData.Deck[1].name);

        gameData.lastSaved = System.DateTime.Now.ToBinary();

        dataHandler.Save(gameData, selectedProfileId);

    }

    public void LoadSettings(){
        settingsData = settingsHandler.LoadSettings();

        settingsPersistenceObjects = FindSettingsPersistenceObject();

        if(settingsData == null){
            Debug.Log("no settings data was found. Creating new settings file.");
            NewSettings();
        }

        foreach(ISettingsPersistence settingsPersistenceObject in settingsPersistenceObjects){
            settingsPersistenceObject.LoadData(settingsData);
        }
    }

    public void LoadGame(){
        gameData = dataHandler.Load(selectedProfileId);

        if(gameData == null){
            Debug.Log("No data was found. There needs to be a save to load.");
            return;
        }

        foreach(IDataPersistence dataPersistenceObject in dataPersistenceObjects){
            dataPersistenceObject.LoadData(gameData);
        }
        //Debug.Log("loaded, first and second card are " + gameData.Deck);
    }

    private void OnApplicationQuit() {
        if(AutoSaveData){
            SaveGame();
            SaveSettings();
        }
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects(){
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }

    private List<ISettingsPersistence> FindSettingsPersistenceObject(){
        IEnumerable<ISettingsPersistence> settingsPersistenceObjects = FindObjectsOfType<MonoBehaviour>(true).OfType<ISettingsPersistence>();
        return new List<ISettingsPersistence>(settingsPersistenceObjects);
    }

    public bool HasGameData(){
        return gameData != null;
    }

    public Dictionary<string, GameData> GetAllProfilesGameData(){
        return dataHandler.LoadAllProfiles();
    }
}
