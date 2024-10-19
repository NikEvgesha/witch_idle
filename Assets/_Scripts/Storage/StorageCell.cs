using UnityEngine;
using System;
public class StorageCell : MonoBehaviour
{
    private int _index;
    private StorageAction _action;
    private Storage _storage;
    private StorageGrid _storageGrid;
    private void Start()
    {
        //string parentName = transform.parent.GetComponent<StorageGrid>().GetName();
        //_action = (parentName == "storage") ? StorageAction.GetFromStorage : StorageAction.PutToStorage;
        GridUIType gridUI = _storageGrid.GetGridUIType();
        _action = (gridUI == GridUIType.StorageGrid) ? StorageAction.GetFromStorage : StorageAction.PutToStorage; //Тут какбудто можно сразу экшен передавать (точнее на 30-35 строчках)
    }
    public void onCellClick()
    {
        if (this.GetComponentInChildren<ItemIcon>() != null)
        {
            _storage.StorageCellClick?.Invoke(_action, _index);
        }
 
    }
    /*
    public void SetIndex(int idx)
    {
        _index = idx;
    }
    */
    public void SettingCell(int idx, Storage storage, StorageGrid storageGrid)
    {
        _index = idx;
        _storage = storage;
        _storageGrid = storageGrid;
    }
}
