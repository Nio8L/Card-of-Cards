using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuEyeAnimation : MonoBehaviour, IPointerClickHandler
{
    public Animator animator;
    public MapWorld showcase;

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

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("cheating");
        ScenePersistenceManager.scenePersistence.worlds.Clear();
        ScenePersistenceManager.scenePersistence.worlds.Add(showcase);
    }
}
