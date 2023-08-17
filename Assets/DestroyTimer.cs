using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float destroyAfter;
    public GameObject spawnOnDeath;
    void Update()
    {
        destroyAfter -= Time.deltaTime;
        if (destroyAfter <= 0)
        {
            Destroy(gameObject);
            if (spawnOnDeath != null) Instantiate(spawnOnDeath, transform.position, Quaternion.identity);
        }
    }
}
