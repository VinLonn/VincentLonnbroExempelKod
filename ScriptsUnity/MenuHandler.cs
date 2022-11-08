using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject menu;

    // Timer är för att se till att menyn inte sätts av och på medans knapptrycket görs. 
    // TimerLimit är hur länge man ska vänta innan man kan sätta av/på menyn.
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
        // Detta är knapp B och Y
        // För att hantera menyn på handen 
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
