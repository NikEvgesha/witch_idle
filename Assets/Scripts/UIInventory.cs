using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

public class UIInventory : MonoBehaviour
{
    private List<PlantsData> _items;
    private List<PlantIcon> _UIitems;

    private void Start()
    {
        RenderNewImage();
    }

    private void OnEnable()
    {
        EventManager.UpdateUIInventory += UpdateUI;
    }


    private void UpdateUI()
    {
        RenderNewImage();
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
        foreach (var item in _items)
        {
            _UIitems.Add(Instantiate(item.GetPlantIcon(), this.transform));
        }
    }

    private bool CheckItem() {
        _items = PickUpItems.Instanse.GetUIInventoryData();

        return _items != null && _items.Count > 0;
    }

}
