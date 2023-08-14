using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SaveSlot : MonoBehaviour
{
    [Header("Profile")]
    [SerializeField] private string profileId = "";

    [Header("Content")]
    [SerializeField] private GameObject noDataContent;
    [SerializeField] private GameObject hasDataContent;
    [SerializeField] private TextMeshProUGUI deckSizeText;

    private Button saveSlotButton;

    private void Awake() {
        saveSlotButton = GetComponent<Button>();
    }

    public void SetData(GameData data){
        if(data == null){
            noDataContent.SetActive(true);
            hasDataContent.SetActive(false);
        }else{
            noDataContent.SetActive(false);
            hasDataContent.SetActive(true);
            deckSizeText.text = "DECK SIZE: " + data.cardNames.Count.ToString();
        }
    }

    public string GetProfileId(){
        return profileId;
    }

    public void SetInteractable(bool interactable){
        saveSlotButton.interactable = interactable;
    }

}
