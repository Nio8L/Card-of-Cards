using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEyeAnimation : MonoBehaviour
{
    public Animator animator;

    public void FinishInitialize(){
        animator.SetBool("Blink", true);
        StartCoroutine(Blink());
    }

    public IEnumerator Blink(){
            animator.Play("Base Layer.MenuEyeBlink", 0, 0f);

            float animateAfter = Random.Range(3, 8);
            yield return new WaitForSeconds(animateAfter);

            StartCoroutine(Blink());
    }
}
