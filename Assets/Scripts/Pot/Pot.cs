using System;
using UnityEngine;

public class Pot : InteractionObject
{

    [SerializeField] private PotState _potState;
    [SerializeField] private GameObject _recipeCanvas;
    
    /*private new void OnEnable()
    {
        base.OnEnable();
    }*/
}
