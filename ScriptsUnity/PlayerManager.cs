using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class PlayerManager : MonoBehaviour
{
    // Just nu använder jag Arrays för att hantera alla Players och deras spawnpoints. Tanken är att bygga vidare så att man fritt kan spawna in dessa players.
    // För att göra det så borde jag göra om arrays till listor.
    [SerializeField]
    private PlayerMove[] playersArray;
    [SerializeField]
    private Vector3[] playersArrayOrigin;

    private void Awake()
    {
        // PlayerArray sätts i Editorn. Här tar jag deras position så att man sen kan resetta.
        playersArrayOrigin = new Vector3[playersArray.Length];
        for (int i = 0; i < playersArray.Length; i++)
        {
            playersArrayOrigin[i] = playersArray[i].transform.position;
        }
    }
    // I denna metod går jag igenom alla spelare och gör om en bool som gör att en metod körs i Update i alla PlayerMove.
    // Jag kollar också om de har ett target att röra sig mot.
    public void StartMove()
    {
        for (int i = 0; i < playersArray.Length; i++)
        {
            if (playersArray[i].targetTransform != null)
            {
                playersArray[i].startMove = true;
            }
        }
    }
    // Resetar alla spelarnas positioner samt kör reset metoden. 
    public void ResetPlayers()
    {
        for (int i = 0; i < playersArray.Length; i++)
        {
            playersArray[i].transform.position = playersArrayOrigin[i];
            playersArray[i].ResetTarget(); 
        }
    }
    
}
