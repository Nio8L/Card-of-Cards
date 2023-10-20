using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateSigil : MonoBehaviour
{
    float maxTime = 0.5f;
    float t = -1;
    float scale = 1;
    bool updating = false;
    void Update()
    {
        if (t > -maxTime / 2)
        {
            t -= Time.deltaTime;
            if (t > 0) scale += 2 * Time.deltaTime;
            else scale -= 2 * Time.deltaTime;
            transform.localScale = Vector3.one * scale;
        }
        else if (updating)
        {
            updating = false;
            scale = 1;
            transform.localScale = Vector3.one;
        }

    }

    public void StartAnimation()
    {
        updating = true;
        t = maxTime / 2;
    }
}
