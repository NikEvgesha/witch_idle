using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private List<RecipeData> _recipes;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Recipe _recipePrefab;
    [SerializeField] private UIRecipeBook _UIRecipeBook;
    private Pot _currentPot;

    public static RecipeManager Instance;

    private void Awake()
    {
        Instance = this;
    }
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
    private void DisplayRecipeBook(float timeDecreaseRate = 0) { 
        if (!_canvas.activeInHierarchy)
        {
            _canvas.SetActive(true);
            UpdateTimeRate(timeDecreaseRate);
        }
    }
    
    private void HideRecipeBook()
    {
        if (_canvas.activeInHierarchy)
        {
            _canvas.SetActive(false);
        }
    }

    public void RequestRecipe(Pot pot) {
        _currentPot = pot; 
        DisplayRecipeBook();
    }

    public void FinishRequestRecipe(Pot pot)
    {
        if (_currentPot == pot)
        {
            _currentPot = null;
            HideRecipeBook();
        }
    }

    public void onRecipeSelect(RecipeData recipeData) {
        if (_currentPot == null) return;

        _currentPot.SetRecipe(recipeData);
        FinishRequestRecipe(_currentPot);
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

    public void onCloseButtonClick()
    {
        if (_canvas.activeInHierarchy)
        {
            _currentPot = null;
            HideRecipeBook();
        }     
    }



}
