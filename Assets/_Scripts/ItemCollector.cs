using System.Collections;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public class ItemCollector : MonoBehaviour
{
    private Transform _source;
    private Transform _target;
    private ItemModel _itemObj;
    [SerializeField] private Color _color;

    public void ItemCollect(InventoryItem item, Transform obj, bool toPlayer)
    {
        if (toPlayer)
        {
            _source = obj.transform;
            _target = Inventory.Instanse.gameObject.transform;
        } else
        {
            _source = Inventory.Instanse.gameObject.transform;
            _target = obj.transform;
        }
        ItemModel itemPrefab = item.GetItemPrefab();
        _itemObj = Instantiate(itemPrefab, _source);
        _itemObj.Config(_target);
    }

    

}
