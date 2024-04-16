using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Recipe : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _potionNameField;
    [SerializeField] private Image _potionIcon;
    private RecipeData _recipeData;

    public void InitSlot(RecipeData recipeData)
    {
        _recipeData = recipeData;
        _potionNameField.text = _recipeData.GetPotionName();
        _potionIcon.sprite = _recipeData.GetIcon();
    }

    public void onRecipeSelect()
    {
        EventManager.RecipeSelected.Invoke(_recipeData);
    }
}
