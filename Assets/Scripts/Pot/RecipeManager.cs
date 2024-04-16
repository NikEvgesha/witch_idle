using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private List<RecipeData> _recipes;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Recipe _recipePrefab;
    [SerializeField] private UIRecipeBook _UIRecipeBook;

    private void Start()
    {
        LoadRecipes();
    }

    private void OnEnable()
    {
        EventManager.RecipeBookOpened += DisplayRecipeBook;
        EventManager.RecipeBookClosed += HideRecipeBook;
    }

    private void OnDisable()
    {
        EventManager.RecipeBookOpened -= DisplayRecipeBook;
        EventManager.RecipeBookClosed -= HideRecipeBook;
    }
    public void DisplayRecipeBook(float timeDecreaseRate) { 
        if (!_canvas.activeInHierarchy)
        {
            _canvas.SetActive(true);
            UpdateTimeRate(timeDecreaseRate);
        }
    }

    public void HideRecipeBook()
    {
        if (_canvas.activeInHierarchy)
        {
            _canvas.SetActive(false);
        }
    }

    private void LoadRecipes()
    {
        for (int i = 0; i < _recipes.Count; i++)
        {
            var recipeSlot = Instantiate(_recipePrefab, _UIRecipeBook.transform.position, Quaternion.identity);
            recipeSlot.transform.SetParent(_UIRecipeBook.transform, false);
            recipeSlot.InitSlot(_recipes[i]);
        }
    }

    private void UpdateTimeRate(float rate)
    {
        // отображение времени в зависимости от прокачки котла
    }

    private void UpdateRecipe()
    {

    }



}
