using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Restsite : MonoBehaviour
{
    public Transform[] positionsForPickedCards;
    public int healValue;
    public GameObject firstMenu;
    public GameObject huntMenu;
    public GameObject background;

    DeckDisplay deckDisplay;
    CardDisplay[] cardsPicked = new CardDisplay[3];
    Vector3[] oldPositions = new Vector3[3];

    public void Start() 
    {
        deckDisplay = GameObject.Find("DeckDisplayManager").GetComponent<DeckDisplay>();
        for (int i = 0; i < oldPositions.Length; i++)
        {
            oldPositions[i] = Vector3.one;
        }
    }

    public void StartMenu() 
    {
        background.SetActive(true);
        firstMenu.SetActive(true);
    }

    public void StartHunt() 
    {
        //nz kvo trqbva da ima tuka
    }

    public void Heal()
    {
        if (MapManager.mapDeck.playerHealth < 20 - healValue) MapManager.mapDeck.playerHealth += healValue;
        else MapManager.mapDeck.playerHealth = 20;

        MapManager.mapDeck.UpdateHPText();
        DataPersistenceManager.DataManager.currentCombatAI = null;
        firstMenu.SetActive(false);
    }

    public void openHuntMenu() 
    {
        firstMenu.SetActive(false);
        huntMenu.SetActive(true);
        deckDisplay.ShowDeck(3,500);
    }

    public void TryToPick(CardDisplay card) 
    {
        for (int i = 0; i < 3; i++)
        {
            if (cardsPicked[i] == card) 
            {
                cardsPicked[i].transform.position = oldPositions[i];
                oldPositions[i] = Vector3.one;
                cardsPicked[i] = null;
                Organize();
                break;
            }

            if (cardsPicked[i] == null)
            {
                cardsPicked[i] = card;
                break;
            }
        }
        UpdatePositions();
    }

    void Organize()
    {
        for (int i = 1; i < 3; i++)
        {
            if (cardsPicked[i] != null && cardsPicked[i - 1] == null)
            {
                cardsPicked[i - 1] = cardsPicked[i];
                oldPositions[i - 1] = oldPositions[i];

                cardsPicked[i] = null;
                oldPositions[i] = Vector3.one;
            }
        }
    }

    void UpdatePositions() 
    {
        for (int i = 0; i < 3; i++)
        {
            if (cardsPicked[i] != null) 
            {
                if (oldPositions[i] == Vector3.one) oldPositions[i] = cardsPicked[i].transform.position;
                cardsPicked[i].transform.position = positionsForPickedCards[i].position;
            }
        }
    }
}
