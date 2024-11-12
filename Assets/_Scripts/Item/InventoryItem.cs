using UnityEngine;

[CreateAssetMenu(menuName = "WitchScripts/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    [SerializeField] private ItemIcon _itemIcon;
    [SerializeField] private ItemModel _itemPrefab;
    [SerializeField] private ItemTypes _itemType;
    [SerializeField] private PotionTypes _potionType;
    [SerializeField] private PlantTypes _plantType;
    [SerializeField] private int _price;
    [SerializeField] private int _experience;
    [SerializeField] private int _levelUnlock = 0;

    public ItemIcon GetIcon()
    {
        return _itemIcon;
    }

    public PlantTypes GetPlantType()
    {
        return _plantType;
    }

    public PotionTypes GetPotionType()
    {
        return _potionType;
    }

    public ItemTypes GetItemType()
    {
        return _itemType;
    }
    public int GetPrice()
    {
        return _price;
    }
    public int GetLevelUnlock()
    {
        return _levelUnlock;
    }
    public int GetExperience()
    {
        return _experience;
    }
    public int GetLevelUnlockRecept()
    {
        RecipeData recipe;
        recipe = RecipeManager.Instance.CheckReciept(this);
        if (recipe != null)
        {
            return recipe.GetOpenLevel();
        }
        return _levelUnlock;
    }

    public ItemModel GetItemPrefab() 
    {
        return _itemPrefab;
    }
        

}
