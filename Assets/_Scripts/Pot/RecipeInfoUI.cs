using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeInfoUI : MonoBehaviour
{
    [SerializeField] private GameObject _ingredients;
    [SerializeField] private IngredientSlot _ingredientPrefab;
    [SerializeField] private PotRecipeSlot _recipePrefab;
    [SerializeField] private Button _dropButton;
    [SerializeField] private Transform _recipe;
    [SerializeField] private GrowthTimer _cookTimer;
    [SerializeField] private GameObject _arrowImg;
    private List<InventoryItem> _requiredIngredients;
    private List<IngredientSlot> _UIitems;
    private PotRecipeSlot _currentItem;
    

    public Action DropButtonClick;

    private void Start()
    {
        _UIitems = new List<IngredientSlot>();
    }

    private void OnEnable()
    {
        _cookTimer.TimerFinish += onCookingComplete;
    }

    public void SetRecipeUI(InventoryItem potion, List<InventoryItem> ingredients)
    {
        DestroySlots();
        _requiredIngredients = new List<InventoryItem>(ingredients);
        _ingredients.SetActive(true);
        _arrowImg.SetActive(true);
        if (_currentItem == null)
        {
            ShowCookingItem(potion);
        }

        foreach (var item in ingredients)
        {
            IngredientSlot slot = Instantiate(_ingredientPrefab, _ingredients.transform);
            slot.InitSlot(item);
            _UIitems.Add(slot);
        }
    }

    public void UpdateIngredients(List<InventoryItem> addedIngredients)
    {
        foreach (var item in addedIngredients)
        {
            if (_requiredIngredients.Contains(item))
            {
                Debug.Log("---Item UI add---");
                AddToUIIngredient(item);
                _requiredIngredients.Remove(item);
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
        _arrowImg.SetActive(false);
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
        _UIitems.Clear();
    }

    public void ShowDropButton()
    {
        _dropButton.gameObject.SetActive(true);
    }

    public void HideDropButton()
    {
        _dropButton.gameObject.SetActive(false);
    }



    public void onDropButtonClick()
    {
        DropButtonClick?.Invoke();
    }

    public void HideCookingItem()
    {
        _currentItem.SetProcessIcon(true);
        Destroy(_currentItem.gameObject);
        _currentItem = null;
    }
    public void ShowCookingItem(InventoryItem item)
    {
        _currentItem = Instantiate(_recipePrefab, _recipe);
        _currentItem.InitSlot(item);
    }

    private void onCookingComplete()
    {
        _currentItem.SetProcessIcon(false);
    }


}
