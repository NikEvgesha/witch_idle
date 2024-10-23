using UnityEngine;
using UnityEngine.UI;

public class IngredientSlot : MonoBehaviour
{
    [SerializeField] private Image _FillImage;
    private InventoryItem _item;
    private bool _isAdded = false;

    public void InitSlot(InventoryItem item)
    {
        _item = item;
        Instantiate(_item.GetIcon(), this.transform);
        if (_isAdded)
        {
            FillIcon(_isAdded);
        }
    }

    public void setAdded(bool isAdded)
    {
        _isAdded = isAdded;
        FillIcon(_isAdded);
    } 

    public void FillIcon(bool filled)
    {
        _FillImage.gameObject.SetActive(filled);
    }

    public InventoryItem GetItem() {
        return _item;
    }
}
