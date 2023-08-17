using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTimer : MonoBehaviour
{
    public float destroyAfter;

    void Update()
    {
        destroyAfter -= Time.deltaTime;
        if (destroyAfter <= 0) Destroy(gameObject);
    }
}
