using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;

public class Recipe : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _potionNameField;
    [SerializeField] private Image _potionIcon;
    [SerializeField] private Transform _ingredients;
    private RecipeData _recipeData;


    public void InitSlot(RecipeData recipeData)
    {
        _recipeData = recipeData;
        _potionNameField.text = _recipeData.GetPotionName();
        Instantiate(_recipeData.GetItem().GetIcon(), _potionIcon.transform);
        
        int i = 0;
        foreach (var item in _recipeData.GetIngredients())
        {
            Instantiate(item.GetIcon(), _ingredients);
        }
    }

    public void onRecipeSelect()
    {
        RecipeManager.Instance.onRecipeSelect(_recipeData);
    }
}
