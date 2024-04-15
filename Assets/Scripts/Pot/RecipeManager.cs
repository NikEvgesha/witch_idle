using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeManager : MonoBehaviour
{
    [SerializeField] private List<RecipeData> _recipes;
    [SerializeField] private GameObject _canvas;
    [SerializeField] private Recipe _recipePrefab;
    [SerializeField] private UIRecipeBook _UIRecipeBook;

    public void DisplayRecipeBook() { 
        if (!_canvas.activeInHierarchy)
        {
            _canvas.SetActive(true);

        }
    }


    private void RenderRecipeSlots()
    {

    }

    

}
