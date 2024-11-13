using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private List<RecipeData> _recipes;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Recipe _recipePrefab;
    [SerializeField] private UIRecipeBook _UIRecipeBook;
    [SerializeField] private GameObject _recipeUIObj;
    [SerializeField] private Transform _activePoint;
    [SerializeField] private Transform _hidePoint;
    [SerializeField] private float _moveStep = 5f;
    //[SerializeField] private Animator _animator;
    private Pot _currentPot;
    private float _lerpPoint = 0;

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
        }
            StopAllCoroutines();
            StartCoroutine(Slide(_hidePoint.position,_activePoint.position, true));
            UpdateTimeRate(timeDecreaseRate);
        
    }
    
    private void HideRecipeBook()
    {
        if (_canvas.activeInHierarchy)
        {
            StopAllCoroutines();
            StartCoroutine(Slide(_activePoint.position, _hidePoint.position, false));
        }
    }


    IEnumerator Slide(Vector3 moveFrom, Vector3 moveTo, bool setActive)
    {
        while (_lerpPoint < 1)
        {
            _lerpPoint += Time.fixedDeltaTime * _moveStep;
            //Vector3 curPos = _recipeUIObj.transform.position;
            _recipeUIObj.transform.position = Vector3.Lerp(_recipeUIObj.transform.position, moveTo, _lerpPoint);
            //_RecipeUIObj.transform.Translate((moveTo - curPos).normalized * _moveStep);
            yield return new WaitForFixedUpdate();
        }
        _lerpPoint = 0;
        if (!setActive)
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
        _recipes.Sort();

        for (int i = 0; i < _recipes.Count; i++)
        {
            Debug.Log(_recipes[i].GetPotionName());
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
    public RecipeData CheckReciept(InventoryItem potion)
    {
        foreach (RecipeData recipe in _recipes) 
        {
            if (recipe.GetItem() == potion)
            {
                return recipe;
            }
        }
        return null;
    }



}
