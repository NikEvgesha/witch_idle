using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "WitchScripts/RecipeData")]
public class RecipeData : ScriptableObject, IComparable
{
    [SerializeField] private string _potionName;
    [SerializeField] private float _cookTime;
    [SerializeField] private List<InventoryItem> _ingredients;
    [SerializeField] private InventoryItem _basisItem;
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
    public int GetOpenLevel() 
    {
        int minLevel = 0;
        foreach (InventoryItem ingredient in _ingredients)
        {
            if (minLevel < ingredient.GetLevelUnlockRecept())
            {
                minLevel = ingredient.GetLevelUnlockRecept();
            }
        }
        
        return _needLevelToUse < minLevel ? minLevel : _needLevelToUse;
    }

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

    public List<InventoryItem> GetIngredientList()
    {
        List<InventoryItem> list = new List<InventoryItem>();
        foreach (InventoryItem item in _ingredients)
        {
            list.Add(item);
        }
        return list;
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

    public InventoryItem GetBasis()
    {
        return _basisItem;
    }

    public int CompareTo(object? o)
    {
        _needLevelToUse = GetOpenLevel();
        if (o is RecipeData obj) { 
            int comp = _needLevelToUse.CompareTo(obj._needLevelToUse);
            if (comp != 0) return comp;
            else return _potionName.CompareTo(obj._potionName);
        } 
        else throw new ArgumentException("Некорректное значение параметра");
    } 
}
