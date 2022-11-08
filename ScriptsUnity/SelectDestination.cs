using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDestination : MonoBehaviour
{
    // PlayerMove som finns i parenten. 
    public PlayerMove playerMove;
    
    private int btnEffect = 1;
    private void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
    }

    // Detta script callas i kanpparna. btnEffect byter vad som ska hända. jag skulle kunna ha haft en bool men om jag vill lägga till fler metoder så kan jag lika gärna ha ints från början
    public void ButtonEvent()
    {
        if (btnEffect == 1)
        {
            ChooseDestination(); 
        }
        else if (btnEffect == 2)
        {
            LockDestination();  
        }
    }
    // Denna metod instansiatear det target som spelaren kommer röra sig mot. Den ändrar också btnEffect. 
    private void ChooseDestination()
    {
        playerMove.InstansiateTarget(); 
        btnEffect++; 
    }

    // Här sätts playerMove.targetTransform till clonens transform och resettar btnEffect för att senare kunna köra om allt igen.
    private void LockDestination()
    {
        playerMove.LockTargetPosition();
        btnEffect = 1;
        this.gameObject.SetActive(false);
    
    }
}
