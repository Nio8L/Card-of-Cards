using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    public static Timer timer;
    public float time;
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
        if (sceneName == "MainMenu") time = 0;
    }
}
