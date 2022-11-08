using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // den transform som playerMove ska Mova till.
    public Transform targetTransform;
    // Grundspeeden. Är serialized så att man kan se i editorn.
    [SerializeField]
    private float speed = 5f;

    // clone är det gameobject som kommer att instansiatas, är serialized så att man kan se i editorn. PlayerClone är prefaben.
    [SerializeField]
    private GameObject clone;
    public GameObject PlayerClone;

    // Menyn som finns som child i playern. 
    [SerializeField]
    private SelectDestination playerMenu; 

    // den bool som bestämmer om spelaren ska börja röra sig. 
    public bool startMove = false;

    private void Awake()
    {
        playerMenu = GetComponentInChildren<SelectDestination>();
    }
    // 
    void Update()
    {
        if (startMove)
        {
            Move();
            if (transform.position == targetTransform.position)
            {
                startMove = false; 
            }
        }
    }
    // Här ändrar jag speed från slidern i menyn. Jag måste först sätta speed till 5 (alltså grundspeed) så att inte speed blir expenentionellt högre.
    public void ChangeSpeed(float value)
    {
        speed = 5; 
        speed = speed * value;
    }

    // Testade lite olika sätt att röra på spelaren, men denna var den enklaste och den som funkade bäst för mig.
    public void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, Time.deltaTime * speed); 
    }
    // Instansiatear clonen. Jag vill att den inte ska ha någon parent då det gjorde att Grabbable in ville funka bra.
    public void InstansiateTarget()
    {
        clone = Instantiate(PlayerClone, transform.position + new Vector3(-1, 0, 1), transform.rotation, transform.parent);
    }

    // Stänger av clonens funktionalitet
    public void LockTargetPosition()
    {
        targetTransform = clone.transform;
        clone.GetComponent<Rigidbody>().useGravity = false;
        clone.GetComponent<Collider>().enabled = false;
    }
    // Destroyar clonen och sätter på menyn igen.
    public void ResetTarget()
    {
        Destroy(clone); 
        playerMenu.gameObject.SetActive(true); 
    }
}
