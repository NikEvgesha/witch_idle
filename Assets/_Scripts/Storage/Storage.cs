using System;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    [SerializeField] private CheckPlayer _InteractionArea;
    [SerializeField] private StorageGrid _StorageGrid;
    [SerializeField] private StorageGrid _InventoryGrid;
    [SerializeField] private Canvas _Canvas;
    private bool _inUnloadAreaTrigger = false;
    private bool _inInteractionAreaTrigger = false;
    private List<InventoryItem> _StorageItems;
    private List<InventoryItem> _inventoryItems;
    private int _StorageCapacity = 25;
    public Action<StorageAction, int> StorageCellClick;
    public Action StorageUpdate;
    private void Start()
    {
        _inventoryItems = new List<InventoryItem>();
        _StorageItems = new List<InventoryItem>();
        _StorageGrid.InitGrid(this, _StorageCapacity,GridUIType.StorageGrid);
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
            _InventoryGrid.InitGrid(this, Inventory.Instanse.Capacity,GridUIType.InventoryGrid);
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
            _StorageGrid.SetIcon(icon, i);
        }

        for (; i < _StorageCapacity; i++)
        {
            _StorageGrid.ClearCell(i);
        }
        
        for (i = 0; i < _inventoryItems.Count; i++)
        {
            _InventoryGrid.ClearCell(i);
            ItemIcon icon = _inventoryItems[i].GetIcon();
            _InventoryGrid.SetIcon(icon, i);
        }

        for (; i < Inventory.Instanse.Capacity; i++)
        {
            _InventoryGrid.ClearCell(i);
        }
        StorageUpdate?.Invoke();
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
            TakeItemVault(idx);
        } else if (action == StorageAction.PutToStorage)
        {
            InventoryItem item_tmp = _inventoryItems[idx];
            if (PlaceItemVault(item_tmp))
            {
                Inventory.Instanse.RemoveItem(item_tmp);
            }
        }
        UpdateStorageUI();
    }
    public bool PlaceItemVault(InventoryItem item) //положить в хранилище
    {
        if (_StorageItems.Count >= _StorageCapacity)
        {
            return false;
        }
        _StorageItems.Add(item);
        UpdateStorageUI();
        return true;
    }
    public void TakeItemVault(InventoryItem item) //взять из хранилища
    {
        _StorageItems.Remove(item);
        UpdateStorageUI();

    }
    private void TakeItemVault(int idx) //взять из хранилища по индексу
    {
        _StorageItems.RemoveAt(idx);
        UpdateStorageUI();

    }
    public bool CheckItemInStorage(InventoryItem item)
    {
        foreach (var itemStore in _StorageItems)
        {
            if (item.GetPotionType() == itemStore.GetPotionType())
            {
                return true;
            }
        }
        return false;
    }


}
