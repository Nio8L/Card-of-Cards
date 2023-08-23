using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapMenu : MonoBehaviour
{
    public GameObject menuButtons;
    public GameObject stuffToDisable;

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.Escape)){
            menuButtons.SetActive(!menuButtons.activeSelf);
            stuffToDisable.SetActive(!stuffToDisable.activeSelf);
        }
    }

    public void OnClickMainMenu(){
        menuButtons.SetActive(!menuButtons.activeSelf);
        SceneManager.LoadSceneAsync("Main Menu");
    }

    public void ResumeGame(){
        menuButtons.SetActive(!menuButtons.activeSelf);
        stuffToDisable.SetActive(!stuffToDisable.activeSelf);
    }

    public void OnClickBackToGame(){
        menuButtons.SetActive(!menuButtons.activeSelf);
        SceneManager.LoadSceneAsync("SampleScene");
    }

    public void SaveGame(){
        DataPersistenceManager.DataManager.SaveGame();
    }
}
