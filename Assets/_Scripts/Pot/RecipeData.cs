using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WitchScripts/RecipeData")]
public class RecipeData : ScriptableObject
{
    [SerializeField] private string _potionName;
    [SerializeField] private float _cookTime;
    [SerializeField] private List<InventoryItem> _ingredients;
    [SerializeField] private int _cost;
    [SerializeField] private RecipeState _status;
    [SerializeField] private Sprite _potionIcon;
    [SerializeField] private InventoryItem _potionItem;
    [SerializeField] private int _needLevelToUse;


    public string GetPotionName() { return _potionName; }
    public int GetCost() { return _cost;}
    public Sprite GetIcon() { return _potionIcon; }
    public RecipeState GetState() { return _status; }

    public float GetCookTime() { return _cookTime;}
    public int GetOpenLevel() { return _needLevelToUse; }

   /* public void GetIngredients(out List<PlantTypes> items)
    {
        items = new List<PlantTypes>();
        foreach (var item in _ingredients)
        {
            items.Add(item);
        }
       
    }*/
   public InventoryItem GetItem()
    {
        return _potionItem;
    }

    public List<PlantTypes> GetIngredientsTypes()
    {
        var items = new List<PlantTypes>();
        foreach (var item in _ingredients)
        {
            items.Add(item.GetPlantType());
        }
        return items;
    }

    public List<InventoryItem> GetIngredients()
    {
        var items = new List<InventoryItem>();
        foreach (var item in _ingredients)
        {
            items.Add(item);
        }
        return items;
    }
}
