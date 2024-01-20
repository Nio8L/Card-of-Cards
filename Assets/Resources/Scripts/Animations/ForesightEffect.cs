using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForesightEffect : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AnimationUtilities.ChangeAlpha(transform, 0.4f, 0.6f, 0f);
    }
}
