using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PickUpItems : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private int _startCapacity;
    private int _capacity;
    public static PickUpItems Instanse;
    private List<PlantsData> _inventory;

    private void Awake()
    {
        Instanse = this;
        
    }

    private void Start()
    {
        _capacity = _startCapacity;
        _inventory = new List<PlantsData>();
    }


    public List<GameObject> GetUIInventoryData() {

        if (_inventory == null) {
            return null;
        }
        List<GameObject> iconList = new List<GameObject>();
        foreach (var item in _inventory)
        {
            iconList.Add(item.GetPlantIcon());

        }
        return iconList;
    }

    public void AddItem(PlantsData plantData) {
        _inventory.Add(plantData);
    }



}