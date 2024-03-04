using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stackable : MonoBehaviour
{
    public CustomerNeed itemType;
    public Collider collider;
    public float colliderBound => collider.bounds.size.y;

    public int income;

   
}
