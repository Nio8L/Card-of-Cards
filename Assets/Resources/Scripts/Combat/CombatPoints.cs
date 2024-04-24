using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatPoints : MonoBehaviour, IDataPersistence
{
    public static CombatPoints combatPoints;

    public bool disabled = false;

    public int combatPointsCount = 0;
    private readonly int combatPointsMax = 3;

    public SpriteRenderer[] pointCircles = new SpriteRenderer[3];

    private void Awake() {
        if(combatPoints != null){
            Destroy(gameObject);
            return;
        }

        combatPoints = this;
        DontDestroyOnLoad(gameObject);

        if(disabled) return;

        HideCircles();

        EventManager.CombatEnd += AddPoint;
    }

    

    private void OnDestroy() {
        EventManager.CombatEnd -= AddPoint;
    }

    public void AddPoint(){
        if(CombatManager.combatManager == null) return;

        if(CombatManager.combatManager.enemy.huntAI) return;
        
        if(combatPointsCount < combatPointsMax){
            combatPointsCount++;
        }

        pointCircles[combatPointsCount - 1].color = Color.red;
    }

    public void AddPoints(int numberOfPoints){
        for(int i = 0; i < numberOfPoints; i++){
            AddPoint();
        }
    }
    
    public void LoadPoints(int numberOfPoints){
        combatPointsCount = numberOfPoints;

        for(int i = 0; i < combatPointsCount; i++){
            if (pointCircles[i] != null)
            {
                pointCircles[i].color = Color.red;
            }
        }
    }

    public void RemovePoint(){
        if(combatPointsCount > 0){
            combatPointsCount--;
        }

        if (pointCircles[combatPointsCount] != null)
        {
            pointCircles[combatPointsCount].color = Color.white;
        }
    }

    public void RemovePoints(int numberOfPoints){
        for(int i = 0; i < numberOfPoints; i++){
            RemovePoint();
        }
    }

    public void ResetPoints(){
        RemovePoints(combatPointsMax);
    }

    public void HideCircles(){
        bool setActive = !(SceneManager.GetActiveScene().name == "Main Menu");
        
        for(int i = 0; i < 3; i++){
            pointCircles[i].gameObject.SetActive(setActive);
        }
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneChange;
    }

    void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneChange;
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode){
        if(disabled) return;
        
        HideCircles();
    }

    public void LoadData(GameData data)
    {
        if(SceneManager.GetActiveScene().name == "Main Menu") return;

        ResetPoints();
        LoadPoints(data.combatPoints);
    }

    public void SaveData(GameData data)
    {
        if(SceneManager.GetActiveScene().name == "Main Menu") return;

        data.combatPoints = combatPointsCount;
    }
}
