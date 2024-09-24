using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using static UnityEditor.Progress;

public class RecipeInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject _ingredients;
    [SerializeField] private IngredientSlot _ingredientPrefab;
    [SerializeField] private InventoryItem _waterItem;
    private List<InventoryItem> _requiredIngredients;
    private List<IngredientSlot> _UIitems;

    private void Start()
    {
        _UIitems = new List<IngredientSlot>();
    }
    public void SetRecipe(RecipeData recipe) {
        DestroySlots();
        _UIitems = new List<IngredientSlot>();
        _requiredIngredients = recipe.GetIngredients();
        _ingredients.SetActive(true);
        foreach (var item in _requiredIngredients)
        {
            IngredientSlot slot = Instantiate(_ingredientPrefab, _ingredients.transform);
            slot.InitSlot(item);
            _UIitems.Add(slot);

        }
    }  

    public void SetWater()
    {
        DestroySlots();
        _UIitems = new List<IngredientSlot>();
        _ingredients.SetActive(true);
        IngredientSlot slot = Instantiate(_ingredientPrefab, _ingredients.transform);
        slot.InitSlot(_waterItem);
        _UIitems.Add(slot);
    }
    /*    private void DisplayIngredients()
        {
            foreach (var item in _UIitems)
            {
                item.UpdateState();
            }

        }*/

    public void UpdateIngredients(List<InventoryItem> addedIngredients)
    {
        foreach (var item in addedIngredients)
        {
            if (_requiredIngredients.Contains(item))
            {
                AddToUIIngredient(item);
                //_requiredIngredients.Remove(item);
            }
        }
        //DisplayIngredients();
    }

    private void AddToUIIngredient(InventoryItem item)
    {
        foreach (var UIitem in _UIitems)
        {
            if (UIitem.GetItem() == item)
            {
                UIitem.setAdded(true);
                return;
            }
        }
    }

    public void HideIngredients()
    {
        _ingredients.SetActive(false);
    }

    private void DestroySlots()
    {
        if (_UIitems.Count > 0)
        {
            foreach (var item in _UIitems)
            {
                Destroy(item.gameObject);
            }
        }
    }



}
