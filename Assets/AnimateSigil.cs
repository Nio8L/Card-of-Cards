using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSigil : MonoBehaviour
{
    float maxTime = 0.5f;
    float t = -1;
    float scale = 1;
 
    void Update()
    {
        if (t > -maxTime / 2)
        {
            t -= Time.deltaTime;
            if (t > 0) scale += 2 * Time.deltaTime;
            else scale -= 2 * Time.deltaTime;
            transform.localScale = Vector3.one * scale;
        }
    }

    public void StartAnimation()
    {
        t = maxTime / 2;
    }
}
