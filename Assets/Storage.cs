using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] private CheckPlayer _unloadArea;
    [SerializeField] private CheckPlayer _InteractionArea;
    [SerializeField] private StorageGrid _grid;
    private bool _inUnloadAreaTrigger = false;
    private bool _inInteractionAreaTrigger = false;
    private List<InventoryItem> _StorageItems;
    private int _StorageCapacity = 24;

    private void Start()
    {
        _StorageItems = new List<InventoryItem>();
        _grid.InitGrid();
        _grid.gameObject.SetActive(false);
    }
    private new void OnEnable()
    {
        EventManager.StorageCellClick += tryCollectItem;
        _unloadArea.OnTrigger += onUnloadAreaEnter;
        _InteractionArea.OnTrigger += onInteractionAreaEnter;
    }

    private void OnDisable()
    {
        EventManager.StorageCellClick -= tryCollectItem;
        _unloadArea.OnTrigger -= onUnloadAreaEnter;
        _InteractionArea.OnTrigger -= onInteractionAreaEnter;
    }

    private void onInteractionAreaEnter(bool inTrigger = true)
    {
        _inInteractionAreaTrigger = inTrigger;
        if (inTrigger)
        {
            _grid.gameObject.SetActive(true);
        }
        else
        {
            _grid.gameObject.SetActive(false);
        }
    }

    private void onUnloadAreaEnter(bool inTrigger = true)
    {
        _inUnloadAreaTrigger = inTrigger;
        List<InventoryItem> inventoryItems = Inventory.Instanse.GetUIInventoryData(); ;

        foreach (var item in inventoryItems)
        {
            if (_StorageItems.Count < _StorageCapacity)
            {
                InventoryItem item_tmp = item;
                _StorageItems.Add(item_tmp);
                Inventory.Instanse.RemoveItem(item_tmp);
            }
            else break;
        }
        UpdateStorageUI();
    }


    private void UpdateStorageUI()
    {
        int i = 0;
        for (; i < _StorageItems.Count; i++)
        {
            _grid.ClearCell(i);
            ItemIcon icon = _StorageItems[i].GetIcon();
            if (!icon) Debug.Log("No icon");
            _grid.setIcon(icon, i);
        }

        for (; i < _StorageCapacity; i++)
        {
            _grid.ClearCell(i);
        }
    }


    private void tryCollectItem(int idx)
    {
        if (idx >= _StorageItems.Count || !Inventory.Instanse.AddItem(_StorageItems[idx]))
        {
            return;
        } 
        _StorageItems.RemoveAt(idx);
        UpdateStorageUI();
    }
}
