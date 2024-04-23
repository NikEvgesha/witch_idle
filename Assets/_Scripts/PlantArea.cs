using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantArea : MonoBehaviour
{
    public event Action PlayerOnPlantArea;
    public event Action PlayerOffPlantArea;

    private void OnTriggerEnter(Collider other)
    {
        PlayerOnPlantArea?.Invoke();
    }
    public void PlayerOnArea()
    {
        
        PlayerOnPlantArea?.Invoke();
    }

    public void PlayerOffArea()
    {
        PlayerOffPlantArea?.Invoke();
    }
}
