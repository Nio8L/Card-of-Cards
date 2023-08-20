using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LostSoulVisuals : MonoBehaviour
{
    // Start is called before the first frame update
    float radius;
    public float radiusGrowthPerSecond;
    public float maxRadius;
    float growth = 1;
    public float angle;
    public float angleIncreasePerSecond;
    Vector3 startPos;
    public bool primaryHeart;
    public GameObject soulParticles;
    public GameObject lifeMark;


    void Start()
    {
        startPos = transform.position;
        if (primaryHeart)
        {

            Instantiate(soulParticles, transform.position, Quaternion.identity);
            //Instantiate(lifeMark, transform.position, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        radius += radiusGrowthPerSecond * growth * Time.deltaTime;
        angle += angleIncreasePerSecond * Time.deltaTime;

        if (radius >= maxRadius)
        {
            growth = -1;
        }

        float xDist = Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
        float yDist = Mathf.Sin(angle * Mathf.Deg2Rad) * radius;

        transform.position = startPos + new Vector3(xDist, yDist, 0);
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}
