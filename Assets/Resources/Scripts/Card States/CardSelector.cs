using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    
    GameObject card;

    int smallestIndex;

    float upperLimit = -3;

    // Update is called once per frame
    void Update()
    {   
        if (Input.GetMouseButton(0) && CombatManager.combatManager.deck.selectedCard != null) return;
        int numOfCards = CombatManager.combatManager.deck.cardsInHand.Count;
       
        // If there are no cards return
        if (numOfCards < 1) return;

        if(CombatManager.combatManager.deck.hoveredCard != null){
            upperLimit = -1.8f;
        }else{
            upperLimit = -3;
        }

        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        float distanceXLeft = mouseWorldPosition.x - CombatManager.combatManager.deck.cardsInHand[0].transform.position.x;
        float distanceXRight = mouseWorldPosition.x - CombatManager.combatManager.deck.cardsInHand[^1].transform.position.x;

        for(int i = 0; i < numOfCards; i++){
            if(i != smallestIndex || mouseWorldPosition.y > upperLimit || distanceXLeft < -1.3 || distanceXRight > 1.3){
                GameObject card = CombatManager.combatManager.deck.cardsInHand[i];
                card.GetComponent<CardInHand>().Unselect(card.transform);
            }
        }

        if (CombatManager.combatManager.deck.cardsInHand.Count > 0)
        {
            
            if(distanceXLeft < - 1.3 || distanceXRight > 1.3){
                return;
            }
        }

        if(mouseWorldPosition.y > upperLimit || numOfCards == 0){
            return;
        }else{
            float[] distance = new float[numOfCards];
            for(int i = 0; i < numOfCards; i++){
                card = CombatManager.combatManager.deck.cardsInHand[i];

                float mod = 0;
                if (i == smallestIndex) mod = 0.5f;
                distance[i] = Vector2.Distance(mouseWorldPosition, card.transform.position) - mod;
            }
            
            smallestIndex = Array.IndexOf(distance, distance.Min());
            CombatManager.combatManager.deck.cardsInHand[smallestIndex].GetComponent<CardInHand>().GetOnTop(CombatManager.combatManager.deck.cardsInHand[smallestIndex].transform);
            //Debug.Log(CombatManager.combatManager.deck.cardsInHand[smallestIndex].GetComponent<CardInHand>().card.name);
        }


        

        //Debug.Log(CombatManager.combatManager.deck.cardsInHandAsCards[smallestIndex].name);
    }
}
