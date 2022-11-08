using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHeadScript : MonoBehaviour
{
    // Script som gör att spelarmenyn kollar mot spelarens huvud. Använder sig av CenterEyeAnchor. 
    public Transform cameraPlayer;


    private void Update()
    {
        transform.LookAt(cameraPlayer); 
    }

}
