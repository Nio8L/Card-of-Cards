using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatCamera : MonoBehaviour
{
    Camera mainCamera;
    float shakeTime;
    public float defaultShakeTime;
    public float defaultShakeAmount;
    float shakeAmount;

    Vector3 defaultPosition;

    public bool TRIGGER;
    void Start()
    {
        mainCamera = GetComponent<Camera>();
        defaultPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {

        if (shakeTime > 0) {
            Vector3 newPos = Random.insideUnitSphere * shakeAmount;
            newPos.z = -10;
            transform.localPosition = newPos;
            shakeTime -= Time.deltaTime;
        
        } else {
            shakeTime = 0f;
            transform.position = defaultPosition;
        }
    }

    public void Shake(){
        if (!DataPersistenceManager.DataManager.screenShake) return;
        
        shakeTime = defaultShakeTime;
        shakeAmount = defaultShakeAmount;
    }
}
