using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [SerializeField] private int _startCapacity;

    private int _capacity;

    public int Capacity
    {
        get { return _capacity; }
        set
        {
            _capacity = value;
            EventManager.CapacityUpdate?.Invoke(_capacity);
            SaveControl.Instanse.SaveCapacity(_capacity);
        }
    }
    public static Inventory Instanse;
    private List<InventoryItem> _inventory;

    private void Awake()
    {
        Instanse = this;
        
    }

    private void Start()
    {
        if (SaveControl.Instanse.TryGetCapacity()) { 
            Capacity = SaveControl.Instanse.GetCapacity();
        } else
        {
            Capacity = _startCapacity;
        }
        _inventory = new List<InventoryItem>();
    }


    public List<InventoryItem> GetUIInventoryData() {
        List<InventoryItem> items = new List<InventoryItem>();
        if (_inventory != null) { 
            foreach (var item in _inventory)
            {
                items.Add(item);
            }
        }
        return items;
        /*
        if (_inventory == null) {
            return new List<GameObject>();
        }
        List<GameObject> iconList = new List<GameObject>();
        foreach (var item in _inventory)
        {
            iconList.Add(item.GetPlantIcon());
        }
        return iconList;*/
    }

    public bool AddItem(InventoryItem item) {

        if (_inventory.Count >= _capacity) 
            return false;

        _inventory.Add(item);
        UIInventory.Instance.UpdateUI();
        return true;
    }

    public bool RemoveItem(InventoryItem plant)
    {
        if (_inventory.Contains(plant))
        {
            _inventory.Remove(plant);
            UIInventory.Instance.UpdateUI();
            return true;
        }
        return false;

    }

    /*public bool CheckItem(PlantTypes plant)
    {
        foreach (var item in _inventory)
        {
            if (item.GetPlantType() == plant) return true;
        }
        return false;
    }*/



}
