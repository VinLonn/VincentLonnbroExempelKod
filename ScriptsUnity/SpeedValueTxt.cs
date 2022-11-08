using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedValueTxt : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    private Text textSpeed;

    // Detta script används bara för att ändra texten som står under slidern.

    private void Awake()
    {
        slider = GetComponentInParent<Slider>();
        textSpeed = GetComponent<Text>();
    }

    // då jag inte använder whole numbers för slidern så måste jag runda till 1 decimal för att valuen alltid ska få plats. 
    // Egentligen så borde jag nog ha två separata textelement för "Speed Value: " och slider.value.
    public void ChangeSpeedValue()
    {
        textSpeed.text = "Speed Value: " + decimal.Round(((decimal)slider.value), 1);
    }
}
