using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForesightEffect : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 velocity;
    public float gravity;
    SpriteRenderer spriteRenderer;
    float alpha = 1;
    void Start()
    {
        velocity = new Vector3(Random.Range(-0.5f, 0.5f), 1, 0);
        velocity.Normalize();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //transform.position += velocity * Time.deltaTime;
        velocity.y -= gravity * Time.deltaTime;
        if (velocity.y < -0.1f)
        {
            alpha -= Time.deltaTime * 3;
            spriteRenderer.color = new Color(1, 1, 1, alpha);
        }
    }
}
