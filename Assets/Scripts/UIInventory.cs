using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private GameObject _UIEmptySlot;
    [SerializeField] private Transform _SlotsPoint;

    private List<PlantsData> _items;
    private List<PlantIcon> _UIitems;
    private List<GameObject> _UIslots;
    private bool _isVisible = true;

    private void Start()
    {
        RenderNewImage();
    }

    private void OnEnable()
    {
        EventManager.UpdateUIInventory += UpdateUI;
        EventManager.CapacityUpdate += UpdateCapacityUI;
    }


    private void UpdateUI()
    {
        if (!_isVisible) {
            return;
        }
        RenderNewImage();
    }


    private void UpdateCapacityUI(int capacity) {
        if (_UIslots != null)
        {
            foreach (var _UIslot in _UIslots)
            {
                Destroy(_UIslot);
            }
        }
        _UIslots = new List<GameObject>();

        for (int i = 0; i < capacity; i++)
        {
            _UIslots.Add(Instantiate(_UIEmptySlot, _SlotsPoint));
        }
        UpdateUI();
    }

    private void RenderNewImage() {
        if (!CheckItem()) {
            return;
        }
        if (_UIitems != null)
        {
            foreach (var _UIitems in _UIitems)
            {
                Destroy(_UIitems.gameObject);
            }
        }
        _UIitems = new List<PlantIcon>();
        int i = 0;
        foreach (var item in _items)
        {
            _UIitems.Add(Instantiate(item.GetPlantIcon(), _UIslots[i].transform));
            i++;
        }
    }

    private bool CheckItem() {
        _items = Inventory.Instanse.GetUIInventoryData();

        return _items != null && _items.Count > 0;
    }



    public void SwitchVisibility() {
        _isVisible = !_isVisible;
        _SlotsPoint.gameObject.SetActive(_isVisible);
        if (_isVisible)
            UpdateUI();
    }

}
