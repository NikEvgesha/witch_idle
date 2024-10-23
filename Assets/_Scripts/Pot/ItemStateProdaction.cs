using UnityEngine;

public class ItemStateProdaction : MonoBehaviour
{
    [SerializeField] private IngredientSlot _itemPrefab;
    private IngredientSlot _currentItem;
    public void HideCookingItem()
    {
        Destroy(_currentItem.gameObject);
    }
    public void ShowCookingItem(InventoryItem item)
    {
        _currentItem = Instantiate(_itemPrefab, gameObject.transform);
        _currentItem.InitSlot(item);
    }
}
