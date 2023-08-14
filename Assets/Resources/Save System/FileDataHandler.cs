using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using UnityEditor.Build.Content;

public class FileDataHandler
{
    private string dataDirPath = "";

    private string dataFileName = "";

    public FileDataHandler(string dataDirPath, string dataFileName){
        this.dataDirPath = dataDirPath;
        this.dataFileName = dataFileName;
    }

    public SettingsData LoadSettings(){
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        SettingsData loadedData = null;

        if(File.Exists(fullPath)){
            try{
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)){
                    using(StreamReader reader = new StreamReader(stream)){
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<SettingsData>(dataToLoad);
            }catch(Exception e){
                Debug.LogError("Error ocurred when trying to load from " + fullPath + "\n" + e);
            }
        }
        return loadedData;
    }

    public GameData Load(string profileId){
        if(profileId == null){
            return null;
        }
        
        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);

        GameData loadedData = null;

        if(File.Exists(fullPath)){
            try{
                string dataToLoad = "";
                using(FileStream stream = new FileStream(fullPath, FileMode.Open)){
                    using(StreamReader reader = new StreamReader(stream)){
                        dataToLoad = reader.ReadToEnd();
                    }
                }

                loadedData = JsonUtility.FromJson<GameData>(dataToLoad);
            }catch(Exception e){
                Debug.LogError("Error ocurred when trying to load from " + fullPath + "\n" + e);
            }
        }   
        return loadedData;
    }

    public void SaveSettings(SettingsData data){
        string fullPath = Path.Combine(dataDirPath, dataFileName);

        try{
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create)){
                using(StreamWriter writer = new StreamWriter(stream)){
                    writer.Write(dataToStore);
                }
            }
        }catch(Exception e){
            Debug.LogError("Error occured when trying to save settings data to " + fullPath + "\n" + e);
        }
    }

    public void Save(GameData data, string profileId){
        if(profileId == null){
            Debug.Log("no profileID");
            return;
        }

        string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);

        try{
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath));

            string dataToStore = JsonUtility.ToJson(data);

            using(FileStream stream = new FileStream(fullPath, FileMode.Create)){
                using(StreamWriter writer = new StreamWriter(stream)){
                    writer.Write(dataToStore);
                }
            }
        }catch(Exception e){
            Debug.LogError("Error occured when trying to save game data to " + fullPath + "\n" + e);
        }
    }

    public Dictionary<string, GameData> LoadAllProfiles(){
        Dictionary<string, GameData> profileDictionary = new();

        IEnumerable<DirectoryInfo> dirInfos = new DirectoryInfo(dataDirPath).EnumerateDirectories();

        foreach(DirectoryInfo dirInfo in dirInfos){
            string profileId = dirInfo.Name;

            string fullPath = Path.Combine(dataDirPath, profileId, dataFileName);
            if(!File.Exists(fullPath)){
                Debug.LogWarning("Skipping directory because it does not contain any data: " + profileId);
                continue;
            }

            GameData profileData = Load(profileId);

            if(profileData != null){
                profileDictionary.Add(profileId, profileData);
            }else{
                Debug.LogError("Tried to load profile but something went wrong: " + profileId);
            }
        }

        return profileDictionary;
    }

    public string GetMostRecentProfileId(){
        string mostRecentProfileId = null;
        
        Dictionary<string, GameData> profilesGameData = LoadAllProfiles();

        foreach(KeyValuePair<string, GameData> pair in profilesGameData){
            string profileId = pair.Key;
            GameData gameData = pair.Value;

            if(gameData == null){
                continue;
            }
        
            if(mostRecentProfileId == null){
                mostRecentProfileId = profileId;
            }else{
                DateTime mostRecentDateTime = DateTime.FromBinary(profilesGameData[mostRecentProfileId].lastSaved);
                DateTime newDateTime = DateTime.FromBinary(gameData.lastSaved);

                if(newDateTime > mostRecentDateTime){
                    mostRecentProfileId = profileId;
                }
            }
        }
        return mostRecentProfileId;
    }
}
