using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    
    GameObject card;

    int smallestIndex;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {  
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPosition.z = 0;

        int numOfCards = CombatManager.combatManager.deck.cardsInHand.Count;
        
        for(int i = 0; i < numOfCards; i++){
            if (CombatManager.combatManager.deck.cardsInHand.Count > 0)
            {
                double distanceXLeft = mouseWorldPosition.x - CombatManager.combatManager.deck.cardsInHand[0].transform.position.x;
                double distanceXRight = mouseWorldPosition.x - CombatManager.combatManager.deck.cardsInHand[^1].transform.position.x;

                if(i != smallestIndex || mouseWorldPosition.y > -3 || mouseWorldPosition.y < -5 || distanceXLeft < -1.3 || distanceXRight > 1.3){
                    CombatManager.combatManager.deck.cardsInHand[i].GetComponent<CardInHand>().Unselect(CombatManager.combatManager.deck.cardsInHand[i].transform);
                }
            }
            
        }

        if (CombatManager.combatManager.deck.cardsInHand.Count > 0)
        {
            double distanceXLeft = mouseWorldPosition.x - CombatManager.combatManager.deck.cardsInHand[0].transform.position.x;
            double distanceXRight = mouseWorldPosition.x - CombatManager.combatManager.deck.cardsInHand[^1].transform.position.x;
            
            if(distanceXLeft < - 1.3 || distanceXRight > 1.3){
                return;
            }
        }

        if(mouseWorldPosition.y > -3 || mouseWorldPosition.y < -5){
            return;
        }else{
            double[] distance = new double[numOfCards];
            for(int i = 0; i < numOfCards; i++){
                card = CombatManager.combatManager.deck.cardsInHand[i];

                distance[i] = Vector2.Distance(mouseWorldPosition, card.transform.position);
            }
            
            smallestIndex = Array.IndexOf(distance, distance.Min());
            CombatManager.combatManager.deck.cardsInHand[smallestIndex].GetComponent<CardInHand>().GetOnTop(CombatManager.combatManager.deck.cardsInHand[smallestIndex].transform);
        }


        

        //Debug.Log(CombatManager.combatManager.deck.cardsInHandAsCards[smallestIndex].name);
    }
}
