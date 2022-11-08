using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomColour : MonoBehaviour
{
    // Ett placeholder script. Egentligen s� vill jag att alla ska ha en separat textur med nummer, men nu har jag en random f�rg ist�llet. 
    private void Awake()
    {
        // Man kan inte �ndra material.color till en Color32, men man fritt byta mellan Color och Color32. 
        Color32 newColourRGB = new Color32((byte)Random.Range(1, 256), (byte)Random.Range(1, 256), (byte)Random.Range(1, 256), 255);
        Color newColour = newColourRGB;
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.color = newColour;  
    }
}
