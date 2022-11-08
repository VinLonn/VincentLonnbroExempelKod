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

    // Detta script callas i kanpparna. btnEffect byter vad som ska h�nda. jag skulle kunna ha haft en bool men om jag vill l�gga till fler metoder s� kan jag lika g�rna ha ints fr�n b�rjan
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
    // Denna metod instansiatear det target som spelaren kommer r�ra sig mot. Den �ndrar ocks� btnEffect. 
    private void ChooseDestination()
    {
        playerMove.InstansiateTarget(); 
        btnEffect++; 
    }

    // H�r s�tts playerMove.targetTransform till clonens transform och resettar btnEffect f�r att senare kunna k�ra om allt igen.
    private void LockDestination()
    {
        playerMove.LockTargetPosition();
        btnEffect = 1;
        this.gameObject.SetActive(false);
    
    }
}
