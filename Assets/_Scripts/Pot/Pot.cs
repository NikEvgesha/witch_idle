using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Pot : InteractionObject
{

    [SerializeField] private PotState _potState;
    [SerializeField] private CheckPlayer _potArea;
    [SerializeField] private float _timeDecreaseRate;
    [SerializeField] private GrowthTimer _cookingTimer;
    [SerializeField] private RecipeInfoUI _recipeInfoUI;
    [SerializeField] private ItemCollector _itemCollector;
    [SerializeField] private HarvestTimer _harvestTimer;
    [SerializeField] private int _harvestTime = 1;
    //[SerializeField] private ItemStateProdaction _itemStateInfoUI;
    private RecipeData _currentRecipe;
    private List<InventoryItem> _requiredIngredients;
    private bool _inTrigger;

    private new void OnEnable()
    {
        base.OnEnable();
        _potArea.OnTrigger += TryCollect;
        _cookingTimer.TimerFinish += FinishCooking;
        _recipeInfoUI.DropButtonClick += DropRecipe;
        _harvestTimer.TimerFinish += Collect;
    }

    private void OnDisable()
    {
        _potArea.OnTrigger -= TryCollect;
        _cookingTimer.TimerFinish -= FinishCooking;
        _recipeInfoUI.DropButtonClick -= DropRecipe;
        _harvestTimer.TimerFinish -= Collect;
    }

    private void TryCollect(bool inTrigger = true)
    {
        _inTrigger = inTrigger;
        CheckState();
    }

    private new void CheckState()
    {
        Debug.Log("CHECK STATE: " + _potState);
        base.CheckState();
        switch (_potState) 
        {
            case PotState.Empty:
                {
                    
                    _cookingTimer.gameObject.SetActive(false);
                    if (_inTrigger)
                        {
                        RecipeManager.Instance.RequestRecipe(this);   
                        }
                        else
                        {
                        RecipeManager.Instance.FinishRequestRecipe(this);
                        }
                    break;
                }
            case PotState.BasisRequire:
                {
                    if (_inTrigger)
                    {
                        CheckIngredients();
                        if (_requiredIngredients.Count == 0)
                        {
                            OnBasisAdd();
                        }
                        else
                        {
                            _recipeInfoUI.ShowDropButton();
                        }
                    }
                    else
                    {
                        _recipeInfoUI.HideDropButton();
                    }
                    break;
                }
            case PotState.IngredientRequire:
                {
                    if (_inTrigger)
                    {
                        CheckIngredients();
                        if (_requiredIngredients.Count == 0)
                        {
                            StartCooking();
                        } else
                        {
                            _recipeInfoUI.ShowDropButton();
                        }
                    } else
                    {
                        _recipeInfoUI.HideDropButton();
                    }
                    break;
                }
            case PotState.Done:
                {
                    TryPotionCollect();
                    break;
                }
            default:
                break;
        }
        
    }

    private void TryPotionCollect()
    {
        if (Inventory.Instanse.HaveEmptySlot()) 
        {
            _harvestTimer.gameObject.SetActive(_inTrigger);
            if (_inTrigger)
            {
                _harvestTimer.StartWellTimer(_harvestTime);
            }
        }
    }

    private void Collect()
    {
        if (Inventory.Instanse.AddItem(_currentRecipe.GetItem()))
        {
            WitchPlayerController.Instanse.Experience += _currentRecipe.GetExperience();
            _recipeInfoUI.HideCookingItem();
            _harvestTimer.gameObject.SetActive(false);
            _potState = PotState.Empty;
            CheckState();
            //SaveSeedBed();
        }
    }

    public void SetRecipe(RecipeData recipeData)
    {
        _potState = PotState.BasisRequire;
        _currentRecipe = recipeData;
        _requiredIngredients = new List<InventoryItem>();
        RequestIngredients();
        _recipeInfoUI.SetRecipeUI(_currentRecipe.GetItem(), _requiredIngredients);
        CheckState();
    }

    private void OnBasisAdd()
    {
        _potState = PotState.IngredientRequire;
        RequestIngredients();
        _recipeInfoUI.SetRecipeUI(_currentRecipe.GetItem(), _requiredIngredients);
        CheckState();
    }


    private void RequestIngredients()
    {
        Debug.Log(_currentRecipe.GetPotionName());
        if (_potState == PotState.BasisRequire)
        {
            _requiredIngredients.Add(_currentRecipe.GetBasis());
        }
        else
        {
            _requiredIngredients = _currentRecipe.GetIngredients();
        }
    }

    private void CheckIngredients()
    {
        List<InventoryItem> inventoryItems;
        List<InventoryItem> addedItems = new List<InventoryItem>();
        inventoryItems = Inventory.Instanse.GetUIInventoryData();
        foreach (var item in inventoryItems)
        {
            InventoryItem item_tmp = item;
            if (_requiredIngredients.Contains(item_tmp))
            {
                _requiredIngredients.Remove(item_tmp);
                addedItems.Add(item_tmp);
                Inventory.Instanse.RemoveItem(item_tmp);
                _itemCollector.ItemCollect(item_tmp, this.transform, false);
            }
        }
        if (addedItems.Count != 0)
        {
            _recipeInfoUI.UpdateIngredients(addedItems);
        }
    }

    private void StartCooking() {

        RecipeManager.Instance.FinishRequestRecipe(this);
        _potState = PotState.Cooking;
        _recipeInfoUI.HideIngredients();
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

    public void DropRecipe()
    {
        if (_potState == PotState.IngredientRequire || _potState == PotState.BasisRequire)
        {
            _recipeInfoUI.HideCookingItem();
        }
        _recipeInfoUI.HideIngredients();
        _recipeInfoUI.HideDropButton();
        _potState = PotState.Empty;
        _currentRecipe = null;
        CheckState();
    }

   

}
