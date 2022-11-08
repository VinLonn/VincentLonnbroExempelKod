using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class ChangeSpeedPlayer : MonoBehaviour
{
    // PlayerMove �r inte spelaren, utan de som ska simuleras.
    [SerializeField]
    private PlayerMove playerMove;

    // Denna slider �r den som finns i menyn.
    [SerializeField]
    private Slider slider; 

    // den float som kommer anv�ndas f�r att �ndra playerMove.speed.
    private float speed;

    private void Awake()
    {
        playerMove = GetComponentInParent<PlayerMove>();
        slider = GetComponentInChildren<Slider>();
    }

    public void ChangeSpeed()
    {
        // Av n�gon anledning s� blev speed o�ndlig n�r jag anv�nde slider.value i ChangeSpeed metoden, s� jag s�tter speed som slider.value.
        speed = slider.value;
        playerMove.ChangeSpeed(speed);  

    }
}
