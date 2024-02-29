using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour, IDataPersistence
{
    public static Timer timer;
    public float time = 0;

    public void LoadData(GameData data)
    {
        time = data.runTime;
    }

    public void SaveData(GameData data)
    {
        data.runTime = time;
    }


    void Start()
    {
        if(timer != null){
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        timer = this;
    }

    // Update is called once per frame
    void Update()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "Map" || sceneName == "Combat") time += Time.deltaTime;
        //if (sceneName == "Main Menu") time = 0;
    }
}
