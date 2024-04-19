using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Pot : InteractionObject
{

    [SerializeField] private PotState _potState;
    [SerializeField] private CheckPlayer _potArea;
    [SerializeField] private float _timeDecreaseRate;
    [SerializeField] private GrowthTimer _cookingTimer;
    private RecipeData _currentRecipe;
    private List<PlantTypes> _requiredIngredients;

    private new void OnEnable()
    {
        base.OnEnable();
        _potArea.OnTrigger += CheckState;
        _cookingTimer.TimerFinish += FinishCooking;
    }

    private void CheckState(bool inTrigger = true)
    {
        switch (_potState) {
            case PotState.Empty:
                {
                    if (inTrigger)
                            {
                                EventManager.RecipeBookOpened?.Invoke(_timeDecreaseRate);
                                EventManager.RecipeSelected += OnRecipeSelect;
                            }
                            else
                            {
                                EventManager.RecipeBookClosed?.Invoke();
                                EventManager.RecipeSelected -= OnRecipeSelect;
                            }
                    break;
                }
            case PotState.IngredientRequire:
                {
                    if (inTrigger)
                    {
                        CheckIngredients(_currentRecipe);
                        if (_requiredIngredients.Count == 0)
                        {
                            StartCooking();
                        }
                    }
                    break;
                }
                }
        
    }

    private void OnDisable()
    {
        _potArea.OnTrigger += CheckState;
    }

    private void OnRecipeSelect(RecipeData recipeData)
    {
        Debug.Log("Recipe selected: " + recipeData.GetPotionName());
        _potState = PotState.IngredientRequire;
        EventManager.RecipeBookClosed?.Invoke();
        _currentRecipe = recipeData;
        recipeData.GetIngredients(out _requiredIngredients);
        Debug.Log(_requiredIngredients.Count);
    }

    private void CheckIngredients(RecipeData recipeData)
    {
        List<PlantsData> inventoryItems;
        Inventory.Instanse.GetUIInventoryData(out inventoryItems);
        foreach (var item in inventoryItems)
        {
            PlantsData item_tmp = item;
            var type = item.GetPlantType();
            if (_requiredIngredients.Contains(type))
            {
                _requiredIngredients.Remove(type);
                Inventory.Instanse.RemoveItem(item_tmp);
            }
        }

    }

    private void StartCooking() {
        _potState = PotState.Cooking;
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
