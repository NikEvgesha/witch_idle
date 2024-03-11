using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
    private List<PlantsData> _inventory;

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
        _inventory = new List<PlantsData>();
    }


    public List<PlantsData> GetUIInventoryData() {
        if (_inventory == null)
            return new List<PlantsData>();
        return _inventory;
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

    public bool AddItem(PlantsData plantData) {

        if (_inventory.Count >= _capacity) 
            return false;

        _inventory.Add(plantData);
        EventManager.UpdateUIInventory?.Invoke();
        return true;
    }



}
