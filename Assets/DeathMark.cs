using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathMark : MonoBehaviour
{
    // Start is called before the first frame update
    float dir = 1;
    public float maxDir = 30;
    float angle = 0;
    public float angleIncreasePerSecond;
    public float timeBetweenFlips = 0.5f;
    float currentTime;
    bool resting = true;
    float scale = 1;
    void Start()
    {
        currentTime = timeBetweenFlips;
    }

    // Update is called once per frame
    void Update()
    {
        if (resting)
        {
            currentTime -= Time.deltaTime;
        }

        if (currentTime <= 0f)
        {
            resting = false;
            angle += angleIncreasePerSecond * Time.deltaTime * dir;
            scale += Time.deltaTime;
            if ((angle >= maxDir && dir == 1) || (angle <= -maxDir && dir == -1))
            {
                resting = true;
                currentTime = timeBetweenFlips;
                dir *= -1;
            }
        }
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
        transform.localScale = Vector3.one * scale/2;
    }
}
