using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Recipe : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _potionNameField;
    [SerializeField] private Image _potionIcon;
    [SerializeField] private Transform _ingredients;
    [SerializeField] private Button _button;
    [SerializeField] private GameObject _levelLock;
    [SerializeField] private TextMeshProUGUI _levelText;
    private RecipeData _recipeData;
    private bool _recipeUnlock = false;

    private void OnEnable()
    {
        EventManager.PlayerLevelChange += CheckRecipeLock;
    }
    private void OnDisable()
    {
        EventManager.PlayerLevelChange -= CheckRecipeLock;
    }
    public void InitSlot(RecipeData recipeData)
    {
        _recipeData = recipeData;
        _potionNameField.text = _recipeData.GetPotionName();
        Instantiate(_recipeData.GetItem().GetIcon(), _potionIcon.transform);
        foreach (var item in _recipeData.GetIngredients())
        {
            Instantiate(item.GetIcon(), _ingredients);
        }
        CheckRecipeLock(WitchPlayerController.Instanse.PlayerLevel);
        _levelText.text = _recipeData.GetOpenLevel().ToString();
    }

    public void onRecipeSelect()
    {
        RecipeManager.Instance.onRecipeSelect(_recipeData);
    }
    private void CheckRecipeLock(int level)
    {
        if (!_recipeUnlock) 
        {
            if (_recipeData.GetOpenLevel() <= level)
            {
                _recipeUnlock = true;
            }
        }
        _button.interactable = _recipeUnlock;
        _levelLock.SetActive(!_recipeUnlock);
    }
}
