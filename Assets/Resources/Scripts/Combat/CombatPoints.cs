using UnityEngine;
using UnityEngine.SceneManagement;

public class CombatPoints : MonoBehaviour, IDataPersistence
{
    public static CombatPoints combatPoints;

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

        EventManager.CombatEnd += AddPoint;
    }

    

    private void OnDestroy() {
        EventManager.CombatEnd -= AddPoint;
    }

    public void AddPoint(){
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

    void OnEnable() {
        SceneManager.sceneLoaded += OnSceneChange;
    }

    void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneChange;
    }

    private void OnSceneChange(Scene scene, LoadSceneMode mode){
        if(SceneManager.GetActiveScene().name == "Main Menu"){
            Destroy(gameObject);
        }
    }

    public void LoadData(GameData data)
    {
        ResetPoints();
        AddPoints(data.combatPoints);
    }

    public void SaveData(GameData data)
    {
        data.combatPoints = combatPointsCount;
    }
}
