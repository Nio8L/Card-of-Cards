using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsMenu : MonoBehaviour, ISettingsPersistence
{
    [Header("Menu Navigation")]
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private GameMenu gameMenu;

    [Header("Menu Operational Stuff")]
    [SerializeField] private Slider audioSlider;
    [SerializeField] private TextMeshProUGUI audioText;
    [SerializeField] private Button audioButton;


    [SerializeField] private Slider musicSlider;
    [SerializeField] private TextMeshProUGUI musicSliderText;
    [SerializeField] private Button musicButton;

    [Header("Sprites")]
    public Sprite onImage;
    public Sprite offImage;

    public bool disableAudio = false;
    private bool disableMusic = false;


    public void SaveData(SettingsData data){
        data.audioLevel = (int)audioSlider.value;
        data.musicLevel = (int)musicSlider.value;

        data.disableAudio = disableAudio;
        data.disableMusic = disableMusic;
    }

    public void LoadData(SettingsData data){
        audioSlider.value = data.audioLevel;
        musicSlider.value = data.musicLevel;

        if(data.disableAudio){
            audioButton.image.sprite = offImage;
        }else{
            audioButton.image.sprite = onImage;
        }
        disableAudio = data.disableAudio;
        SoundManager.soundManager.disableAudio = data.disableAudio;
        
        if(data.disableMusic){
            musicButton.image.sprite = offImage;
        }else{
            musicButton.image.sprite = onImage;
        }
        disableMusic = data.disableMusic;
        SoundManager.soundManager.disableMusic = data.disableMusic;
        
    }

    public void OnBackClick(){
        SoundManager.soundManager.Play("ButtonClick");
        DataPersistenceManager.DataManager.SaveSettings();
        if(mainMenu != null){
            mainMenu.ActivateMenu();
        }else if(gameMenu != null){
            gameMenu.menuButtons.SetActive(!gameMenu.menuButtons.activeSelf);
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

    public void SoundToggleClick(){
        if(disableAudio){
            audioButton.image.sprite = onImage;
        }else{
            audioButton.image.sprite = offImage;
        }
        
        disableAudio = !disableAudio;

        SoundManager.soundManager.disableAudio = disableAudio;
    }

    public void MusicToggleClick(){
        if(disableMusic){
            musicButton.image.sprite = onImage;
        }else{
            musicButton.image.sprite = offImage;
        }

        disableMusic = !disableMusic;

        SoundManager.soundManager.disableMusic = disableMusic;
        SoundManager.soundManager.StopCurrentMusic();
        SoundManager.soundManager.PlayMusic(SoundManager.soundManager.currentMusic);
    }
}
