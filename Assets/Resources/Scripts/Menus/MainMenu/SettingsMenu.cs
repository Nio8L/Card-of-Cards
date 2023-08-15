using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Diagnostics;

public class SettingsMenu : MonoBehaviour, ISettingsPersistence
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;

    [Header("Menu Operational Stuff")]
    [SerializeField] private Slider audioSlider;
    [SerializeField] private TextMeshProUGUI audioText;
    [SerializeField] private Toggle autoSaveSwitch;

    public void SaveData(ref SettingsData data){
        data.audioLevel = (int)audioSlider.value;
        data.autoSave = autoSaveSwitch.isOn;
    }

    public void LoadData(SettingsData data){
        audioSlider.value = data.audioLevel;
        autoSaveSwitch.isOn = data.autoSave;
    }

    public void OnBackClick(){
        DataPersistenceManager.DataManager.SaveSettings();
        mainMenu.ActivateMenu();
        DeactivateMenu();
    }

    public void ActivateMenu(){
        gameObject.SetActive(true);
        DataPersistenceManager.DataManager.LoadSettings();
    }

    public void DeactivateMenu(){
        gameObject.SetActive(false);
    }

    public void OnAutoSaveClick(){
        DataPersistenceManager.DataManager.AutoSaveData = !DataPersistenceManager.DataManager.AutoSaveData;
        
    }

    public void SliderChange(){
        audioText.text = "SOUND LEVEL: " + audioSlider.value.ToString() + "%";
        
    }
}