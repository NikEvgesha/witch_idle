using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moneyy : MonoBehaviour
{
    public int amount;
    public float boundY => collider.bounds.size.y;
    public Collider collider;
}
