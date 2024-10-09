using UnityEngine;

[CreateAssetMenu(menuName = "WitchScripts/InventoryItem")]
public class InventoryItem : ScriptableObject
{
    [SerializeField] private ItemIcon _itemIcon;
    [SerializeField] private ItemTypes _itemType;
    [SerializeField] private PotionTypes _potionType;
    [SerializeField] private PlantTypes _plantType;
    [SerializeField] private int _price;

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


}
