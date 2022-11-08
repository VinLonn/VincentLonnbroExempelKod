using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;

    // Timer �r f�r att se till att menyn inte s�tts av och p� medans knapptrycket g�rs. 
    // TimerLimit �r hur l�nge man ska v�nta innan man kan s�tta av/p� menyn.
    private float timer;
    private float timerLimit;

    private bool menuState; 
    private void Awake()
    {
        timer = 0;
        timerLimit = 0.6f;
        menuState = menu.activeSelf; 
    }

    void Update()
    {
        // Detta �r knapp B och Y
        // F�r att hantera menyn p� handen 
        timer += Time.deltaTime;
        if (OVRInput.Get(OVRInput.Button.Two))
        {

            if (timer >= timerLimit)
            {
                menuState = !menuState;
                menu.SetActive(menuState);
                timer = 0;
            }
        }
    }
}
