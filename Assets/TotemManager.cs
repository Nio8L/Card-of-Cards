using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class TotemManager : MonoBehaviour
{
    public static TotemManager totemManager;
    public List<Totem> totems = new List<Totem>();

    static Transform parentUIObject;
    void Awake(){
        // Destroy this object if a TotemManager already exists
        if(totemManager != null){
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
        totemManager = this;
    }
    public static void LoadScene(){
        string sceneName = SceneManager.GetActiveScene().name;
        parentUIObject = GameObject.Find("TotemMenu").transform;

        UpdateIcons();
        
        if (sceneName == "Combat"){
            SetupTotems();
        }else{
            UnsubscribeTotems();
        }
    }
    public static void UpdateIcons(){
        for (int i = 0; i < 3; i++){
            TotemDisplay targetDisplay = parentUIObject.GetChild(i).GetComponent<TotemDisplay>();
            if (i < totemManager.totems.Count){
                // Update all displays with the correct totem
                targetDisplay.UpdateDisplay(totemManager.totems[i]);
            }else{
                // Clear the display
                targetDisplay.UpdateDisplay(null);
            }
        }
    }   

    public static void SetupTotems(){
        //Debug.Log("SetupTotems");
        for (int i = 0; i < totemManager.totems.Count; i++){
            Totem thisTotem = totemManager.totems[i];
            thisTotem.Setup();
        }
    }

    public static void UnsubscribeTotems(){
        //Debug.Log("UnsubscribeTotems");
        for (int i = 0; i < totemManager.totems.Count; i++){
            Totem thisTotem = totemManager.totems[i];
            thisTotem.Unsubscribe();
        }
    }

    public static void UseActive(int totemIndex){
        if (totemIndex >= totemManager.totems.Count) return;
        if (CombatManager.combatManager == null)     return;

        Totem targetTotem =  totemManager.totems[totemIndex];
        targetTotem.Active();
        targetTotem.Unsubscribe();
        totemManager.totems.RemoveAt(totemIndex);
        UpdateIcons();
    }
}
