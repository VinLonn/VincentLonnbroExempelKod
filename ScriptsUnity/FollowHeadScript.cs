using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHeadScript : MonoBehaviour
{
    // Script som g�r att spelarmenyn kollar mot spelarens huvud. Anv�nder sig av CenterEyeAnchor. 
    public Transform cameraPlayer;


    private void Update()
    {
        transform.LookAt(cameraPlayer); 
    }

}
