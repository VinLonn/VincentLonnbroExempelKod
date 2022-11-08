using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OVR;

public class PlayerManager : MonoBehaviour
{
    // Just nu anv�nder jag Arrays f�r att hantera alla Players och deras spawnpoints. Tanken �r att bygga vidare s� att man fritt kan spawna in dessa players.
    // F�r att g�ra det s� borde jag g�ra om arrays till listor.
    [SerializeField]
    private PlayerMove[] playersArray;
    [SerializeField]
    private Vector3[] playersArrayOrigin;

    private void Awake()
    {
        // PlayerArray s�tts i Editorn. H�r tar jag deras position s� att man sen kan resetta.
        playersArrayOrigin = new Vector3[playersArray.Length];
        for (int i = 0; i < playersArray.Length; i++)
        {
            playersArrayOrigin[i] = playersArray[i].transform.position;
        }
    }
    // I denna metod g�r jag igenom alla spelare och g�r om en bool som g�r att en metod k�rs i Update i alla PlayerMove.
    // Jag kollar ocks� om de har ett target att r�ra sig mot.
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
    // Resetar alla spelarnas positioner samt k�r reset metoden. 
    public void ResetPlayers()
    {
        for (int i = 0; i < playersArray.Length; i++)
        {
            playersArray[i].transform.position = playersArrayOrigin[i];
            playersArray[i].ResetTarget(); 
        }
    }
    
}
