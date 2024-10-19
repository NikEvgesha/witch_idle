using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;

public class Storage : MonoBehaviour
{
    [SerializeField] private CheckPlayer _InteractionArea;
    [SerializeField] private StorageGrid _StorageGrid;
    [SerializeField] private StorageGrid _InventoryGrid;
    [SerializeField] private Canvas _Canvas;
    private bool _inUnloadAreaTrigger = false;
    private bool _inInteractionAreaTrigger = false;
    private List<InventoryItem> _StorageItems;
    List<InventoryItem> _inventoryItems;
    private int _StorageCapacity = 25;
    public Action<StorageAction, int> StorageCellClick;
    private void Start()
    {
        _inventoryItems = new List<InventoryItem>();
        _StorageItems = new List<InventoryItem>();
        _StorageGrid.InitGrid(this, _StorageCapacity);
        _Canvas.gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        StorageCellClick += tryCollectItem;
        _InteractionArea.OnTrigger += onInteractionAreaEnter;
    }

    private void OnDisable()
    {
        StorageCellClick -= tryCollectItem;
        _InteractionArea.OnTrigger -= onInteractionAreaEnter;
    }

    public void CloseUI()
    {
        onInteractionAreaEnter(false);
    }

    private void onInteractionAreaEnter(bool inTrigger = true)
    {
        _inInteractionAreaTrigger = inTrigger;
        if (inTrigger)
        {
            foreach (Transform child in _InventoryGrid.transform)
            {
                Destroy(child.gameObject);
            }
            _InventoryGrid.InitGrid(this, Inventory.Instanse.Capacity);
            _Canvas.gameObject.SetActive(true);
            UpdateStorageUI();
        }
        else
        {
            _Canvas.gameObject.SetActive(false);
        }
    }


    public void PutAllToStorage()
    {
        _inventoryItems = Inventory.Instanse.GetUIInventoryData();

        foreach (var item in _inventoryItems)
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
        _inventoryItems = Inventory.Instanse.GetUIInventoryData();
        int i = 0;
        for (; i < _StorageItems.Count; i++)
        {
            _StorageGrid.ClearCell(i);
            ItemIcon icon = _StorageItems[i].GetIcon();
            _StorageGrid.setIcon(icon, i);
        }

        for (; i < _StorageCapacity; i++)
        {
            _StorageGrid.ClearCell(i);
        }
        
        for (i = 0; i < _inventoryItems.Count; i++)
        {
            _InventoryGrid.ClearCell(i);
            ItemIcon icon = _inventoryItems[i].GetIcon();
            _InventoryGrid.setIcon(icon, i);
        }

        for (; i < Inventory.Instanse.Capacity; i++)
        {
            _InventoryGrid.ClearCell(i);
        }
    }


    private void tryCollectItem(StorageAction action, int idx)
    {
        _inventoryItems = Inventory.Instanse.GetUIInventoryData();
        if (action == StorageAction.GetFromStorage)
        {
            if (idx >= _StorageItems.Count || !Inventory.Instanse.AddItem(_StorageItems[idx]))
            {
                return;
            }
            _StorageItems.RemoveAt(idx);
        } else if (action == StorageAction.PutToStorage)
        {
            if (_StorageItems.Count < _StorageCapacity)
            {
                InventoryItem item_tmp = _inventoryItems[idx];
                _StorageItems.Add(item_tmp);
                Inventory.Instanse.RemoveItem(item_tmp);
            }
        }
        UpdateStorageUI();
    }


}
