using System;
using UnityEngine;

public class Pot : InteractionObject
{

    [SerializeField] private PotState _potState;
    [SerializeField] private CheckPlayer _potArea;
    [SerializeField] private float _timeDecreaseRate;

    private new void OnEnable()
    {
        base.OnEnable();
        _potArea.OnTrigger += CheckState;
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
    }
}
