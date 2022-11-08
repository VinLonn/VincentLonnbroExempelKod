using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // den transform som playerMove ska Mova till.
    public Transform targetTransform;
    // Grundspeeden. �r serialized s� att man kan se i editorn.
    [SerializeField]
    private float speed = 5f;

    // clone �r det gameobject som kommer att instansiatas, �r serialized s� att man kan se i editorn. PlayerClone �r prefaben.
    [SerializeField]
    private GameObject clone;
    public GameObject PlayerClone;

    // Menyn som finns som child i playern. 
    [SerializeField]
    private SelectDestination playerMenu; 

    // den bool som best�mmer om spelaren ska b�rja r�ra sig. 
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
    // H�r �ndrar jag speed fr�n slidern i menyn. Jag m�ste f�rst s�tta speed till 5 (allts� grundspeed) s� att inte speed blir expenentionellt h�gre.
    public void ChangeSpeed(float value)
    {
        speed = 5; 
        speed = speed * value;
    }

    // Testade lite olika s�tt att r�ra p� spelaren, men denna var den enklaste och den som funkade b�st f�r mig.
    public void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetTransform.position, Time.deltaTime * speed); 
    }
    // Instansiatear clonen. Jag vill att den inte ska ha n�gon parent d� det gjorde att Grabbable in ville funka bra.
    public void InstansiateTarget()
    {
        clone = Instantiate(PlayerClone, transform.position + new Vector3(-1, 0, 1), transform.rotation, transform.parent);
    }

    // St�nger av clonens funktionalitet
    public void LockTargetPosition()
    {
        targetTransform = clone.transform;
        clone.GetComponent<Rigidbody>().useGravity = false;
        clone.GetComponent<Collider>().enabled = false;
    }
    // Destroyar clonen och s�tter p� menyn igen.
    public void ResetTarget()
    {
        Destroy(clone); 
        playerMenu.gameObject.SetActive(true); 
    }
}
