using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventory : MonoBehaviour
{
    private List<GameObject> _items;
    private List<GameObject> _UIitems;
    
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

        foreach (var item in _items)
        {
            _UIitems.Add(Instantiate(item, this.transform));

        }


    }

    private bool CheckItem() {
        _items = PickUpItems.Instanse.GetUIInventoryData();

        return _items != null && _items.Count > 0;
    }

}
