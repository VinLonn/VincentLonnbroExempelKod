using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedValueTxt : MonoBehaviour
{
    [SerializeField]
    private Slider slider;
    private Text textSpeed;

    // Detta script anv�nds bara f�r att �ndra texten som st�r under slidern.

    private void Awake()
    {
        slider = GetComponentInParent<Slider>();
        textSpeed = GetComponent<Text>();
    }

    // d� jag inte anv�nder whole numbers f�r slidern s� m�ste jag runda till 1 decimal f�r att valuen alltid ska f� plats. 
    // Egentligen s� borde jag nog ha tv� separata textelement f�r "Speed Value: " och slider.value.
    public void ChangeSpeedValue()
    {
        textSpeed.text = "Speed Value: " + decimal.Round(((decimal)slider.value), 1);
    }
}
