using System;
using System.Collections.Generic;
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
    [SerializeField] private ItemStateProdaction _itemStateInfoUI;
    private RecipeData _currentRecipe;
    private List<PlantTypes> _requiredIngredients;
    private bool _inTrigger;

    private new void OnEnable()
    {
        base.OnEnable();
        _potArea.OnTrigger += TryCollect;
        _cookingTimer.TimerFinish += FinishCooking;
    }

    private void OnDisable()
    {
        _potArea.OnTrigger -= TryCollect;
        _cookingTimer.TimerFinish -= FinishCooking;
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
                            RecipeManager.Instance.RequestRecipe(this);
                        }

                    } else
                    {
                        RecipeManager.Instance.FinishRequestRecipe(this);
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

        _itemStateInfoUI.HideCookingItem();
        _potState = PotState.Empty;
        CheckState();
        //SaveSeedBed();
    }

    public void SetRecipe(RecipeData recipeData)
    {
        _potState = PotState.IngredientRequire;
        _currentRecipe = recipeData;
        _requiredIngredients = _currentRecipe.GetIngredientsTypes();
        _recipeInfoUI.SetRecipe(_currentRecipe);
        CheckState();
    }

    private void CheckIngredients(RecipeData recipeData)
    {
        List<InventoryItem> inventoryItems;
        List<InventoryItem> addedItems = new List<InventoryItem>();
        inventoryItems = Inventory.Instanse.GetUIInventoryData();
        foreach (var item in inventoryItems)
        {
            InventoryItem item_tmp = item;
            var type = item.GetPlantType();
            if (type != PlantTypes.None && _requiredIngredients.Contains(type))
            {
                _requiredIngredients.Remove(type);
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
        _itemStateInfoUI.ShowCookingItem(_currentRecipe.GetItem());
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


   

}
