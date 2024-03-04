using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    private Vector3 mouseStart;

    public float speed;
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouseStart = Input.mousePosition;
            
        }

        if (Input.GetMouseButton(0))
        {
            transform.Rotate(Vector3.up, (mouseStart - Input.mousePosition).x*Time.deltaTime*speed);

        }
    }
}
