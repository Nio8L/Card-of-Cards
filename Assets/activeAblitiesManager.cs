using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class activeAblitiesManager : MonoBehaviour
{
    List<Sigil> activatedActiveSigils = new List<Sigil>();

    CombatManager combatManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1)) 
        {
            int guess = TryToFindSlot();
            if (guess == -1) return;

            bool playerCard;
            bool benched;
            playerCard = guess >= 8 ? true : false;
            guess %= 8;
            benched = guess >=4 ? true : false;
            guess %= 4;
        }
    }

    int TryToFindSlot()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, transform.forward, 100);
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.collider.tag == "BenchSlot")
            {
                return hit.transform.GetComponent<BenchSlot>().slot + 4 + (hit.transform.GetComponent<BenchSlot>().playerSlot  ? 8 : 0);
            }
            else if (hit.collider.tag == "CardSlot")
            {
                return hit.transform.GetComponent<CardSlot>().slot + (hit.transform.GetComponent<CardSlot>().playerSlot ? 8 : 0);
            }
        }
        return -1;
    }
}
