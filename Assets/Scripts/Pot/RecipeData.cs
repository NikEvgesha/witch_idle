using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(menuName = "WitchScripts/RecipeData")]
public class RecipeData : ScriptableObject, IInventoryItem
{
    [SerializeField] private string _potionName;
    [SerializeField] private float _cookTime;
    [SerializeField] private List<PlantTypes> _ingredients;
    [SerializeField] private int _cost;
    [SerializeField] private RecipeState _status;
    [SerializeField] private Sprite _potionIcon;
    

    public string GetPotionName() { return _potionName; }
    public int GetCost() { return _cost;}
    public Sprite GetIcon() { return _potionIcon; }
    public RecipeState GetState() { return _status; }

    public float GetCookTime() { return _cookTime;}

    public void GetIngredients(out List<PlantTypes> items)
    {
        items = new List<PlantTypes>();
        foreach (var item in _ingredients)
        {
            items.Add(item);
        }
       
    }
}
