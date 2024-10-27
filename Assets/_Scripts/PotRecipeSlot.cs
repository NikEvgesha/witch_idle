using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotRecipeSlot : IngredientSlot
{
    [SerializeField] private GameObject _processIcon;
    [SerializeField] private GameObject _completeIcon;
    
    public void SetProcessIcon(bool isProcessing)
    {
        if (isProcessing)
        {
            _completeIcon.SetActive(false);
            _processIcon.SetActive(true);
        } else
        {
            _completeIcon.SetActive(true);
            _processIcon.SetActive(false);
        }
    }
}
