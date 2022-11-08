using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayCastPlayer : MonoBehaviour
{
	public Transform rayTransform;

	public LineRenderer linePointer = null;

	public float rayDrawDistance = 2.5f;

	void Update()
	{
		
		Ray ray = new Ray(rayTransform.position, rayTransform.forward);
		linePointer.SetPosition(0, ray.origin);
		linePointer.SetPosition(1, ray.origin + ray.direction * rayDrawDistance);
	}
}
