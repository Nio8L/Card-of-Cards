using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuEyeAnimation : MonoBehaviour
{
    public Animator animator;

    public void FinishInitialize(){
        LeanTween.cancelAll();
        animator.SetBool("Blink", true);
        Blink(0);
    }

    public void Blink(float beginAfter){

        LeanTween.delayedCall(beginAfter, () => {
            float animateAfter = Random.Range(3, 8);

            //Debug.Log(animateAfter);
            animator.Play("Base Layer.MenuEyeBlink", 0, 0f);
            Blink(animateAfter);
        });
    }

    private void OnDisable() {
        LeanTween.cancelAll();
    }
}
