using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ChangeSpeedPlayer : MonoBehaviour
{
    // PlayerMove är inte spelaren, utan de som ska simuleras.
    [SerializeField]
    private PlayerMove playerMove;

    // Denna slider är den som finns i menyn.
    [SerializeField]
    private Slider slider; 

    // den float som kommer användas för att ändra playerMove.speed.
    private float speed;

    private void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
        slider = GetComponentInChildren<Slider>();
    }

    public void ChangeSpeed()
    {
        // Av någon anledning så blev speed oändlig när jag använde slider.value i ChangeSpeed metoden, så jag sätter speed som slider.value.
        speed = slider.value;
        playerMove.ChangeSpeed(speed);  

    }
}
