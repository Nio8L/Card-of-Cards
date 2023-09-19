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
    [SerializeField] private GameMenu gameMenu;
    [SerializeField] private MapMenu mapMenu;

    [Header("Menu Operational Stuff")]
    [SerializeField] private Slider audioSlider;
    [SerializeField] private TextMeshProUGUI audioText;

    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicSliderText;

    public void SaveData(ref SettingsData data){
        if(audioSlider != null){
            data.audioLevel = (int)audioSlider.value;
            data.musicLevel = (int)musicSlider.value;
        }
    }

    public void LoadData(SettingsData data){
        audioSlider.value = data.audioLevel;
        musicSlider.value = data.musicLevel;
    }

    public void OnBackClick(){
        SoundManager.soundManager.Play("ButtonClick");
        DataPersistenceManager.DataManager.SaveSettings();
        if(mainMenu != null){
            mainMenu.ActivateMenu();
        }else if(gameMenu != null){
            gameMenu.menuButtons.SetActive(!gameMenu.menuButtons.activeSelf);
        }else{
            mapMenu.menuButtons.SetActive(!mapMenu.menuButtons.activeSelf);
        }
        DeactivateMenu();
    }

    public void ActivateMenu(){
        gameObject.SetActive(true);
        DataPersistenceManager.DataManager.LoadSettings();
    }

    public void DeactivateMenu(){
        gameObject.SetActive(false);
    }

    public void SliderChange(){
        audioText.text = "SOUND LEVEL: " + audioSlider.value.ToString() + "%";
        DataPersistenceManager.DataManager.audioLevel = audioSlider.value;
        SoundManager.soundManager.UpdateVolume(audioSlider.value / 100);
    }

    public void MusicSliderChange(){
        musicSliderText.text = "MUSIC LEVEL: " + musicSlider.value.ToString() + "%";
        DataPersistenceManager.DataManager.musicLevel = musicSlider.value;
        SoundManager.soundManager.UpdateMusicVolume(musicSlider.value / 100);
    }
}
