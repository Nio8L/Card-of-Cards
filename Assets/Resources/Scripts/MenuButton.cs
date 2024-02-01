using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MenuButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TooltipSystem.tooltipSystem.tooltip.gameObject.SetActive(false);
    }
    public void ReturnToMenu()
    {
        SceneManager.LoadSceneAsync("Main Menu");
        DataPersistenceManager.DataManager.DeleteMostRecentProfileData();
    }
}
