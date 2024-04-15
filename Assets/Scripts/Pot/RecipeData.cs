using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WitchScripts/RecipeData")]
public class RecipeData : ScriptableObject
{
    [SerializeField] private string _recipeName;
    [SerializeField] private float _cookTime;
    [SerializeField] private List<PlantTypes> _ingredients;
    [SerializeField] private int _cost;
    [SerializeField] private RecipeState _status;
}
