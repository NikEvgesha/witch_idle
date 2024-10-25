using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;

public class Pot : InteractionObject
{

    [SerializeField] private PotState _potState;
    [SerializeField] private CheckPlayer _potArea;
    [SerializeField] private float _timeDecreaseRate;
    [SerializeField] private GrowthTimer _cookingTimer;
    [SerializeField] private RecipeInfoUI _recipeInfoUI;
    //[SerializeField] private ItemStateProdaction _itemStateInfoUI;
    private RecipeData _currentRecipe;
    private List<InventoryItem> _requiredIngredients;
    private bool _inTrigger;

    private new void OnEnable()
    {
        base.OnEnable();
        _potArea.OnTrigger += TryCollect;
        _cookingTimer.TimerFinish += FinishCooking;
        _recipeInfoUI.DropButtonClick += DropRecipe;
    }

    private void OnDisable()
    {
        _potArea.OnTrigger -= TryCollect;
        _cookingTimer.TimerFinish -= FinishCooking;
        _recipeInfoUI.DropButtonClick -= DropRecipe;
    }

    private void TryCollect(bool inTrigger = true)
    {
        _inTrigger = inTrigger;
        CheckState();
    }

    private new void CheckState()
    {
        base.CheckState();
        switch (_potState) 
        {
            case PotState.Empty:
                {
                    _cookingTimer.gameObject.SetActive(false);
                    if (_inTrigger)
                        {
                        RecipeManager.Instance.RequestRecipe(this);   
                        }
                        else
                        {
                        RecipeManager.Instance.FinishRequestRecipe(this);
                        }
                    break;
                }
            case PotState.WaterRequire:
                {
                    if (_inTrigger)
                    {
                        if (!CheckWater())
                        {
                            _recipeInfoUI.ShowDropButton();
                        }
                    }
                    else
                    {
                        _recipeInfoUI.HideDropButton();
                    }

                    break;
                }
            case PotState.IngredientRequire:
                {
                    if (_inTrigger)
                    {

                        CheckIngredients(_currentRecipe);
                        if (_requiredIngredients.Count == 0)
                        {
                            StartCooking();
                        } else
                        {
                            _recipeInfoUI.ShowDropButton();
                        }

                    } else
                    {
                        _recipeInfoUI.HideDropButton();
                    }
                    break;
                }
            case PotState.Done:
                {
                    PotionCollect();
                    break;
                }
            default:
                break;
        }
        
    }

    private void PotionCollect()
    {

        if (!Inventory.Instanse.AddItem(_currentRecipe.GetItem()))
        {
            return;
        }

        _recipeInfoUI.HideCookingItem();
        _potState = PotState.Empty;
        CheckState();
        //SaveSeedBed();
    }

    public void SetRecipe(RecipeData recipeData)
    {
        _potState = PotState.WaterRequire;
        _currentRecipe = recipeData;
        _recipeInfoUI.SetWater();
        CheckState();
    }

    private void SetRecipeUI()
    {
        _requiredIngredients = _currentRecipe.GetIngredients();
        _recipeInfoUI.SetRecipe(_currentRecipe);
        _recipeInfoUI.ShowCookingItem(_currentRecipe.GetItem());
    }

    private void CheckIngredients(RecipeData recipeData)
    {
        List<InventoryItem> inventoryItems;
        List<InventoryItem> addedItems = new List<InventoryItem>();
        inventoryItems = Inventory.Instanse.GetUIInventoryData();
        foreach (var item in inventoryItems)
        {
            InventoryItem item_tmp = item;
            //var type = item.GetPlantType();
            if (_requiredIngredients.Contains(item_tmp))
            {
                _requiredIngredients.Remove(item_tmp);
                addedItems.Add(item_tmp);
                Inventory.Instanse.RemoveItem(item_tmp);
            }
        }
        if (addedItems.Count != 0)
        {
            _recipeInfoUI.UpdateIngredients(addedItems);
        }
    }

    private void StartCooking() {

        RecipeManager.Instance.FinishRequestRecipe(this);
        _potState = PotState.Cooking;
        _recipeInfoUI.HideIngredients();
        //_itemStateInfoUI.ShowCookingItem(_currentRecipe.GetItem());
        _potArea.gameObject.SetActive(false);
        _cookingTimer.gameObject.SetActive(true);
        _cookingTimer.StartGrowthTimer(_currentRecipe.GetCookTime());
    }


    private void FinishCooking()
    {
        _cookingTimer.gameObject.SetActive(false);
        _potArea.gameObject.SetActive(true);
        _potState = PotState.Done;
    }


    private bool CheckWater()
    {
        bool waterAdded = false;
        List<InventoryItem> inventoryItems = Inventory.Instanse.GetUIInventoryData();
        foreach (var item in inventoryItems)
        {
            InventoryItem item_tmp = item;
            ItemTypes type = item.GetItemType();
            if (type == ItemTypes.Water)
            {
                Inventory.Instanse.RemoveItem(item_tmp);
                waterAdded = true;
            }
        }
        if (waterAdded)
        {
            _potState = PotState.IngredientRequire;
            SetRecipeUI();
            CheckState();
        }
        return waterAdded;
    }


    public void DropRecipe()
    {
        if (_potState == PotState.IngredientRequire)
        {
            _recipeInfoUI.HideCookingItem();
        }
        _recipeInfoUI.HideIngredients();
        _potState = PotState.Empty;
        _currentRecipe = null;
        CheckState();
    }

   

}
