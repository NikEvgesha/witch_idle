using UnityEngine;
using System;
public class StorageCell : MonoBehaviour
{
    private int _index;
    private StorageAction _action;
    private void Start()
    {
        string parentName = transform.parent.GetComponent<StorageGrid>().GetName();
        _action = (parentName == "storage") ? StorageAction.GetFromStorage : StorageAction.PutToStorage;
    }
    public void onCellClick()
    {
        if (this.GetComponentInChildren<ItemIcon>() != null)
        {
            EventManager.StorageCellClick?.Invoke(_action, _index);
        }
 
    }

    public void SetIndex(int idx)
    {
        _index = idx;
    }
}
