using System.Collections.Generic;
using UnityEngine;

public class UIInventory : MonoBehaviour
{
    [SerializeField] private GameObject _UIEmptySlot;
    [SerializeField] private Transform _SlotsPoint;

    private List<InventoryItem> _items;
    private List<ItemIcon> _UIitems;
    private List<GameObject> _UIslots;
    private bool _isVisible = true;

    public static UIInventory Instance;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        RenderNewImage();
    }

    private void OnEnable()
    {
        EventManager.CapacityUpdate += UpdateCapacityUI;
    }

    public void UpdateUI()
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
            foreach (ItemIcon _UIitems in _UIitems)
            {
                Destroy(_UIitems.gameObject);
            }
        }
        _UIitems = new List<ItemIcon>();
        int i = 0;
        foreach (var item in _items)
        {
            _UIitems.Add(Instantiate(item.GetIcon(), _UIslots[i].transform));
            i++;
        }
    }

    private bool CheckItem() {
        _items = Inventory.Instanse.GetUIInventoryData();

        return _items != null; //&& _items.Count > 0;
    }



    public void SwitchVisibility() {
        _isVisible = !_isVisible;
        _SlotsPoint.gameObject.SetActive(_isVisible);
        if (_isVisible)
            UpdateUI();
    }

}
