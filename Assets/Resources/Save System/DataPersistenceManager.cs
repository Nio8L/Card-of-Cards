using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class DataPersistenceManager : MonoBehaviour
{
    [Header("File Storage Config")]

    [SerializeField] private string fileName;

    private GameData gameData;

    public static DataPersistenceManager DataManager { get; private set; }

    private List<IDataPersistence> dataPersistenceObjects;
    private FileDataHandler dataHandler;

    private void Awake() {
        if(DataManager != null){
            Debug.LogError("Found more than one Data Persistence Manager in this scene!");
        }
        DataManager = this;
    }

    private void Start() {
        dataHandler = new FileDataHandler(Application.persistentDataPath, fileName);
        dataPersistenceObjects = FindAllDataPersistenceObjects();
        LoadGame();
    }

    public void NewGame(){
        gameData = new GameData();
    }

    public void SaveGame(){
        foreach(IDataPersistence dataPersistenceObject in dataPersistenceObjects){
            dataPersistenceObject.SaveData(ref gameData);
        }
        //Debug.Log("saved, first and second card are " + gameData.Deck[0].name + ", " + gameData.Deck[1].name);

        dataHandler.Save(gameData);
    }

    public void LoadGame(){
        gameData = dataHandler.Load();

        if(gameData == null){
            Debug.Log("No data was found. Creating new Save");
            NewGame();
        }

        foreach(IDataPersistence dataPersistenceObject in dataPersistenceObjects){
            dataPersistenceObject.LoadData(gameData);
        }
        //Debug.Log("loaded, first and second card are " + gameData.Deck);
    }

    private void OnApplicationQuit() {
        SaveGame();
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects(){
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsOfType<MonoBehaviour>().OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}
